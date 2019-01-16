using Accord.Video;
using Accord.Video.DirectShow;
using ImageMagick;
using LiveSplit.Model;
using LiveSplit.VAS.Models;
using LiveSplit.VAS.Models.Delta;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LiveSplit.VAS
{
    public class Scanner : IDisposable
    {
        private VASComponent _Component;

        private Thread _FrameHandlerThread;

        private GameProfile _GameProfile => _Component.GameProfile;
        private string _VideoDevice => _Component.VideoDevice;
        private VideoCaptureDevice _VideoSource;

        private NewFrameEventHandler _NewFrameEventHandler;
        private VideoSourceErrorEventHandler _VideoSourceErrorEventHandler;

        public CompiledFeatures CompiledFeatures { get; private set; }
        public DeltaManager DeltaManager { get; private set; }

        public Frame CurrentFrame = Frame.Blank;
        public int CurrentIndex = 0;
        public bool IsScannerLocked = false;
        public int ScanningCount = 0;
        public bool IsScanning { get { return ScanningCount > 0; } }
        public bool Restarting { get; set; } = false;
        public int OverloadCount = 0;

        internal int InitCount = 0; // To stop wasting CPU when first starting.

        public event EventHandler<Scan> ScanFinished;
        public event EventHandler<DeltaOutput> NewResult;

        // Todo: Add something for downscaling before comparing for large images.

        public Scanner(VASComponent component)
        {
            _Component = component;
            _VideoSource = new VideoCaptureDevice();
            _NewFrameEventHandler = HandleNewFrame;
            _VideoSourceErrorEventHandler = HandleVideoError;
            _CropGeometry = new Geometry(640, 480);
        }

        private Geometry _VideoGeometry = Geometry.Blank;
        public Geometry VideoGeometry
        {
            get
            {
                if (!_VideoGeometry.HasSize)
                {
                    try
                    {
                        if (!IsVideoSourceRunning() && IsVideoSourceValid())
                        {
                            _VideoSource.Source = DeviceMoniker;
                            _VideoSource.Start();
                        }
                        if (IsVideoSourceRunning())
                        {
                            _VideoSource.NewFrame += SetFrameSize;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Couldn't obtain video Geometry.");
                    }
                }
                return _VideoGeometry;
            }
        }

        // Hacky but it saves on CPU for the scanner.
        private void SetFrameSize(object sender, NewFrameEventArgs e)
        {
            if (!_VideoGeometry.HasSize)
            {
                _VideoGeometry = new Geometry(e.Frame.Size.ToWindows());
                _VideoSource.NewFrame -= SetFrameSize;
            }
        }

        private Geometry _CropGeometry = Geometry.Blank;
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
                if (_CropGeometry != value)
                {
                    _CropGeometry = value;
                    UpdateCropGeometry();
                }
            }
        }

        // Bad name.
        // Not fully implemented yet.
        private Geometry _TrueCropGeometry = Geometry.Blank;
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
                        sGeo.Adjust(CropGeometry.X, CropGeometry.Y);
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

        public double ManuallySetFPS { get; set; } = -1;

        public double AverageFPS      { get; private set; } = 60; // Assume 60 so that the start of the VASL script doesn't go haywire.
        public double RecentMinFPS    { get; private set; } = double.MaxValue;
        public double RecentMaxFPS    { get; private set; } = double.Epsilon;
        public double MinFPS          { get; private set; } = double.MaxValue;
        public double MaxFPS          { get; private set; } = double.Epsilon;
        public double AverageScanTime { get; private set; } = 0;
        public double MinScanTime     { get; private set; } = 0;
        public double MaxScanTime     { get; private set; } = 0;
        public double AverageWaitTime { get; private set; } = 0;
        public double MinWaitTime     { get; private set; } = 0;
        public double MaxWaitTime     { get; private set; } = 0;

        public double CurrentFPS
        {
            get
            {
                const double MULTIPLIER_RANGE = 0.05;
                const double LOWER_MULTIPLIER = 1 - MULTIPLIER_RANGE;
                const double UPPER_MULTIPLIER = 1 / LOWER_MULTIPLIER;

                if (ManuallySetFPS > 1
                    && ManuallySetFPS > AverageFPS * LOWER_MULTIPLIER
                    && ManuallySetFPS < AverageFPS * UPPER_MULTIPLIER)
                    return ManuallySetFPS;
                else
                    return AverageFPS;
            }
        }

        public bool IsVideoSourceValid()
        {
            var v = Regex.Match(_VideoDevice, "@device.*}");
            return v.Success && !string.IsNullOrEmpty(new FilterInfo(v.Value).Name);
        }

        public string DeviceMoniker
        {
            get
            {
                if (IsVideoSourceValid())
                {
                    return Regex.Match(_VideoDevice, "@device.*}").Value;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool IsVideoSourceRunning()
        {
            return _VideoSource.IsRunning;
        }

        public void SubscribeToFrameHandler(EventHandler<Scan> method)
        {
            ScanFinished += method;
        }

        public void UnsubscribeFromFrameHandler(EventHandler<Scan> method)
        {
            ScanFinished -= method;
        }

        public Geometry ResetCropGeometry()
        {
            _CropGeometry = Geometry.Blank;
            UpdateCropGeometry();
            return CropGeometry;
        }

        public void Stop()
        {
            Log.Info("Stopping scanner...");
            try
            {
                Log.Verbose("Stopping Frame Handler thread...");
                _FrameHandlerThread?.Abort();
                Log.Verbose("Frame Handler thread stopped.");

                if (_VideoSource != null)
                {
                    Log.Verbose("Signalling video source to stop...");
                    _VideoSource.SignalToStop();
                    Log.Verbose("Signalled video source to stop.");
                    _VideoSource.NewFrame -= _NewFrameEventHandler;
                    _VideoSource.VideoSourceError -= _VideoSourceErrorEventHandler;
                }
                else
                {
                    Log.Verbose("Video source was never set, ignoring.");
                }

                Log.Verbose("Resetting scanner variables...");
                CurrentIndex = 0;
                OverloadCount = 0;
                MinFPS = double.MaxValue;
                MaxFPS = double.Epsilon;
                DeltaManager = null;
                _VideoGeometry = Geometry.Blank;
                _TrueCropGeometry = Geometry.Blank;
                Log.Verbose("Scanner variables reset.");

                if (_VideoSource != null)
                {
                    Log.Verbose("Frame Handler thread stopped. Signalling video source to stop...");
                    _VideoSource.Stop();
                    Log.Verbose("Signalled video source to stop.");
                }

                Log.Info("Scanner stopped.");
            }
            catch (Exception e)
            {
                Log.Error(e, "Scanner failed to stop. This isn't good...");
            }
        }

        public void AsyncStart()
        {
            if (_FrameHandlerThread == null || _FrameHandlerThread.ThreadState != ThreadState.Running)
            {
                Log.Info("Creating scanner thread.");
                ThreadStart t = new ThreadStart(Start);
                _FrameHandlerThread = new Thread(t);
                _FrameHandlerThread.Start();
                Log.Info("Thread created.");
            }
            else
            {
                Restart();
            }
        }

        // Sorry for the nersts mess.
        public void Start()
        {
            try
            {
                Log.Verbose("Initializing start.");
                UpdateCropGeometry();
                Log.Info("Trying to start scanner.");
                if (_GameProfile != null && IsVideoSourceValid() && CompiledFeatures != null)
                {
                    Log.Info("Starting scanner...");
                    CurrentIndex = 0;
                    OverloadCount = 0;
                    DeltaManager = new DeltaManager(CompiledFeatures, 256); // Todo: Unhardcode?
                    InitCount = 0;

                    Log.Verbose("Hooking events onto Accord.");
                    _VideoSource.NewFrame += _NewFrameEventHandler;
                    _VideoSource.VideoSourceError += _VideoSourceErrorEventHandler;
                    Log.Info("Scanner hooked onto video source.");

                    var moniker = DeviceMoniker;
                    if (!string.IsNullOrWhiteSpace(moniker))
                    {
                        try
                        {
                            _VideoSource.Source = moniker;
                            _VideoSource.Start();
                            Log.Info("Scanner started.");
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "Video source failed to start.");
                        }
                    }
                    else
                    {
                        Log.Warning("How did you manage to change the selected device in a few milliseconds?");
                    }
                }
                else
                {
                    var str = "Scanner start failed because:";

                    if (_GameProfile == null) str += " The game profile was empty.";
                    if (!IsVideoSourceValid()) str += " The script was not loaded.";
                    if (CompiledFeatures == null) str += " CompiledFeatures was blank.";

                    Log.Verbose(str);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Unknown Scanner.Start() error, please show this to the component developers.");
            }
        }

        public void Restart()
        {
            if (!Restarting)
            {
                Restarting = true;
                Log.Info("Restarting scanner...");
                Stop();
                Log.Verbose("Stopped, sleeping for 1000ms.");
                Thread.Sleep(1000);
                Start();
                Log.Info("Restart finished.");
                Restarting = false;
            }
            else
            {
                Log.Verbose("THERE CAN BE ONLY ONE THREAD.");
            }
        }

        public void UpdateCropGeometry()
        {
            _TrueCropGeometry = Geometry.Blank;
            if (_Component.IsScriptLoaded() && _GameProfile != null)
            {
                Log.Info("Adjusting profile to set dimensions...");
                IsScannerLocked = true;
                CompiledFeatures = new CompiledFeatures(_GameProfile, CropGeometry);
                IsScannerLocked = false;
                Log.Info("Profile adjusted.");
            }
            else
            {
                Log.Verbose("Game Profile is not set or script is not loaded. UpdateCropGeometry() failed.");
            }
        }

        // Does this dispose properly?
        private static IMagickImage GetComposedImage(Bitmap input, int channelIndex, ColorSpace colorSpace)
        {
            if (input == null)
            {
                return null;
            }

            IMagickImage mi = new MagickImage(input);
            mi.ColorSpace = colorSpace;
            if (channelIndex > -1)
            {
                mi = mi.Separate().ElementAt(channelIndex);
            }
            return mi;
        }

        private void HandleVideoError(object sender, VideoSourceErrorEventArgs e)
        {
            Log.Error(e.Exception, "Video capture fatal error. " + e.Description);

            if (IsVideoSourceRunning())
            {
                Restart();
            }
        }

        public void HandleNewFrame(object sender, NewFrameEventArgs e)
        {
            var now = TimeStamp.CurrentDateTime.Time;
            InitCount++;
            if (!IsScannerLocked &&
                (InitCount > 255 || InitCount % 10 == 0) &&
                !CompiledFeatures.IsPaused(now) &&
                ScanningCount < 12)
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
            else if (ScanningCount >= 12)
            {
                OverloadCount++;
                if (OverloadCount > 50)
                {
                    Log.Warning("Frame handler is too overloaded, restarting scanner...");
                    Restart();
                }
                else
                {
                    Log.Warning("Frame handler is overloaded!!!");
                }
            }
        }

        // Todo: prevFile isn't necessary. Instead store the features of the current scan to be used on the next.
        // That could cause sync problems so it needs to be investigated.
        private void NewRun(Scan scan, int index)
        {
            var deltas = new double[CompiledFeatures.FeatureCount];
            var benchmarks = new double[CompiledFeatures.FeatureCount];
            var now = scan.CurrentFrame.DateTime;
            var fileImageBase = scan.CurrentFrame.Bitmap;
            var prevFileImageBase = CompiledFeatures.UsesDupeCheck(now) ? scan.PreviousFrame.Bitmap : null;

            try
            {
                foreach (var cWatchZone in CompiledFeatures.CWatchZones)
                    CropScan(ref deltas, ref benchmarks, now, fileImageBase, prevFileImageBase, cWatchZone);
            }
            catch (Exception e)
            {
                scan.Dispose();
                Log.Error(e, "Error scanning frame.");
                if (IsVideoSourceRunning() && !IsScannerLocked)
                {
                    ScanningCount--;
                }
            }

            var scanEnd = TimeStamp.CurrentDateTime.Time;

            try
            {
                DeltaManager?.AddResult(index, scan, scanEnd, deltas, benchmarks);
                NewResult(this, new DeltaOutput(DeltaManager, index, CurrentFPS));

                ScanningCount--;

                if (ScanFinished != null) ScanFinished(this, scan);

                //scan.Dispose();

                // It's on its own thread so running it here should be okay.
                if (index % Math.Ceiling(AverageFPS) == 0)
                {
                    RefreshBenchmarks();
                }

                if (index >= DeltaManager.History.Count && AverageFPS > 70d)
                {
                    Log.Warning("Framerate is abnormally high, usually an indicator the video feed is not active.");
                    Restart();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Unknown Scanner Error.");
            }
        }

        private void RefreshBenchmarks()
        {
            int count = 0;
            double sumFPS      = double.Epsilon, minFPS      = double.MaxValue, maxFPS      = double.Epsilon,
                   sumScanTime = double.Epsilon, minScanTime = double.MaxValue, maxScanTime = double.Epsilon,
                   sumWaitTime = double.Epsilon, minWaitTime = double.MaxValue, maxWaitTime = double.Epsilon;
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
            count = Math.Max(count, 1); // Make sure count > 0. count is 0 when History is blank.
            AverageFPS = 3000d / Math.Round(sumFPS / count * 3000d);
            RecentMaxFPS = 1 / minFPS;
            RecentMinFPS = 1 / maxFPS;
            MinFPS = Math.Min(MinFPS, RecentMinFPS);
            MaxFPS = Math.Max(MaxFPS, RecentMaxFPS);
            AverageScanTime = sumScanTime / count;
            MinScanTime = minScanTime;
            MaxScanTime = maxScanTime;
            AverageWaitTime = sumWaitTime / count;
            MinWaitTime = minWaitTime;
            MaxWaitTime = maxWaitTime;

            // Todo: If AverageWaitTime is too high, shrink some of the large images before comparing.
            //       Maybe benchmark individual features though? Some ErrorMetrics are more intense than others.
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
                        ComposeScan(ref deltas, ref benchmarks, now, fileImageCropped, prevFileImageCropped, cWatcher);
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
                        foreach (var cWatchImage in cWatcher.CWatchImages)
                            CompareAgainstFeature(ref deltas, ref benchmarks, now, fileImageComposed, cWatcher, cWatchImage);
                    else if (cWatcher.IsDuplicateFrame)
                        CompareAgainstPreviousFrame(ref deltas, ref benchmarks, now, fileImageComposed, prevFileImageComposed, cWatcher);
                    else
                        throw new NotImplementedException("How'd you get here?");
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
            var metricResult = fileImageCompare.Compare(deltaImage, cWatcher.ErrorMetric);
            deltas[cWatchImage.Index] = cWatcher.ErrorMetric.Standardize(
                cWatchImage.MetricUpperBound,
                metricResult,
                cWatchImage.TransparencyRate);
        }

        private static void SetBenchmark(ref double[] benchmarks, TimeStamp timeStamp, CWatchImage cWatchImage)
        {
            benchmarks[cWatchImage.Index] = (TimeStamp.Now - timeStamp).TotalSeconds;
        }

        public void Dispose()
        {
            Stop();
            IsScannerLocked = true;
        }
    }
}
