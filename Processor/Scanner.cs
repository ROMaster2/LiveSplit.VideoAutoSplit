using Accord.Video;
using Accord.Video.DirectShow;
using ImageMagick;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
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
        public static Thread Thread;

        public static GameProfile GameProfile = null;
        private static VideoCaptureDevice VideoSource = new VideoCaptureDevice();

        public static Frame CurrentFrame = Frame.Blank;
        public static int CurrentIndex = 0;
        public static bool IsScannerLocked = false;
        public static int ScanningCount = 0;
        public static bool IsScanning { get { return ScanningCount > 0; } }

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
        public static double MinFPS          { get; internal set; } = double.NaN;
        public static double MaxFPS          { get; internal set; } = double.NaN;
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
            VideoSource.SignalToStop();
            UnsubscribeFromFrameHandler(new NewFrameEventHandler(HandleNewFrame));
            while (IsScanning) Thread.Sleep(1);
            CurrentIndex = 0;
            DeltaManager.History = new DeltaResults[DeltaManager.HistorySize];
            VideoSource.Stop();
            while (VideoSource.IsRunning) Thread.Sleep(1);
            Thread?.Abort();
        }

        public static void AsyncStart()
        {
            if (!Thread?.IsAlive ?? false)
            {
                ThreadStart t = new ThreadStart(Start);
                Thread = new Thread(t);
                Thread.Start();
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
                SubscribeToFrameHandler(new NewFrameEventHandler(HandleNewFrame));
            }
        }

        public static void Restart()
        {
            Stop();
            Thread.Sleep(200);
            Start();
        }

        public static void UpdateCropGeometry()
        {
            _TrueCropGeometry = Geometry.Blank;
            if (GameProfile != null)
            {
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
                        wi.SetMagickImage(false);
                        wi.Index = i;
                        i++;
                    }
                }
                // Using this until implementation of multiple screens in the aligner form.
                foreach (var s in GameProfile.Screens)
                {
                    s.CropGeometry = CropGeometry;
                }

                IsScannerLocked = true;
                CompiledFeatures.Compile(GameProfile);
                IsScannerLocked = false;
            }
        }

        // Does this dispose properly?
        private static IMagickImage GetComposedImage(IMagickImage input, int channelIndex, ColorSpace colorSpace)
        {
            IMagickImage mi = input.Clone();
            mi.ColorSpace = colorSpace;
            if (channelIndex > -1)
            {
                mi = input.Separate().ElementAt(channelIndex);
            }
            return mi;
        }

        internal static int initCount = 0; // To stop wasting CPU when first starting.

        public static void HandleNewFrame(object sender, NewFrameEventArgs e)
        {
            var now = TimeStamp.CurrentDateTime.Time;
            initCount++;
            if (!IsScannerLocked && (initCount > 300 || initCount % 10 == 0) && !CompiledFeatures.IsPaused(now))
            {
                ScanningCount++;
                // We should NOT be cloning this much, but then the previous frame gets disposed.
                // I'll figure it out at some point...hopefully.
                var newScan = new Scan(new Frame(now, (Bitmap)e.Frame.Clone()), CurrentFrame.Clone());
                CurrentFrame = new Frame(now, (Bitmap)e.Frame.Clone());
                var index = CurrentIndex;
                CurrentIndex++;
                Task.Factory.StartNew(() => NewRun(newScan, index));
            }
        }

        // Todo: prevFile isn't necessary. Instead store the features of the current scan to be used on the next.
        // That could cause sync problems so it needs to be investigated.
        private static void NewRun(Scan scan, int index)
        {
            var deltas = new double[CompiledFeatures.FeatureCount];
            var benchmarks = new double[CompiledFeatures.FeatureCount];
            using (var fileImageBase = new MagickImage(scan.CurrentFrame.Bitmap))
            using (var prevFileImageBase = CompiledFeatures.HasDupeCheck ? new MagickImage(scan.PreviousFrame.Bitmap) : null)
            {
                foreach (var cWatchZone in CompiledFeatures.CWatchZones)
                    CropScan(ref deltas, ref benchmarks, scan.CurrentFrame.DateTime, fileImageBase, prevFileImageBase, cWatchZone);
            }
            ScanningCount--;
            var scanEnd = TimeStamp.CurrentDateTime.Time;
            scan.Dispose();
            DeltaManager.AddResult(index, scan, scanEnd, deltas, benchmarks);
            NewResult(null, new DeltaManager(index, AverageFPS));

            // It's on its own thread so running it here should be okay.
            if (index % 30 == 0)
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

        private static void CropScan(ref double[] deltas, ref double[] benchmarks, DateTime now, IMagickImage fileImageBase, IMagickImage prevFileImageBase, CWatchZone cWatchZone)
        {
            if (!cWatchZone.IsPaused(now))
            {
                using (var fileImageCropped = fileImageBase.Clone(cWatchZone.MagickGeometry))
                using (var prevFileImageCropped = CompiledFeatures.HasDupeCheck ? prevFileImageBase.Clone(cWatchZone.MagickGeometry) : null)
                {
                    foreach (var cWatcher in cWatchZone.CWatches) ComposeScan(ref deltas, ref benchmarks, now, fileImageCropped, prevFileImageCropped, cWatcher);
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

        private static void ComposeScan(ref double[] deltas, ref double[] benchmarks, DateTime now, IMagickImage fileImageCropped, IMagickImage prevFileImageCropped, CWatcher cWatcher)
        {
            if (!cWatcher.IsPaused(now))
            {
                using (var fileImageComposed = GetComposedImage(fileImageCropped, cWatcher.Channel, cWatcher.ColorSpace))
                using (var prevFileImageComposed = CompiledFeatures.HasDupeCheck ? GetComposedImage(prevFileImageCropped, cWatcher.Channel, cWatcher.ColorSpace) : null)
                {
                    if (cWatcher.Equalize)
                    {
                        fileImageComposed.Equalize();
                        if (CompiledFeatures.HasDupeCheck) prevFileImageComposed.Equalize();
                    }

                    if (cWatcher.IsStandard)
                        foreach (var cWatchImage in cWatcher.CWatchImages) CompareAgainstFeature(ref deltas, ref benchmarks, now, fileImageComposed, cWatcher, cWatchImage);
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
                    if (cWatchImage.HasAlpha) fileImageCompare.Composite(deltaImage, CompositeOperator.CopyAlpha);

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
            if (!cWatchImage.IsPaused(now))
            {
                var benchmark = TimeStamp.Now;
                using (var fileImageCompare = fileImageComposed.Clone())
                using (var prevFileImageCompare = prevFileImageComposed.Clone())
                {
                    SetDelta(ref deltas, fileImageCompare, prevFileImageCompare, cWatcher, cWatchImage);
                    SetBenchmark(ref benchmarks, benchmark, cWatchImage);
                }
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
