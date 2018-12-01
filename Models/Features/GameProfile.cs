using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.IO.Compression;
using LiveSplit.Options;

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
        public string RawScript = null;

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

        public static GameProfile FromXmlStream(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GameProfile));
                GameProfile gp = (GameProfile)serializer.Deserialize(reader);
                gp.ReSyncRelationships();
                return gp;
            }
        }

        public static GameProfile FromPath(string path)
        {
            GameProfile gp;

            if (File.Exists(path))
                gp = FromZip(path);
            else if (Directory.Exists(path))
                gp = FromFolder(path);
            else
                throw new FileNotFoundException();

            return gp;
        }

        private static GameProfile FromZip(string filePath)
        {
            var archive = ZipFile.OpenRead(filePath);
            var entries = archive.Entries;

            var search = ".xml";
            var xmlFiles = entries.Where(z => z.Name.ToLower().Contains(search));
            if (xmlFiles.Count() == 0)
            {
                throw new Exception("Game Profile XML is missing");
            }
            else if (xmlFiles.Count() > 1)
            {
                throw new Exception("Multiple XML files found, we only need one.");
            }
            var xmlStream = xmlFiles.First().Open();


            GameProfile gp = FromXmlStream(xmlStream);


            search = "script.asl"; // To rename
            var scriptFiles = entries.Where(z => z.Name.ToLower().Contains(search));
            if (scriptFiles.Count() == 0)
            {
                throw new Exception("Script file (script.asl) is missing.");
            }
            else if (scriptFiles.Count() > 1)
            {
                throw new Exception("Multiple script files found, we only need one.");
            }
            var scriptStream = scriptFiles.First().Open();
            gp.RawScript = new StreamReader(scriptStream).ReadToEnd();

            foreach (var s in gp.Screens)
            {
                search = s.Name.ToLower() + "_autofit.png";
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
                search = wi.FilePath.Replace('\\', '/').ToLower();
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

        private static GameProfile FromFolder(string folderPath)
        {
            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);

            var search = "*.xml";
            var xmlFiles = Directory.GetFiles(folderPath, search, SearchOption.TopDirectoryOnly);
            if (xmlFiles.Count() == 0)
            {
                throw new Exception("Game Profile XML is missing");
            }
            else if (xmlFiles.Count() > 1)
            {
                throw new Exception("Multiple XML files found, we only need one.");
            }
            var xmlFile = xmlFiles.First();


            GameProfile gp = FromXmlStream(new FileStream(xmlFile, FileMode.Open, FileAccess.Read));


            search = "script.asl"; // To rename
            var scriptFiles = Directory.GetFiles(folderPath, search, SearchOption.TopDirectoryOnly);
            if (scriptFiles.Count() == 0)
            {
                throw new Exception("Script file (script.asl) is missing.");
            }
            else if (scriptFiles.Count() > 1)
            {
                LiveSplit.Options.Log.Warning("Multiple script files found, we only need one. Using first one.");
            }
            using (var scriptStream = File.Open(scriptFiles.First(), FileMode.Open))
                gp.RawScript = new StreamReader(scriptStream).ReadToEnd();

            foreach (var s in gp.Screens)
            {
                search = s.Name.ToLower() + "_autofit.png";
                var autofitFiles = Directory.GetFiles(folderPath, search, SearchOption.AllDirectories);
                if (autofitFiles.Count() == 0)
                {
                    continue;
                }
                else if (autofitFiles.Count() > 1)
                {
                    LiveSplit.Options.Log.Warning("Multiple autofit images found for screen " + s.Name + ". Using first one.");
                }
                var autofitFile = autofitFiles.First();
                s.Autofitter.Image = new Bitmap(autofitFile);
            }
            foreach (var wi in gp.WatchImages)
            {
                search = wi.FilePath.Replace('\\', '/').ToLower();
                var imageFiles = Directory.GetFiles(folderPath, search, SearchOption.AllDirectories);
                if (imageFiles.Count() == 0)
                {
                    throw new Exception("Image for " + search + " not found.");
                }
                else if (imageFiles.Count() > 1)
                {
                    LiveSplit.Options.Log.Warning("Multiple images found for " + wi.FilePath + ", somehow. Using first one.");
                }
                var imageFile = imageFiles.First();
                wi.Image = new Bitmap(imageFile);
            }

            return gp;
        }

        override public string ToString()
        {
            return Name;
        }
    }
}
