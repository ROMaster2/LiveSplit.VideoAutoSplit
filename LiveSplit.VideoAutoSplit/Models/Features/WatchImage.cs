﻿using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using ImageMagick;

namespace LiveSplit.VAS.Models
{
    public class WatchImage
    {
        public int Index;
        public string FilePath;

        // TO REMOVE
        [XmlIgnore]
        public MagickImage MagickImage { get; internal set; }

        [XmlIgnore]
        public Screen Screen { get { return WatchZone.Screen; } }

        [XmlIgnore]
        public WatchZone WatchZone { get { return Watcher.WatchZone; } }

        [XmlIgnore]
        public Watcher Watcher { get; internal set; }

        public string FileName
        {
            get
            {
                var s = FilePath.LastIndexOf('\\');
                var d = FilePath.LastIndexOf('.');
                return FilePath.Substring(s + 1, d - s - 1);
            }
        }

        public string FullName
        {
            get
            {
                return Screen.Name + "/" + WatchZone.Name + "/" + Watcher.Name + " - " + FileName;
            }
        }

        private string _Name;

        public string Name
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_Name))
                {
                    return _Name;
                }
                else
                {
                    return FileName;
                }
            }
            set
            {
                _Name = value;
            }
        }

        private Bitmap _Image;

        public Bitmap Image
        {
            get
            {
                if (_Image == null)
                {
                    if (File.Exists(FilePath))
                    {
                        _Image = new Bitmap(FilePath);
                    }
                    else
                    {
                        throw new FileNotFoundException("Image not located at " + FilePath);
                    }
                }
                return _Image;
            }
            set
            {
                Dispose();
                _Image = value;
            }
        }

        internal WatchImage(Watcher watcher, string filePath)
        {
            Watcher = watcher;
            FilePath = filePath;
        }

        // @TODO: Add exception handling when file does not exist anymore.
        internal WatchImage() { }

        // TO REMOVE
        public void SetName(Screen screen, WatchZone watchZone, Watcher watcher)
        {
            _Name = screen.Name + "/" + watchZone.Name + "/" + watcher.Name + " - " + FileName;
        }

        // TO REMOVE
        /*
        public void SetMagickImage(bool extremePrecision)
        {
            var mi = new MagickImage((Bitmap)Image)
            {
                ColorSpace = Watcher.ColorSpace
            };
            if (!extremePrecision)
            {
                var mGeo = new MagickGeometry(
                    (int)Math.Round(WatchZone.CropGeometry.Width),
                    (int)Math.Round(WatchZone.CropGeometry.Height))
                {
                    IgnoreAspectRatio = true
                };
                mi.Scale(mGeo);
                mi.RePage();
            }
            else
            {
                var underlay = new MagickImage(MagickColor.FromRgba(0, 0, 0, 0), (int)Screen.Geometry.Width, (int)Screen.Geometry.Height);
                underlay.Composite(mi, new PointD(WatchZone.Geometry.Width, WatchZone.Geometry.Height), CompositeOperator.Copy);
                underlay.RePage();
                var mGeo = Scanner.CropGeometry.ToMagick(false);
                mGeo.IgnoreAspectRatio = true;
                underlay.Resize(mGeo);
                underlay.RePage();
                underlay.Trim();
                mi = underlay;
            }
            if (Watcher.Equalize)
            {
                mi.Equalize();
            }
            MagickImage = mi;
        }
        */

        public void Dispose()
        {
            _Image?.Dispose();
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Name)) return Name;
            else return FileName;
        }
    }
}
