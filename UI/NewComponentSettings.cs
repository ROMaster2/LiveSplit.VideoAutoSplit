using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Accord.Video.DirectShow;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using LiveSplit.VAS;
using LiveSplit.VAS.Models;
using LiveSplit.VAS.UI;
using LiveSplit.VAS.VASL;

namespace LiveSplit.UI.Components
{
    public partial class NewComponentSettings : UserControl
    {
        private Options    Options;
        private ScanRegion ScanRegion;
        private Features   Features;
        private Debug      Debug;

        public string ProfilePath { get { return Options.ProfilePath; } set { Options.ProfilePath = value; } }
        public string VideoDevice { get { return Options.VideoDevice; } set { Options.VideoDevice = value; } }
        public string GameVersion { get { return Options.GameVersion; } set { Options.GameVersion = value; } }
        internal IDictionary<string, CheckBox> BasicSettings;
        internal IDictionary<string, bool> BasicSettingsState;
        internal IDictionary<string, dynamic> CustomSettingsState;

        public GameProfile GameProfile { get { return Scanner.GameProfile; } set { Scanner.GameProfile = value; } }
        public Geometry CropGeometry { get { return Scanner.CropGeometry; } set { Scanner.CropGeometry = value; } }

        public NewComponentSettings(string profilePath = null)
        {
            InitializeComponent();

            Options    = new Options(profilePath);
            ScanRegion = new ScanRegion();
            Features   = new Features();
            Debug      = new Debug();

            BasicSettings = Options.BasicSettings;
            BasicSettingsState = Options.BasicSettingsState;
            CustomSettingsState = Options.CustomSettingsState;
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            XmlElement settingsNode = document.CreateElement("Settings");

            settingsNode.AppendChild(SettingsHelper.ToElement(document, "Version", "0.1")); // Todo: Make const
            settingsNode.AppendChild(SettingsHelper.ToElement(document, "ProfilePath", ProfilePath));
            settingsNode.AppendChild(SettingsHelper.ToElement(document, "VideoDevice", VideoDevice));
            settingsNode.AppendChild(SettingsHelper.ToElement(document, "GameVersion", GameVersion));
            settingsNode.AppendChild(SettingsHelper.ToElement(document, "CropGeometry", CropGeometry));
            AppendBasicSettingsToXml(document, settingsNode);
            AppendCustomSettingsToXml(document, settingsNode);

            return settingsNode;
        }

        private void AppendBasicSettingsToXml(XmlDocument document, XmlNode settingsNode)
        {
            foreach (var state in BasicSettings)
            {
                if (BasicSettingsState.ContainsKey(state.Key.ToLower()))
                {
                    var value = BasicSettingsState[state.Key.ToLower()];
                    settingsNode.AppendChild(SettingsHelper.ToElement(document, state.Key, value));
                }
            }
        }

        private void AppendCustomSettingsToXml(XmlDocument document, XmlNode parent)
        {
            XmlElement vaslParent = document.CreateElement("CustomSettings");

            foreach (var state in CustomSettingsState)
            {
                var obj = (object)state.Value;
                XmlElement element = SettingsHelper.ToElement(document, "Setting", state.Value);
                XmlAttribute id   = SettingsHelper.ToAttribute(document, "id", state.Key);
                XmlAttribute type = SettingsHelper.ToAttribute(document, "type", obj.GetType().ToString());

                element.Attributes.Append(id);
                element.Attributes.Append(type);
                vaslParent.AppendChild(element);
            }

            parent.AppendChild(vaslParent);
        }

        public void SetSettings(XmlNode settings)
        {
            var element = (XmlElement)settings;
            if (!element.IsEmpty)
            {
                ParseStandardOptionsFromXml(element);
                ParseBasicSettingsFromXml(element);
                ParseCustomSettingsFromXml(element);
            }
        }

        private void ParseStandardOptionsFromXml(XmlElement element)
        {
            ProfilePath = SettingsHelper.ParseString(element["ProfilePath"], string.Empty);
            VideoDevice = SettingsHelper.ParseString(element["VideoDevice"], string.Empty);
            GameVersion = SettingsHelper.ParseString(element["GameVersion"], string.Empty);

            if (element["CropGeometry"] != null)
            {
                var geo = new Geometry()
                {
                    X = SettingsHelper.ParseDouble(element["CropX"], 0),
                    Y = SettingsHelper.ParseDouble(element["CropY"], 0),
                    Width = SettingsHelper.ParseDouble(element["CropWidth"], 0),
                    Height = SettingsHelper.ParseDouble(element["CropHeight"], 0)
                };
            }
            /*
            try
            {
                GameProfile = GameProfile.FromPath(ProfilePath);
            }
            catch (Exception e)
            {
                LiveSplit.Options.Log.Error(e); // Change, probably
            }*/
        }

        private void ParseBasicSettingsFromXml(XmlElement element)
        {
            foreach (var setting in BasicSettings)
            {
                if (element[setting.Key] != null)
                {
                    var value = bool.Parse(element[setting.Key].InnerText);

                    if (setting.Value.Enabled)
                        setting.Value.Checked = value;

                    BasicSettingsState[setting.Key.ToLower()] = value;
                }
            }
        }

        private void ParseCustomSettingsFromXml(XmlElement data)
        {
            XmlElement customSettingsNode = data["CustomSettings"];

            if (customSettingsNode != null && customSettingsNode.HasChildNodes)
            {
                foreach (XmlElement element in customSettingsNode.ChildNodes)
                {
                    if (element.Name != "Setting")
                        continue;

                    string id = element.Attributes["id"].Value;
                    string type = element.Attributes["type"].Value;

                    if (id != null)
                    {
                        dynamic value;
                        switch (type)
                        {
                            case "bool":
                                value = SettingsHelper.ParseBool(element);
                                break;
                            case "float":
                            case "double":
                                value = SettingsHelper.ParseDouble(element);
                                break;
                            case "byte":
                            case "sbyte":
                            case "short":
                            case "ushort":
                            case "int":
                            case "uint":
                            case "long":
                            case "ulong":
                                value = SettingsHelper.ParseInt(element);
                                break;
                            case "char":
                            case "string":
                                value = SettingsHelper.ParseString(element);
                                break;
                            case "timespan":
                                value = SettingsHelper.ParseTimeSpan(element);
                                break;
                            default:
                                throw new NotSupportedException("Data type is either incorrect or unsupported.");
                        }
                        CustomSettingsState[id] = value;
                    }
                }
            }
            Options.UpdateNodesValues(CustomSettingsState, null);
        }

        public void SetVASLSettings(VASLSettings settings)
        {
            InitVASLSettings(settings, true);
        }

        public void ResetVASLSettings()
        {
            InitVASLSettings(new VASLSettings(), false);
        }

        private void InitVASLSettings(VASLSettings settings, bool scriptLoaded)
        {
            Options.InitVASLSettings(settings, scriptLoaded);
        }
                
    }
}
