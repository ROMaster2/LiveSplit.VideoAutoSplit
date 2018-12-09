using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Accord.Video;
using Accord.Video.DirectShow;
using ImageMagick;
using LiveSplit.Model;
using LiveSplit.VAS.Models;
using LiveSplit.VAS.Models.Delta;

namespace LiveSplit.VAS
{
    public class Scanner
    {
        // Good/bad idea?
        public double AverageFPS { get; internal set; } = 60; // Assume 60 so that the start of the VASL script doesn't go haywire.
        public double MinFPS { get; internal set; } = 60;
        public double MaxFPS { get; internal set; } = 60;
        public double AverageScanTime { get; internal set; } = double.NaN;
        public double MinScanTime { get; internal set; } = double.NaN;
        public double MaxScanTime { get; internal set; } = double.NaN;
        public double AverageWaitTime { get; internal set; } = double.NaN;
        public double MinWaitTime { get; internal set; } = double.NaN;
        public double MaxWaitTime { get; internal set; } = double.NaN;
        // Todo: Add something for downscaling before comparing for large images.
        public int OverloadedCount = 0;
        public int CurrentIndex = 0;
        public bool IsScannerLocked = false;
        public int ScanningCount = 0;
        public bool IsScanning { get { return ScanningCount > 0; } }
        public bool Restarting { get; set; } = false;
        public string DeviceMoniker => IsVideoSourceValid() ? Regex.Match(_VideoDevice, "@device.*}").Value : null;

        public CompiledFeatures CompiledFeatures { get; private set; }
        public DeltaManager DeltaManager { get; private set; }
        public Frame CurrentFrame = Frame.Blank;
        public Thread FrameHandlerThread;

        internal int initCount = 0; // To stop wasting CPU when first starting.

        private string _VideoDevice => _Component.VideoDevice;
        private readonly VASComponent _Component;
        private GameProfile _GameProfile => _Component.GameProfile;
        private Geometry _CropGeometry = Geometry.Blank;
        private Geometry _TrueCropGeometry = Geometry.Blank; // Bad name.
        private Geometry _VideoGeometry = Geometry.Blank;
        private readonly VideoCaptureDevice _VideoSource = new VideoCaptureDevice();

        public event EventHandler<DeltaOutput> NewResult;

        private readonly NewFrameEventHandler _NewFrameEventHandler;
        private readonly VideoSourceErrorEventHandler _VideoSourceErrorEventHandler;

        public Geometry VideoGeometry
        {
            get
            {
                if (!IsVideoGeometrySet())
                {
                    if (!IsVideoSourceRunning() && IsVideoSourceValid())
                    {
                        _VideoSource.Source = DeviceMoniker;
                        _VideoSource.Start();
                    }
                    if (IsVideoSourceRunning())
                    {
                        Task.Factory.StartNew(() => SubscribeToFrameHandler(SetFrameSize));
                    }
                }
                return _VideoGeometry;
            }
        }

        public Geometry CropGeometry
        {
            get
            {
                if (!_CropGeometry.HasSize)
                {
                    _CropGeometry = VideoGeometry;
                }
                return _CropGeometry;
            }
            set
            {
                _CropGeometry = value;
                UpdateCropGeometry();
            }
        }

        public Geometry TrueCropGeometry
        {
            get
            {
                if (!_TrueCropGeometry.HasSize)
                {
                    if (_GameProfile != null)
                    {
                        double x = 32768d;
                        double y = 32768d;
                        double width = -32768d;
                        double height = -32768d;
                        foreach (var wz in _GameProfile.Screens[0].WatchZones)
                        {
                            var geo = wz.Geometry;
                            geo.RemoveAnchor(wz.Screen.Geometry);
                            x = Math.Min(x, geo.X);
                            y = Math.Min(y, geo.Y);
                            width = Math.Max(width, geo.X + geo.Width);
                            height = Math.Max(height, geo.Y + geo.Height);
                        }
                        width -= x;
                        height -= y;
                        var sGeo = new Geometry(x, y, width, height);
                        sGeo.ResizeTo(CropGeometry, _GameProfile.Screens[0].Geometry);
                        sGeo.Update(CropGeometry.X, CropGeometry.Y);
                        _TrueCropGeometry = sGeo;
                    }
                    else
                    {
                        _TrueCropGeometry = CropGeometry;
                    }
                }
                return _TrueCropGeometry;
            }
        }

        public Scanner(VASComponent component)
        {
            _Component = component;
            _NewFrameEventHandler = HandleNewFrame;
            _VideoSourceErrorEventHandler = HandleVideoError;
        }

        private bool IsVideoGeometrySet()
        {
            return _VideoGeometry.HasSize;
        }

        // Hacky but it saves on CPU for the scanner.
        private void SetFrameSize(object sender, NewFrameEventArgs e)
        {
            if (!_VideoGeometry.HasSize)
            {
                _VideoGeometry = new Geometry(e.Frame.Size.ToWindows());
                UnsubscribeFromFrameHandler(SetFrameSize);
            }
        }

        public bool IsVideoSourceValid()
        {
            var v = Regex.Match(_VideoDevice, "@device.*}");
            return v.Success && !string.IsNullOrEmpty(new FilterInfo(v.Value).Name);
        }

        public bool IsVideoSourceRunning()
        {
            return _VideoSource.IsRunning;
        }

        public void SubscribeToFrameHandler(NewFrameEventHandler method)
        {
            _VideoSource.NewFrame += method;
        }

        public void UnsubscribeFromFrameHandler(NewFrameEventHandler method)
        {
            _VideoSource.NewFrame -= method;
        }

        public void SubscribeToErrorHandler(VideoSourceErrorEventHandler method)
        {
            _VideoSource.VideoSourceError += method;
        }

        public void UnsubscribeFromErrorHandler(VideoSourceErrorEventHandler method)
        {
            _VideoSource.VideoSourceError -= method;
        }

        public Geometry ResetCropGeometry()
        {
            _CropGeometry = Geometry.Blank;
            UpdateCropGeometry();
            return CropGeometry;
        }

        public void Stop()
        {
            IsScannerLocked = true;
            _VideoSource?.SignalToStop();
            UnsubscribeFromFrameHandler(_NewFrameEventHandler);
            CurrentIndex = 0;
            DeltaManager = null;
            _VideoSource?.WaitForStop();
            _VideoGeometry = Geometry.Blank;
            FrameHandlerThread?.Abort();
        }

        public void AsyncStart()
        {
            if (FrameHandlerThread == null || FrameHandlerThread.ThreadState != ThreadState.Running)
            {
                ThreadStart t = new ThreadStart(Start);
                FrameHandlerThread = new Thread(t);
                FrameHandlerThread.Start();
            }
        }

        public void Start()
        {
            UpdateCropGeometry();
            if (_GameProfile != null && IsVideoSourceValid())
            {
                CurrentIndex = 0;
                DeltaManager = new DeltaManager(CompiledFeatures, 256); // Todo: Unhardcode?
                initCount = 0;

                SubscribeToFrameHandler(_NewFrameEventHandler);
                SubscribeToErrorHandler(_VideoSourceErrorEventHandler);

                _VideoSource.Source = DeviceMoniker;
                _VideoSource.Start();
            }
        }

        public void Restart()
        {
            if (!Restarting)
            {
                _Component.LogEvent("Fatal error encountered, restarting scanner...");
                Restarting = true;
                if (IsScannerLocked) Thread.Sleep(1);
                Stop();
                Thread.Sleep(1000);
                Start();
                Restarting = false;
            }
        }

        public void UpdateCropGeometry()
        {
            _TrueCropGeometry = Geometry.Blank;
            if (_GameProfile != null)
            {
                IsScannerLocked = true;

                // @TODO: TO REMOVE
                // I'm afraid that removing it will break things so that'll happen later.
                int i = 0;
                foreach (var wz in _GameProfile.Screens[0].WatchZones)
                {
                    var g = wz.Geometry;
                    g.RemoveAnchor(wz.Screen.Geometry);
                    g.ResizeTo(CropGeometry, _GameProfile.Screens[0].Geometry);
                    g.Update(CropGeometry.X, CropGeometry.Y);
                    wz.CropGeometry = g;

                    foreach (var wi in wz.WatchImages)
                    {
                        //wi.SetMagickImage(false);
                        wi.Index = i;
                        i++;
                    }
                }
                // Using this until implementation of multiple screens in the aligner form.
                foreach (var s in _GameProfile.Screens) s.CropGeometry = CropGeometry;

                CompiledFeatures = new CompiledFeatures(_GameProfile, CropGeometry);
                IsScannerLocked = false;
            }
        }

        // Does this dispose properly?
        private static IMagickImage GetComposedImage(Bitmap input, int channelIndex, ColorSpace colorSpace)
        {
            if (input == null) return null;

            IMagickImage mi = new MagickImage(input)
            {
                ColorSpace = colorSpace
            };

            return (channelIndex > -1) ? mi = mi.Separate().ElementAt(channelIndex) : mi;
        }

        private void HandleVideoError(object sender, VideoSourceErrorEventArgs e)
        {
            _Component.LogEvent(e.Description);

            if (e.Exception != null)
            {
                _Component.LogEvent("Accord exception details:");
                _Component.LogEvent(e.Exception);
            }

            if (IsVideoSourceRunning())
            {
                Restart();
            }
        }

        public void HandleNewFrame(object sender, NewFrameEventArgs e)
        {
            var now = TimeStamp.CurrentDateTime.Time;
            var test = !CompiledFeatures.IsPaused(now);
            initCount++;
            if (!IsScannerLocked
                && (initCount > 255 || initCount % 10 == 0)
                && !CompiledFeatures.IsPaused(now)
                && ScanningCount < 20)
            {
                ScanningCount++;

                var currentFrame = new Frame(now, (Bitmap)e.Frame.Clone());
                var previousFrame = CurrentFrame;
                CurrentFrame = currentFrame;

                var newScan = new Scan(currentFrame, previousFrame, CompiledFeatures.UsesDupeCheck(now));

                var index = CurrentIndex;
                CurrentIndex++;

                Task.Factory.StartNew(() => NewRun(newScan, index));
            }
            else if (ScanningCount >= 20)
            {
                // @TODO: Make this do something more.
                LiveSplit.Options.Log.Warning("VAS: Frame handler is overloaded!!!");
            }
        }

        // @TODO: prevFile isn't necessary. Instead store the features of the current scan to be used on the next.
        // That could cause sync problems so it needs to be investigated.
        private void NewRun(Scan scan, int index)
        {
            var deltas = new double[CompiledFeatures.FeatureCount];
            var benchmarks = new double[CompiledFeatures.FeatureCount];
            var now = scan.CurrentFrame.DateTime;
            var fileImageBase = scan.CurrentFrame.Bitmap;
            var prevFileImageBase = !scan.HasPreviousFrame ? scan.PreviousFrame.Bitmap : null;

            try
            {
                foreach (var cWatchZone in CompiledFeatures.CWatchZones)
                {
                    CropScan(ref deltas, ref benchmarks, now, fileImageBase, prevFileImageBase, cWatchZone);
                }
            }
            catch (ArgumentException e)
            {
                LiveSplit.Options.Log.Error(e);
                if (IsVideoSourceRunning() && !IsScannerLocked)
                {
                    ScanningCount--;
                }
            }
            catch (InvalidOperationException e)
            {
                LiveSplit.Options.Log.Error(e);
                if (IsVideoSourceRunning() && !IsScannerLocked)
                {
                    ScanningCount--;
                }
            }
            catch (AccessViolationException e)
            {
                LiveSplit.Options.Log.Error(e);
                if (IsVideoSourceRunning() && !IsScannerLocked)
                {
                    ScanningCount--;
                }
            }
            finally
            {
                var scanEnd = TimeStamp.CurrentDateTime.Time;

                DeltaManager.AddResult(index, scan, scanEnd, deltas, benchmarks);

                NewResult(null, new DeltaOutput(DeltaManager, index, AverageFPS));

                ScanningCount--;
                scan.Dispose();

                // It's on its own thread so running it here should be okay.
                if (index % Math.Ceiling(AverageFPS) == 0)
                {
                    int count = 0;
                    double sumFPS = 0d, minFPS = 9999d, maxFPS = 0d,
                        sumScanTime = 0d, minScanTime = 9999d, maxScanTime = 0d,
                        sumWaitTime = 0d, minWaitTime = 9999d, maxWaitTime = 0d;
                    foreach (var d in DeltaManager.History)
                    {
                        if (!d.IsBlank)
                        {
                            count++;
                            var fd = d.FrameDuration.TotalSeconds;
                            sumFPS += fd;
                            minFPS = Math.Min(minFPS, fd);
                            maxFPS = Math.Max(maxFPS, fd);
                            var sd = d.ScanDuration.TotalSeconds;
                            sumScanTime += sd;
                            minScanTime = Math.Min(minScanTime, sd);
                            maxScanTime = Math.Max(maxScanTime, sd);
                            var wd = d.WaitDuration.TotalSeconds;
                            sumWaitTime += wd;
                            minWaitTime = Math.Min(minWaitTime, wd);
                            maxWaitTime = Math.Max(maxWaitTime, wd);
                        }
                    }
                    count = Math.Max(count, 1); // Somehow we get NaNs...
                    AverageFPS = 3000d / Math.Round(sumFPS / count * 3000d);
                    MaxFPS = 1 / minFPS;
                    MinFPS = 1 / maxFPS;
                    AverageScanTime = sumScanTime / count;
                    MinScanTime = minScanTime;
                    MaxScanTime = maxScanTime;
                    AverageWaitTime = sumWaitTime / count;
                    MinWaitTime = minWaitTime;
                    MaxWaitTime = maxWaitTime;

                    // Todo: If AverageWaitTime is too high, shrink some of the large images before comparing.
                    //       Maybe benchmark individual features though? Some ErrorMetrics are more intense than others.
                }
            }

            if (index >= DeltaManager.History.Count && AverageFPS > 70d)
            {
                Restart();
            }
        }

        private static void CropScan(
            ref double[] deltas,
            ref double[] benchmarks,
            DateTime now,
            Bitmap fileImageBase,
            Bitmap prevFileImageBase,
            CWatchZone cWatchZone)
        {
            if (!cWatchZone.IsPaused(now))
            {
                using (var fileImageCropped = fileImageBase.Clone(cWatchZone.Rectangle, PixelFormat.Format24bppRgb))
                using (var prevFileImageCropped = cWatchZone.UsesDupeCheck(now) ?
                    prevFileImageBase?.Clone(cWatchZone.Rectangle, PixelFormat.Format24bppRgb) : null)
                {
                    foreach (var cWatcher in cWatchZone.CWatches)
                    {
                        ComposeScan(ref deltas, ref benchmarks, now, fileImageCropped, prevFileImageCropped, cWatcher);
                    }
                }
            }
            else
            {
                foreach (var cWatcher in cWatchZone.CWatches)
                {
                    foreach (var cWatchImage in cWatcher.CWatchImages)
                    {
                        deltas[cWatchImage.Index] = double.NaN;
                        benchmarks[cWatchImage.Index] = 0;
                    }
                }
            }
        }

        private static void ComposeScan(
            ref double[] deltas,
            ref double[] benchmarks,
            DateTime now,
            Bitmap fileImageCropped,
            Bitmap prevFileImageCropped,
            CWatcher cWatcher)
        {
            if (!cWatcher.IsPaused(now))
            {
                using (var fileImageComposed = GetComposedImage(fileImageCropped, cWatcher.Channel, cWatcher.ColorSpace))
                using (var prevFileImageComposed = GetComposedImage(prevFileImageCropped, cWatcher.Channel, cWatcher.ColorSpace))
                {
                    if (cWatcher.Equalize)
                    {
                        fileImageComposed.Equalize();
                        prevFileImageComposed?.Equalize();
                    }

                    if (cWatcher.IsStandard)
                    {
                        foreach (var cWatchImage in cWatcher.CWatchImages)
                        {
                            CompareAgainstFeature(ref deltas, ref benchmarks, now, fileImageComposed, cWatcher, cWatchImage);
                        }
                    }
                    else if (cWatcher.IsDuplicateFrame)
                    {
                        CompareAgainstPreviousFrame(ref deltas, ref benchmarks, now, fileImageComposed, prevFileImageComposed, cWatcher);
                    }
                    else
                    {
                        throw new NotImplementedException("How'd you get here?");
                    }
                }
            }
            else
            {
                foreach (var cWatchImage in cWatcher.CWatchImages)
                {
                    deltas[cWatchImage.Index] = double.NaN;
                    benchmarks[cWatchImage.Index] = 0;
                }
            }
        }

        private static void CompareAgainstFeature(
            ref double[] deltas,
            ref double[] benchmarks,
            DateTime now,
            IMagickImage fileImageComposed,
            CWatcher cWatcher,
            CWatchImage cWatchImage)
        {
            if (!cWatchImage.IsPaused(now))
            {
                var benchmark = TimeStamp.Now;
                using (var fileImageCompare = fileImageComposed.Clone())
                using (var deltaImage = cWatchImage.MagickImage.Clone())
                {
                    if (cWatchImage.HasAlpha)
                    {
                        fileImageCompare.Composite(cWatchImage.AlphaChannel, CompositeOperator.Over);
                        //fileImageCompare.Write(@"C:\test7.png");
                    }

                    SetDelta(ref deltas, fileImageCompare, deltaImage, cWatcher, cWatchImage);
                    SetBenchmark(ref benchmarks, benchmark, cWatchImage);
                }
            }
            else
            {
                deltas[cWatchImage.Index] = double.NaN;
                benchmarks[cWatchImage.Index] = 0;
            }
        }

        private static void CompareAgainstPreviousFrame(
            ref double[] deltas,
            ref double[] benchmarks,
            DateTime now,
            IMagickImage fileImageComposed,
            IMagickImage prevFileImageComposed,
            CWatcher cWatcher)
        {
            var cWatchImage = cWatcher.CWatchImages[0];
            if (!cWatchImage.IsPaused(now) && prevFileImageComposed != null)
            {
                var benchmark = TimeStamp.Now;
                SetDelta(ref deltas, fileImageComposed, prevFileImageComposed, cWatcher, cWatchImage);
                SetBenchmark(ref benchmarks, benchmark, cWatchImage);
            }
            else
            {
                deltas[cWatchImage.Index] = double.NaN;
                benchmarks[cWatchImage.Index] = 0;
            }
        }

        private static void SetDelta(
            ref double[] deltas,
            IMagickImage fileImageCompare,
            IMagickImage deltaImage,
            CWatcher cWatcher,
            CWatchImage cWatchImage)
        {
            deltas[cWatchImage.Index] = fileImageCompare.Compare(deltaImage, cWatcher.ErrorMetric);
        }

        private static void SetBenchmark(ref double[] benchmarks, TimeStamp timeStamp, CWatchImage cWatchImage)
        {
            benchmarks[cWatchImage.Index] = (TimeStamp.Now - timeStamp).TotalSeconds;
        }
    }
}
