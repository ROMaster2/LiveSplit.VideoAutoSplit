using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace LiveSplit.VFM.Models
{
    public class Screen
    {
        internal Screen(GameProfile gameProfile, string name, bool useAdvanced, Geometry geometry)
        {
            GameProfile = gameProfile;
            Name = name;
            UseAdvanced = useAdvanced;
            Geometry = geometry;
        }

        internal Screen() { }

        public string Name;
        public Geometry Geometry;
        [XmlIgnore]
        public Geometry CropGeometry; // Temporary place
        [XmlIgnore]
        public Geometry GameGeometry; // Temporary place

        public bool UseAdvanced;
        public List<WatchZone> WatchZones = new List<WatchZone>();
        public WatchImage Autofitter;

        [XmlIgnore]
        public GameProfile GameProfile { get; internal set; }
        [XmlIgnore]
        public List<Watcher> Watches
        { get { var a = new List<Watcher>(); a.AddRange(WatchZones.SelectMany(wz => wz.Watches)); return a; } }
        [XmlIgnore]
        public List<WatchImage> WatchImages
        { get { var a = new List<WatchImage>(); a.AddRange(Watches.SelectMany(w => w.WatchImages)); return a; } }

        public void ReSyncRelationships()
        {
            if (WatchZones.Count > 0)
            {
                foreach (var wz in WatchZones)
                {
                    wz.Screen = this;
                    wz.ReSyncRelationships();
                }
            }
        }

        override public string ToString()
        {
            return Name;
        }
    }
}
