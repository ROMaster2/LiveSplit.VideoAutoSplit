using ImageMagick;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LiveSplit.Model;

namespace LiveSplit.VAS.Models
{
    public static class CompiledFeatures
    {
        private const int INIT_PIXEL_LIMIT = 16777216;

        public static CWatchZone[] CWatchZones { get; private set; }
        public static int FeatureCount { get; private set; }
        public static bool HasDupeCheck { get; private set; }
        public static int PixelLimit { get; private set; }
        public static int PixelCount { get; private set; }
        public static IReadOnlyDictionary<string, int> IndexNames { get; private set; }
        internal static IDictionary<int, long> PauseIndex { get; private set; }

        public static void Compile(GameProfile gameProfile, int pixelLimit = INIT_PIXEL_LIMIT)
        {
            Scanner.IsScannerLocked = true;
            while (Scanner.ScanningCount > 0) { Thread.Sleep(5); }

            if (CWatchZones != null)
            {
                foreach (var cWatchZone in CWatchZones)
                {
                    cWatchZone.Dispose();
                }
            }
            HasDupeCheck = false;
            PixelLimit = pixelLimit; // Todo: Implement resizing when (total) PixelCount exceeds PixelLimit. It won't be easy.
            PixelCount = 0;

            var nameDictionary = new Dictionary<string, int>();
            var pauseDictionary = new Dictionary<int, long>();

            var cWatchZones = new CWatchZone[gameProfile.WatchZones.Count];
            var indexCount = 0;

            for (int i1 = 0; i1 < gameProfile.WatchZones.Count; i1++)
            {
                WatchZone watchZone = gameProfile.WatchZones[i1];
                Screen screen = watchZone.Screen;

                var CWatches = new CWatcher[watchZone.Watches.Count];

                var gameGeo = screen.GameGeometry.HasSize ? screen.GameGeometry : screen.Geometry;
                var wzCropGeo = watchZone.WithoutScale(gameGeo);
                wzCropGeo.RemoveAnchor(gameGeo);
                wzCropGeo.ResizeTo(screen.CropGeometry, gameGeo);
                wzCropGeo.Update(screen.CropGeometry.X, screen.CropGeometry.Y);

                for (int i2 = 0; i2 < watchZone.Watches.Count; i2++)
                {
                    Watcher watcher = watchZone.Watches[i2];

                    if (watcher.WatcherType == WatcherType.Standard)
                    {
                        var wiCount = watcher.WatchImages.Count;
                        var CWatchImages = new CWatchImage[wiCount];

                        if (wiCount <= 0)
                            throw new ArgumentException("Standard Watchers require at least one image to compare against.");

                        for (int i3 = 0; i3 < wiCount; i3++)
                        {
                            WatchImage watchImage = watcher.WatchImages[i3];
                            var mi = new MagickImage(watchImage.Image) { ColorSpace = watcher.ColorSpace };

                            GetComposedImage(ref mi, watcher.Channel);
                            StandardResize(ref mi, wzCropGeo);
                            //PreciseResize(ref mi, watchZone.Geometry, gameGeo, screen.CropGeometry, watcher.ColorSpace);
                            if (watcher.Equalize) mi.Equalize();

                            CWatchImages[i3] = new CWatchImage(watchImage.Name, indexCount, mi);

                            pauseDictionary.Add(indexCount, -DateTime.MaxValue.Ticks);
                            AddIndexName(nameDictionary, indexCount, watchZone.Name, watcher.Name, watchImage.FileName);
                            PixelCount += (int)Math.Round(wzCropGeo.Width) * (int)Math.Round(wzCropGeo.Height); // Todo: Un-hardcode the rounding
                            indexCount++;
                        }

                        CWatches[i2] = new CWatcher(CWatchImages, watcher);
                    }
                    else if (watcher.WatcherType == WatcherType.DuplicateFrame)
                    {
                        HasDupeCheck = true;

                        CWatches[i2] = new CWatcher(new CWatchImage[] { new CWatchImage(watcher.Name, indexCount, null) }, watcher);

                        pauseDictionary.Add(indexCount, -DateTime.MaxValue.Ticks);
                        AddIndexName(nameDictionary, indexCount, watchZone.Name, watcher.Name, watcher.Name);
                        PixelCount += (int)Math.Round(wzCropGeo.Width) * (int)Math.Round(wzCropGeo.Height); // Todo: Un-hardcode the rounding
                        indexCount++;
                    }
                    else
                    {
                        throw new NotImplementedException("WatcherType is not set or does not exist. This really shouldn't happen.");
                    }
                }

                cWatchZones[i1] = new CWatchZone(watchZone.Name, wzCropGeo, CWatches);
            }

            IndexNames = nameDictionary;
            PauseIndex = pauseDictionary;
            FeatureCount = indexCount;
            CWatchZones = cWatchZones;

            Scanner.IsScannerLocked = false;
            ValidateIndexNames();
        }

        private static void AddIndexName(IDictionary<string, int> dictionary, int index, string name1, string name2, string name3)
        {
            var strings = new List<string>
            {
                name1,
                name1 + "/" + name2,
                name2,
                name1 + "/" + name2 + "/" + name3,
                name1 + "/" + name3,
                name2 + "/" + name3,
                name3
            };
            foreach (var s in strings)
            {
                if (dictionary.ContainsKey(s))
                {
                    dictionary[s] = -1;
                }
                else
                {
                    dictionary.Add(new KeyValuePair<string, int>(s, index));
                }
            }
        }

        private static void ValidateIndexNames()
        {
            var indexes = IndexNames.Values;
            for (int i = 0; i < FeatureCount; i++)
            {
                if (!indexes.Contains(i))
                    LiveSplit.Options.Log.Warning("Feature #" + i.ToString() + "'s name was not indexed.");
            }
        }

        private static void StandardResize(ref MagickImage mi, Geometry geo)
        {
            var mGeo = geo.ToMagick(false);
            mGeo.IgnoreAspectRatio = true;
            mi.Scale(mGeo);
            mi.RePage();
        }

        // Todo: Test this more...and clean it up.
        private static void PreciseResize(ref MagickImage mi, Geometry wzGeo, Geometry gameGeo, Geometry cropGeo, ColorSpace cs)
        {
            var underlay = new MagickImage(MagickColors.Transparent, (int)gameGeo.Width, (int)gameGeo.Height) { ColorSpace = cs };
            var point = wzGeo.LocationWithoutAnchor(gameGeo);
            underlay.Composite(mi, new PointD(point.X, point.Y), CompositeOperator.Copy);
            underlay.RePage();
            var mGeo = Scanner.CropGeometry.ToMagick(false);
            mGeo.IgnoreAspectRatio = true;
            underlay.Resize(mGeo);
            underlay.RePage();
            underlay.Trim();
            underlay.RePage();
            mi = underlay;
        }

        // May need to update to support multiple channels.
        // Todo: Put this elsewhere. It's copied from Scanner.cs...
        private static void GetComposedImage(ref MagickImage mi, int channelIndex)
        {
            if (channelIndex > -1)
            {
                var mic = mi.Separate();
                mi.Dispose();
                int i = 0;
                foreach (var x in mic)
                {
                    if (i == channelIndex)
                    {
                        mi = (MagickImage)x;
                    }
                    else
                    {
                        x.Dispose();
                    }
                    i++;
                }
            }
        }

        public static void PauseFeature(int index, DateTime untilTime)
        {
            PauseIndex[index] = untilTime.Ticks;
        }

        public static void ResumeFeature(int index, DateTime untilTime)
        {
            PauseIndex[index] = -untilTime.Ticks;
        }

        public static bool IsPaused(DateTime dateTime)
        {
            return CWatchZones.All(wz => wz.IsPaused(dateTime));
        }
    }

    public struct CWatchZone
    {
        public CWatchZone(string name, Geometry geometry, CWatcher[] cWatches)
        {
            Name = name;
            TrueGeometry = geometry;
            MagickGeometry = geometry.ToMagick();
            CWatches = cWatches;
        }
        public string Name { get; }
        public Geometry TrueGeometry { get; }
        public MagickGeometry MagickGeometry { get; }
        public CWatcher[] CWatches { get; }

        public bool IsPaused(DateTime dateTime)
        {
            return CWatches.All(w => w.IsPaused(dateTime));
        }

        public void Dispose()
        {
            foreach (var cWatcher in CWatches)
            {
                cWatcher.Dispose();
            }
        }
    }

    public struct CWatcher
    {
        public CWatcher(
            CWatchImage[] cWatchImages,
            string name,
            WatcherType watcherType,
            ColorSpace colorSpace,
            int channel,
            bool equalize,
            ErrorMetric errorMetric
            )
        {
            Name = name;
            WatcherType = watcherType;
            ColorSpace = colorSpace;
            Channel = channel;
            Equalize = equalize;
            ErrorMetric = errorMetric;
            CWatchImages = cWatchImages;

            IsStandard = WatcherType.Equals(WatcherType.Standard);
            IsDuplicateFrame = WatcherType.Equals(WatcherType.DuplicateFrame);
        }
        public CWatcher(CWatchImage[] cWatchImages, Watcher watcher)
        {
            Name = watcher.Name;
            WatcherType = watcher.WatcherType;
            ColorSpace = watcher.ColorSpace;
            Channel = watcher.Channel;
            Equalize = watcher.Equalize;
            ErrorMetric = watcher.ErrorMetric;
            CWatchImages = cWatchImages;

            IsStandard = WatcherType.Equals(WatcherType.Standard);
            IsDuplicateFrame = WatcherType.Equals(WatcherType.DuplicateFrame);
        }

        public string Name { get; }
        public WatcherType WatcherType { get; }
        public ColorSpace ColorSpace { get; }
        public int Channel { get; }
        public bool Equalize { get; }
        public ErrorMetric ErrorMetric { get; }
        public CWatchImage[] CWatchImages { get; }

        public bool IsStandard { get; }
        public bool IsDuplicateFrame { get; }

        public bool IsPaused(DateTime dateTime)
        {
            return CWatchImages.All(wi => wi.IsPaused(dateTime));
        }

        public void Dispose()
        {
            foreach (var cWatchImage in CWatchImages)
            {
                cWatchImage.Dispose();
            }
        }
    }

    public struct CWatchImage
    {
        // Standard
        public CWatchImage(string name, int index, IMagickImage magickImage)
        {
            Name = name;
            Index = index;
            MagickImage = magickImage;
            HasAlpha = MagickImage.HasAlpha;
            TransparencyRate = MagickImage.TransparencyRate();
        }

        // Dummy for Dupe Frame checking
        public CWatchImage(string name, int index)
        {
            Name = name;
            Index = index;
            MagickImage = null;
            HasAlpha = false;
            TransparencyRate = 0;
        }

        public string Name { get; }
        public int Index { get; }
        public IMagickImage MagickImage { get; }
        public bool HasAlpha { get; }
        public double TransparencyRate { get; }

        public bool IsPaused(DateTime dateTime)
        {
            long value = CompiledFeatures.PauseIndex[Index];
            long now = dateTime.Ticks;
            return value > 0L ? value > now : -value < now;
        }

        public void Dispose()
        {
            MagickImage?.Dispose();
        }

        internal bool Testing
        {
            get
            {
                return IsPaused(TimeStamp.CurrentDateTime.Time);
            }
        }
    }
}
