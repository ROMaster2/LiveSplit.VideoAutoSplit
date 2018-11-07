using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.IO.Compression;

namespace LiveSplit.VAS.Models
{
    public class GameProfile
    {
        public GameProfile(string name)
        {
            Name = name;
        }

        internal GameProfile() { }

        public string Name;

        public List<Screen> Screens { get; internal set; } = new List<Screen>();

        [XmlIgnore]
        public List<WatchZone> WatchZones
        { get { return Screens.SelectMany(s => s.WatchZones).ToList(); } }
        [XmlIgnore]
        public List<Watcher> Watches
        { get { return WatchZones.SelectMany(wz => wz.Watches).ToList(); } }
        [XmlIgnore]
        public List<WatchImage> WatchImages
        { get { return Watches.SelectMany(w => w.WatchImages).ToList(); } }

        public Screen AddScreen(string name, bool useAdvanced, Geometry geometry)
        {
            var screen = new Screen(this, name, useAdvanced, geometry);
            Screens.Add(screen);
            return screen;
        }

        public void ReSyncRelationships()
        {
            if (Screens.Count > 0) {
                foreach (var s in Screens)
                {
                    s.GameProfile = this;
                    s.ReSyncRelationships();
                }
            }
        }

        public static GameProfile FromXml(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GameProfile));
                GameProfile gp = null;

                // Todo: ACTUAL exception handling.
                try
                {
                    gp = (GameProfile)serializer.Deserialize(reader);
                    gp.ReSyncRelationships();
                }
                catch (Exception e) {
                    System.Diagnostics.Debug.WriteLine("Game Profile failed to load.");
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                    return null;
                }

                return gp;
            }
        }

        public static GameProfile FromXml(string filePath)
        {
            return FromXml(new FileStream(filePath, FileMode.Open, FileAccess.Read));
        }

        public static GameProfile FromZip(string filePath)
        {
            var archive = ZipFile.OpenRead(filePath);
            var entries = archive.Entries;
            var xmlFiles = entries.Where(z => z.Name.Contains(".xml"));
            if (xmlFiles.Count() == 0)
            {
                throw new Exception("Game Profile XML is missing");
            }
            else if (xmlFiles.Count() > 1)
            {
                throw new Exception("Multiple XML files found, we only need one.");
            }
            var stream = xmlFiles.First().Open();

            GameProfile gp = FromXml(stream);

            foreach (var s in gp.Screens)
            {
                var search = s.Name.ToLower() + "_autofit.png";
                var files = entries.Where(z => z.Name.ToLower().Contains(search));
                if (files.Count() == 0)
                {
                    continue;
                }
                else if (files.Count() > 1)
                {
                    throw new Exception("Multiple autofit images found for " + s.Name + ", only one per screen is currently supported.");
                }
                var imageStream = files.First().Open();
                s.Autofitter.Image = new Bitmap(imageStream);
            }
            foreach (var wi in gp.WatchImages)
            {
                var search = wi.FilePath.Replace('\\', '/').ToLower();
                var files = entries.Where(z => z.FullName.ToLower().Contains(search));
                if (files.Count() == 0)
                {
                    throw new Exception("Image for " + search + " not found.");
                }
                else if (files.Count() > 1)
                {
                    throw new Exception("Multiple images found for " + search + ", somehow.");
                }
                var imageStream = files.First().Open();
                wi.Image = new Bitmap(imageStream);
            }

            return gp;
        }

        override public string ToString()
        {
            return Name;
        }
    }
}
