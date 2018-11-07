using ImageMagick;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace LiveSplit.VFM.Models
{
    public class WatchZone
    {
        internal WatchZone(Screen screen, string name, ScaleType scaleType, Geometry geometry)
        {
            Screen = screen;
            Name = name;
            ScaleType = scaleType;
            Geometry = geometry;
        }

        internal WatchZone() { }

        public string Name;
        public Geometry Geometry;
        [XmlIgnore]
        public Geometry CropGeometry;

        public ScaleType ScaleType;
        public List<Watcher> Watches = new List<Watcher>();

        [XmlIgnore]
        public Screen Screen { get; internal set; }
        [XmlIgnore]
        public List<WatchImage> WatchImages
        { get { var a = new List<WatchImage>(); a.AddRange(Watches.SelectMany(w => w.WatchImages)); return a; } }


        public Watcher AddWatcher(string name, double frequency = 1d, ColorSpace colorSpace = ColorSpace.RGB)
        {
            var watcher = new Watcher(this, name, frequency, colorSpace);
            Watches.Add(watcher);
            return watcher;
        }

        public void ReSyncRelationships()
        {
            if (Watches.Count > 0)
            {
                foreach (var w in Watches)
                {
                    w.WatchZone = this;
                    w.ReSyncRelationships();
                }
            }
        }

        public Geometry WithoutScale(Geometry gameGeometry)
        {
            double xScale = gameGeometry.Width  / Screen.Geometry.Width;
            double yScale = gameGeometry.Height / Screen.Geometry.Height;

            double _x = Geometry.X;
            double _y = Geometry.Y;
            double _width = Geometry.Width;
            double _height = Geometry.Height;

            if (ScaleType == ScaleType.KeepRatio)
            {
                double scale = Math.Min(xScale, yScale); // Min or Max? Probably Min.
                xScale = scale;
                yScale = scale;
            }
            if (ScaleType != ScaleType.NoScale)
            {
                _x *= xScale;
                _y *= yScale;
                _width *= xScale;
                _height *= yScale;
            }

            return new Geometry(_x, _y, _width, _height, Geometry.Anchor);
        }

        override public string ToString()
        {
            return Name;
        }
    }
}
