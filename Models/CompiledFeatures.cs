using ImageMagick;
using System;
using System.Collections.Generic;
using System.Threading;

namespace LiveSplit.VAS.Models
{
    public static class CompiledFeatures
    {
        private const int INIT_PIXEL_LIMIT = 16777216;

        public static CWatchZone[] CWatchZones { get; internal set; }
        public static int FeatureCount { get; internal set; }
        public static bool HasDupeCheck { get; internal set; }
        public static int PixelLimit { get; internal set; }
        public static int PixelCount { get; internal set; }
        public static IDictionary<string, int> IndexName { get; internal set; }

        private static void AddIndexName(int index, string name1, string name2, string name3)
        {
            var strings = new List<string>
            {
                name1,
                name1 + "/" + name2,
                name2,
                name1 + "/" + name2 + "/" + name3,
                name1 + "/" + name3,
                name2 + "/" + name3,
            };
            foreach (var s in strings)
            {
                if (IndexName.ContainsKey(s))
                {
                    IndexName[s] = -1;
                }
                else
                {
                    IndexName.Add(new KeyValuePair<string, int>(s, index));
                }
            }
        }

        public static void Compile(GameProfile gameProfile, int pixelLimit = INIT_PIXEL_LIMIT)
        {
            if (CWatchZones != null)
            {
                foreach (var cWatchZone in CWatchZones)
                {
                    cWatchZone.Dispose();
                }
                Array.Clear(CWatchZones, 0, CWatchZones.Length);
            }
            HasDupeCheck = false;
            PixelLimit = pixelLimit; // Todo
            PixelCount = 0;
            IndexName = new Dictionary<string, int>();

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
                        var CWatchImages = new CWatchImage[watcher.WatchImages.Count];

                        for (int i3 = 0; i3 < watcher.WatchImages.Count; i3++)
                        {
                            WatchImage watchImage = watcher.WatchImages[i3];
                            var mi = new MagickImage(watchImage.Image) { ColorSpace = watcher.ColorSpace };

                            GetComposedImage(ref mi, watcher.Channel);
                            mi.Write(@"E:\memes0.png");
                            PreciseResize(ref mi, watchZone.Geometry, gameGeo, screen.CropGeometry, watcher.ColorSpace);
                            if (watcher.Equalize) mi.Equalize();

                            CWatchImages[i3] = new CWatchImage(watchImage.Name, indexCount, mi);
                            AddIndexName(indexCount, watchZone.Name, watcher.Name, watchImage.FileName);
                            indexCount++;
                        }

                        CWatches[i2] = new CWatcher(CWatchImages, watcher);
                    }
                    else if (watcher.WatcherType == WatcherType.DuplicateFrame)
                    {
                        HasDupeCheck = true;
                        throw new NotImplementedException("todo lol");
                    }
                }

                cWatchZones[i1] = new CWatchZone(watchZone.Name, wzCropGeo, CWatches);
            }

            CWatchZones = cWatchZones;
        }

        public static void StandardResize(ref MagickImage mi, MagickGeometry geo)
        {
            geo.IgnoreAspectRatio = true;
            mi.Scale(geo);
            mi.RePage();
        }

        // Todo: Test this more...and clean it up.
        public static void PreciseResize(ref MagickImage mi, Geometry wzGeo, Geometry gameGeo, Geometry cropGeo, ColorSpace cs)
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
        public static void GetComposedImage(ref MagickImage mi, int channelIndex)
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
        public string Name;
        public Geometry TrueGeometry;
        public MagickGeometry MagickGeometry;
        public CWatcher[] CWatches;
        public void Dispose()
        {
            foreach (var x in CWatches)
            {
                x.Dispose();
            }
            Array.Clear(CWatches, 0, CWatches.Length);
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

        public string Name;
        public WatcherType WatcherType;
        public ColorSpace ColorSpace;
        public int Channel;
        public bool Equalize;
        public ErrorMetric ErrorMetric;
        public CWatchImage[] CWatchImages;

        public bool IsStandard;
        public bool IsDuplicateFrame;
        public void Dispose()
        {
            foreach (var x in CWatchImages)
            {
                x.Dispose();
            }
            Array.Clear(CWatchImages, 0, CWatchImages.Length);
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

        // Duplicate dummy
        public CWatchImage(string name, int index)
        {
            Name = name;
            Index = index;
            MagickImage = null;
            HasAlpha = false;
            TransparencyRate = 0;
        }

        public string Name;
        public int Index;
        public IMagickImage MagickImage;
        public bool HasAlpha;
        public double TransparencyRate;
        public void Dispose()
        {
            MagickImage?.Dispose();
        }
    }
}
