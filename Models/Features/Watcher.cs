using ImageMagick;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace LiveSplit.VFM.Models
{
    public class Watcher
    {
        internal Watcher(WatchZone watchZone, string name, double frequency = 1d, ColorSpace colorSpace = ColorSpace.RGB)
        {
            WatchZone = watchZone;
            Name = name;
            ColorSpace = colorSpace;
            Frequency = frequency;
        }

        internal Watcher() { }

        public string Name;

        public double Frequency = 1d;

        public ColorSpace ColorSpace = ColorSpace.sRGB;
        public int Channel = -1;
        public bool Equalize = true;

        public ErrorMetric ErrorMetric = ErrorMetric.PeakSignalToNoiseRatio;

        public WatcherType WatcherType = WatcherType.Standard;
        public bool DupeFrameCheck = false; // To remove

        public List<WatchImage> WatchImages = new List<WatchImage>();

        [XmlIgnore]
        public Screen Screen { get { return WatchZone.Screen; } }
        [XmlIgnore]
        public WatchZone WatchZone { get; internal set; }

        public WatchImage AddWatchImage(string filePath)
        {
            var watchImage = new WatchImage(this, filePath);
            WatchImages.Add(watchImage);
            return watchImage;
        }

        public void ReSyncRelationships()
        {
            if (WatchImages.Count > 0)
            {
                foreach (var wi in WatchImages)
                {
                    wi.Watcher = this;
                }
            }
        }

        override public string ToString()
        {
            return Name;
        }
    }
}
