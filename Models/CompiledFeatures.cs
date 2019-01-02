using ImageMagick;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace LiveSplit.VAS.Models
{
    public class CompiledFeatures
    {
        private const int INIT_PIXEL_LIMIT = 16777216;
        // @TODO: Something about rectangles
        private static readonly Geometry MIN_GEOMETRY = new Geometry(-8192, -8192, 1, 1);
        private static readonly Geometry MAX_GEOMETRY = new Geometry(8192, 8192, 8192, 8192);

        private readonly bool _HasDupeCheck;

        public Geometry CaptureGeometry { get; }
        public CWatchZone[] CWatchZones { get; }
        public int FeatureCount { get; }
        public int PixelLimit { get; }
        public int PixelCount { get; }
        public IReadOnlyDictionary<string, int> IndexNames { get; }

        public CompiledFeatures(GameProfile gameProfile, Geometry cropGeometry, int pixelLimit = INIT_PIXEL_LIMIT)
        {
            CaptureGeometry = cropGeometry;
            _HasDupeCheck = false;
            PixelLimit = pixelLimit; // Todo: Implement resizing when (total) PixelCount exceeds PixelLimit. It won't be easy.
            PixelCount = 0;

            var nameDictionary = new Dictionary<string, int>();

            var cWatchZones = new CWatchZone[gameProfile.WatchZones.Count];
            var indexCount = 0;

            // Using this until full implementation of multiple screens.
            foreach (var s in gameProfile.Screens)
            {
                s.CropGeometry = CaptureGeometry;
            }

            for (int i1 = 0; i1 < gameProfile.WatchZones.Count; i1++)
            {
                WatchZone watchZone = gameProfile.WatchZones[i1];
                Screen screen = watchZone.Screen;

                var CWatches = new CWatcher[watchZone.Watches.Count];

                var gameGeo = screen.GameGeometry.HasSize ? screen.GameGeometry : screen.Geometry;
                var wzCropGeo = watchZone.WithoutScale(gameGeo);
                wzCropGeo.RemoveAnchor(gameGeo);
                wzCropGeo.ResizeTo(screen.CropGeometry, gameGeo);
                wzCropGeo.Adjust(screen.CropGeometry.X, screen.CropGeometry.Y);
                wzCropGeo = wzCropGeo.Clamp(MIN_GEOMETRY, MAX_GEOMETRY);

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

                            PixelCount += (int)Math.Round(wzCropGeo.Width) * (int)Math.Round(wzCropGeo.Height); // Todo: Un-hardcode the rounding

                            var metricUpperBound = watcher.ErrorMetric.UpperBound(PixelCount, mi.ChannelCount);

                            CWatchImages[i3] = new CWatchImage(watchImage.Name, indexCount, mi, metricUpperBound);

                            AddIndexName(nameDictionary, indexCount, watchZone.Name, watcher.Name, watchImage.FileName);

                            indexCount++;
                        }

                        CWatches[i2] = new CWatcher(CWatchImages, watcher);
                    }
                    else if (watcher.WatcherType == WatcherType.DuplicateFrame)
                    {
                        _HasDupeCheck = true;

                        CWatches[i2] = new CWatcher(new CWatchImage[] { new CWatchImage(watcher.Name, indexCount) }, watcher);

                        AddIndexName(nameDictionary, indexCount, watchZone.Name, watcher.Name, string.Empty);
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
            FeatureCount = indexCount;
            CWatchZones = cWatchZones;
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

        private void ValidateIndexNames()
        {
            var indexes = IndexNames.Values;
            for (int i = 0; i < FeatureCount; i++)
            {
                if (!indexes.Contains(i))
                    Log.Warning("Feature #" + i.ToString() + "'s name was not indexed.");
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
        private void PreciseResize(ref MagickImage mi, Geometry wzGeo, Geometry gameGeo, Geometry cropGeo, ColorSpace cs)
        {
            var underlay = new MagickImage(MagickColors.Transparent, (int)gameGeo.Width, (int)gameGeo.Height) { ColorSpace = cs };
            var point = wzGeo.LocationWithoutAnchor(gameGeo);
            underlay.Composite(mi, new PointD(point.X, point.Y), CompositeOperator.Copy);
            underlay.RePage();
            var mGeo = CaptureGeometry.ToMagick(false);
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

        // Temporary hack, will fix later.
        public void PauseFeature(int index, DateTime untilTime)
        {
            foreach (var cWatchZone in CWatchZones)
            {
                foreach (var cWatcher in cWatchZone.CWatches)
                {
                    foreach (var cWatchImage in cWatcher.CWatchImages)
                    {
                        if (cWatchImage.Index == index)
                        {
                            cWatchImage.Pause(untilTime);
                        }
                    }
                }
            }
        }

        public void ResumeFeature(int index, DateTime untilTime)
        {
            foreach (var cWatchZone in CWatchZones)
            {
                foreach (var cWatcher in cWatchZone.CWatches)
                {
                    foreach (var cWatchImage in cWatcher.CWatchImages)
                    {
                        if (cWatchImage.Index == index)
                        {
                            cWatchImage.Resume(untilTime);
                        }
                    }
                }
            }
        }

        public bool IsPaused(DateTime dateTime)
        {
            return CWatchZones.All(wz => wz.IsPaused(dateTime));
        }

        public bool UsesDupeCheck()
        {
            return _HasDupeCheck;
        }

        public bool UsesDupeCheck(DateTime dateTime)
        {
            return _HasDupeCheck && CWatchZones.Any(wz => wz.UsesDupeCheck(dateTime));
        }

        public CWatchZone this[int i]
        {
            get
            {
                return CWatchZones[i];
            }
        }

        public IEnumerable<CWatcher> CWatchers
        {
            get
            {
                foreach (var wz in CWatchZones)
                {
                    foreach (var w in wz.CWatches)
                    {
                        yield return w;
                    }
                }
            }
        }

        public IEnumerable<CWatchImage> CWatchImages
        {
            get
            {
                foreach (var w in CWatchers)
                {
                    foreach (var wi in w.CWatchImages)
                    {
                        yield return wi;
                    }
                }
            }
        }
    }

    public class CWatchZone
    {
        private bool _HasDupeCheck;

        public string Name { get; }
        public Geometry Geometry { get; }
        public Rectangle Rectangle { get; }
        public CWatcher[] CWatches { get; }

        public CWatchZone(string name, Geometry geometry, CWatcher[] cWatches)
        {
            Name = name;
            Geometry = geometry;
            Rectangle = geometry.ToRectangle();
            CWatches = cWatches;

            _HasDupeCheck = CWatches.Any(w => w.IsDuplicateFrame);
        }

        public bool UsesDupeCheck(DateTime dateTime)
        {
            return _HasDupeCheck && CWatches.Any(w => w.IsDuplicateFrame && !w.IsPaused(dateTime));
        }

        public bool IsPaused(DateTime dateTime)
        {
            return CWatches.All(w => w.IsPaused(dateTime));
        }

        public void Pause(DateTime? dateTime = null)
        {
            foreach (var cWatcher in CWatches)
            {
                cWatcher.Pause(dateTime);
            }
        }

        public void Resume(DateTime? dateTime = null)
        {
            foreach (var cWatcher in CWatches)
            {
                cWatcher.Resume(dateTime);
            }
        }

        public void Dispose()
        {
            foreach (var cWatcher in CWatches)
            {
                cWatcher.Dispose();
            }
        }

        public CWatcher this[int i]
        {
            get
            {
                return CWatches[i];
            }
        }
    }

    public class CWatcher
    {
        public string Name { get; }
        public WatcherType WatcherType { get; }
        public ColorSpace ColorSpace { get; }
        public int Channel { get; }
        public bool Equalize { get; }
        public ErrorMetric ErrorMetric { get; }
        public CWatchImage[] CWatchImages { get; }

        public bool IsStandard { get; }
        public bool IsDuplicateFrame { get; }

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

        public bool IsPaused(DateTime dateTime)
        {
            return CWatchImages.All(wi => wi.IsPaused(dateTime));
        }

        public void Pause(DateTime? dateTime = null)
        {
            foreach (var cWatchImage in CWatchImages)
            {
                cWatchImage.Pause(dateTime);
            }
        }

        public void Resume(DateTime? dateTime = null)
        {
            foreach (var cWatchImage in CWatchImages)
            {
                cWatchImage.Resume(dateTime);
            }
        }

        public void Dispose()
        {
            foreach (var cWatchImage in CWatchImages)
            {
                cWatchImage.Dispose();
            }
        }

        public CWatchImage this[int i]
        {
            get
            {
                return CWatchImages[i];
            }
        }
    }

    public class CWatchImage
    {
        private static readonly MagickColor _AlphaReplacement = MagickColors.Black;

        private long _PauseTicks;

        public string Name { get; }
        public int Index { get; }
        public IMagickImage MagickImage { get; }
        public IMagickImage AlphaChannel { get; }
        public bool HasAlpha { get; }
        public double TransparencyRate { get; }
        public double MetricUpperBound { get; }

        // Standard
        public CWatchImage(string name, int index, IMagickImage magickImage, double metricUpperBound)
        {
            Name = name;
            Index = index;
            MagickImage = magickImage;
            HasAlpha = MagickImage.HasAlpha;
            TransparencyRate = MagickImage.GetTransparencyRate();
            AlphaChannel = null;
            MetricUpperBound = metricUpperBound;

            _PauseTicks = DateTime.MinValue.Ticks;
            
            if (HasAlpha)
            {
                // Does Separate() clone the channels? If so, does it dispose of them during this?
                var tmpMi = MagickImage.Separate().Last().Clone();
                tmpMi.Negate();
                AlphaChannel = new MagickImage(_AlphaReplacement, MagickImage.Width, MagickImage.Height);
                AlphaChannel.Composite(tmpMi, CompositeOperator.CopyAlpha);
                AlphaChannel.RePage();
                tmpMi.Dispose();

                MagickImage.ColorAlpha(_AlphaReplacement);
                MagickImage.RePage();
            }
        }

        // Dummy for Dupe Frame checking
        public CWatchImage(string name, int index)
        {
            Name = name;
            Index = index;
            MagickImage = null;
            AlphaChannel = null;
            HasAlpha = false;
            TransparencyRate = 0;
            MetricUpperBound = 1;

            _PauseTicks = DateTime.MinValue.Ticks;
        }

        public bool IsPaused(DateTime dateTime)
        {
            var now = dateTime.Ticks;
            return _PauseTicks >= 0L ? _PauseTicks > now : -_PauseTicks < now;
        }

        public void Pause(DateTime? dateTime = null)
        {
            _PauseTicks = dateTime?.Ticks ?? DateTime.MaxValue.Ticks;
        }

        public void Resume(DateTime? dateTime = null)
        {
            _PauseTicks = -dateTime?.Ticks ?? DateTime.MinValue.Ticks;
        }

        public void Dispose()
        {
            MagickImage?.Dispose();
            AlphaChannel?.Dispose();
        }
    }
}
