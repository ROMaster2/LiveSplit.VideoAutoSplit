using Accord.Video;
using Accord.Video.DirectShow;
using ImageMagick;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using LiveSplit.VAS;
using LiveSplit.VAS.VASL;
using LiveSplit.VAS.Models;
using LiveSplit.UI.Components;
using System.IO;
using System.Linq;
using System.Diagnostics;
using LiveSplit.Model;

using Size = System.Drawing.Size;

namespace LiveSplit.VAS
{
    static class Scanner
    {
        public static Thread FrameHandlerThread;

        public static GameProfile GameProfile = null;
        private static VideoCaptureDevice VideoSource = new VideoCaptureDevice();

        public static Frame CurrentFrame = Frame.Blank;
        public static int CurrentIndex = 0;
        public static bool IsScannerLocked = false;
        public static int ScanningCount = 0;
        public static bool IsScanning { get { return ScanningCount > 0; } }
        public static bool Restarting { get; set; } = false;

        private static NewFrameEventHandler NewFrameEventHandler;
        private static VideoSourceErrorEventHandler VideoSourceErrorEventHandler;
        public static event DeltaResultsHandler NewResult;

        // Todo: Add something for downscaling before comparing for large images.
        public static int OverloadedCount = 0;

        private static Geometry _VideoGeometry = Geometry.Blank;
        public static Geometry VideoGeometry
        {
            get
            {
                if (!_VideoGeometry.HasSize && IsVideoSourceValid())
                {
                    VideoSource.Start();
                    SubscribeToFrameHandler(SetFrameSize);
                    while (!_VideoGeometry.HasSize) Thread.Sleep(1);
                }
                return _VideoGeometry;
            }
        }

        // Hacky but it saves on CPU for the scanner.
        private static void SetFrameSize(object sender, NewFrameEventArgs e)
        {
            if (!_VideoGeometry.HasSize)
            {
                _VideoGeometry = new Geometry(e.Frame.Size.ToWindows());
                UnsubscribeFromFrameHandler(SetFrameSize);
            }
        }

        private static Geometry _CropGeometry = Geometry.Blank;
        public static Geometry CropGeometry
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

        // Bad name.
        private static Geometry _TrueCropGeometry = Geometry.Blank;
        public static Geometry TrueCropGeometry
        {
            get
            {
                if (!_TrueCropGeometry.HasSize)
                {
                    if (GameProfile != null)
                    {
                        double x = 32768d;
                        double y = 32768d;
                        double width = -32768d;
                        double height = -32768d;
                        foreach (var wz in GameProfile.Screens[0].WatchZones)
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
                        sGeo.ResizeTo(CropGeometry, GameProfile.Screens[0].Geometry);
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

        // Good/bad idea?
        public static double AverageFPS      { get; internal set; } = 60; // Assume 60 so that the start of the VASL script doesn't go haywire.
        public static double MinFPS          { get; internal set; } = 60;
        public static double MaxFPS          { get; internal set; } = 60;
        public static double AverageScanTime { get; internal set; } = double.NaN;
        public static double MinScanTime     { get; internal set; } = double.NaN;
        public static double MaxScanTime     { get; internal set; } = double.NaN;
        public static double AverageWaitTime { get; internal set; } = double.NaN;
        public static double MinWaitTime     { get; internal set; } = double.NaN;
        public static double MaxWaitTime     { get; internal set; } = double.NaN;

        public static bool IsVideoSourceValid() // Not really but good enough.
        {
            return !string.IsNullOrEmpty(VideoSource.Source);
        }

        public static bool IsVideoSourceRunning()
        {
            return VideoSource.IsRunning;
        }

        public static void SubscribeToFrameHandler(NewFrameEventHandler method)
        {
            VideoSource.NewFrame += method;
        }

        public static void UnsubscribeFromFrameHandler(NewFrameEventHandler method)
        {
            VideoSource.NewFrame -= method;
        }

        public static void SubscribeToErrorHandler(VideoSourceErrorEventHandler method)
        {
            VideoSource.VideoSourceError += method;
        }

        public static void UnsubscribeFromErrorHandler(VideoSourceErrorEventHandler method)
        {
            VideoSource.VideoSourceError -= method;
        }

        public static void SetVideoSource(string monikerString)
        {
            _VideoGeometry = Geometry.Blank;
            VideoSource.Source = monikerString;
        }

        public static Geometry ResetCropGeometry()
        {
            _CropGeometry = Geometry.Blank;
            UpdateCropGeometry();
            return CropGeometry;
        }

        public static void Stop()
        {
            IsScannerLocked = true;
            VideoSource?.SignalToStop();
            UnsubscribeFromFrameHandler(NewFrameEventHandler);
            CurrentIndex = 0;
            DeltaManager.History = new DeltaResults[DeltaManager.HistorySize];
            VideoSource?.WaitForStop();
            _VideoGeometry = Geometry.Blank;
            FrameHandlerThread?.Abort();
        }

        public static void AsyncStart()
        {
            if (GameProfile != null && IsVideoSourceValid())
            {
                if (FrameHandlerThread == null || FrameHandlerThread.ThreadState != System.Threading.ThreadState.Running)
                {
                    ThreadStart t = new ThreadStart(Start);
                    FrameHandlerThread = new Thread(t);
                    FrameHandlerThread.Start();
                }
            }
        }

        public static void Start()
        {
            UpdateCropGeometry();
            if (GameProfile != null && IsVideoSourceValid())
            {
                CurrentIndex = 0;
                VideoSource.Start();

                initCount = 0;
                NewFrameEventHandler = new NewFrameEventHandler(HandleNewFrame);
                SubscribeToFrameHandler(NewFrameEventHandler);
                VideoSourceErrorEventHandler = new VideoSourceErrorEventHandler(HandleVideoError);
                SubscribeToErrorHandler(VideoSourceErrorEventHandler);
            }
        }

        public static void Restart()
        {
            if (!Restarting)
            {
                LiveSplit.Options.Log.Error("VAS: Fatal error encountered, restarting scanner...");
                Restarting = true;
                if (IsScannerLocked) Thread.Sleep(1);
                if (IsVideoSourceRunning()) Stop();
                Thread.Sleep(1000);
                Start();
                Restarting = false;
            }
        }

        public static void UpdateCropGeometry()
        {
            _TrueCropGeometry = Geometry.Blank;
            if (GameProfile != null)
            {
                IsScannerLocked = true;

                // TO REMOVE
                // I'm afraid that removing it will break things so that'll happen later.
                int i = 0;
                foreach (var wz in GameProfile.Screens[0].WatchZones)
                {
                    var g = wz.Geometry;
                    g.RemoveAnchor(wz.Screen.Geometry);
                    g.ResizeTo(CropGeometry, GameProfile.Screens[0].Geometry);
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
                foreach (var s in GameProfile.Screens)
                {
                    s.CropGeometry = CropGeometry;
                }

                CompiledFeatures.Compile(GameProfile);
                IsScannerLocked = false;
            }
        }

        // Does this dispose properly?
        private static IMagickImage GetComposedImage(Bitmap input, int channelIndex, ColorSpace colorSpace)
        {
            if (input == null) return null;

            IMagickImage mi = new MagickImage(input);
            mi.ColorSpace = colorSpace;
            if (channelIndex > -1)
            {
                mi = mi.Separate().ElementAt(channelIndex);
            }
            return mi;
        }

        private static void HandleVideoError(object sender, VideoSourceErrorEventArgs e)
        {
            Restart();
        }

        internal static int initCount = 0; // To stop wasting CPU when first starting.

        public static void HandleNewFrame(object sender, NewFrameEventArgs e)
        {
            var now = TimeStamp.CurrentDateTime.Time;
            initCount++;
            if (!IsScannerLocked &&
                (initCount > 255 || initCount % 10 == 0) &&
                !CompiledFeatures.IsPaused(now) &&
                ScanningCount < 20)
            {
                ScanningCount++;

                var currentFrame = new Frame(now, (Bitmap)e.Frame.Clone());
                var previousFrame = CurrentFrame;
                CurrentFrame = currentFrame;

                var newScan = new Scan(currentFrame, previousFrame, CompiledFeatures.UseDupeCheck(now));

                var index = CurrentIndex;
                CurrentIndex++;

                Task.Factory.StartNew(() => NewRun(newScan, index));
            }
            else if (ScanningCount >= 20)
            {
                // Todo: Make this do something more.
                LiveSplit.Options.Log.Warning("VAS: Frame handler is overloaded!!!");
            }
        }

        // Todo: prevFile isn't necessary. Instead store the features of the current scan to be used on the next.
        // That could cause sync problems so it needs to be investigated.
        private static void NewRun(Scan scan, int index)
        {
            var deltas = new double[CompiledFeatures.FeatureCount];
            var benchmarks = new double[CompiledFeatures.FeatureCount];
            var now = scan.CurrentFrame.DateTime;
            var fileImageBase = scan.CurrentFrame.Bitmap;
            var prevFileImageBase = !scan.HasPreviousFrame ? scan.PreviousFrame.Bitmap : null;

            try
            {
                foreach (var cWatchZone in CompiledFeatures.CWatchZones)
                    CropScan(ref deltas, ref benchmarks, now, fileImageBase, prevFileImageBase, cWatchZone);
            }
            catch (ArgumentException e)
            {
                LiveSplit.Options.Log.Error(e);
                if (IsVideoSourceRunning() && !IsScannerLocked)
                {
                    ScanningCount--;
                    Restart();
                }
            }
            finally
            {
                var scanEnd = TimeStamp.CurrentDateTime.Time;
                DeltaManager.AddResult(index, scan, scanEnd, deltas, benchmarks);
                NewResult(null, new DeltaManager(index, AverageFPS));

                ScanningCount--;
                scan.Dispose();

                // It's on its own thread so running it here should be okay.
                if (index % Math.Round(AverageFPS) == 0 || AverageFPS < 3d)
                {
                    int count = 0;
                    double sumFPS = 0d, minFPS = 9999d, maxFPS = 0d,
                        sumScanTime = 0d, minScanTime = 9999d, maxScanTime = 0d,
                        sumWaitTime = 0d, minWaitTime = 9999d, maxWaitTime = 0d;
                    foreach (var d in DeltaManager.History)
                    {
                        if (d != null)
                        {
                            count++;
                            var fd = d.FrameDuration.TotalSeconds; // Not handling div by 0. I'll be impressed if it actually happens.
                            sumFPS += fd;
                            minFPS = Math.Min(minFPS, fd);
                            maxFPS = Math.Max(maxFPS, fd);
                            var sd = d.ScanDuration.TotalSeconds;
                            sumScanTime += sd;
                            minScanTime = Math.Min(minScanTime, sd);
                            maxScanTime = Math.Max(maxScanTime, sd);
                            var wd = d.WaitDuration?.TotalSeconds ?? AverageWaitTime;
                            sumWaitTime += wd;
                            minWaitTime = Math.Min(minWaitTime, wd);
                            maxWaitTime = Math.Max(maxWaitTime, wd);
                        }
                    }
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

            if (index >= DeltaManager.HistorySize && AverageFPS > 70d)
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
                using (var prevFileImageCropped = cWatchZone.UseDupeCheck(now) ?
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
                        fileImageCompare.Composite(deltaImage, CompositeOperator.CopyAlpha);
                        fileImageCompare.ColorAlpha(MagickColors.Black);
                        deltaImage.ColorAlpha(MagickColors.Black); // Todo: Add to CWatchImage instead;
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
