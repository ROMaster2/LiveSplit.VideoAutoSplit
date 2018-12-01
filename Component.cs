using Accord.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.VAS.VASL;
using LiveSplit.VAS;
using LiveSplit.VAS.UI;
using LiveSplit.VAS.Models;
using System.IO.Compression;
using System.Linq;

namespace LiveSplit.UI.Components
{
    public class VASComponent : LogicComponent
    {
        //internal static readonly string[] BASIC_SETTING_TYPES = new string[] { "Start", "Split", "Reset"};

        public override string ComponentName => "Video Auto Splitter";

        private LiveSplitState State;

        private NewComponentSettings ComponentUI;

        private string _ProfilePath = string.Empty;
        public string ProfilePath
        {
            get
            {
                return _ProfilePath;
            }
            set
            {
                if (_ProfilePath != value)
                {
                    ProfileCleanup();
                    _ProfilePath = value;
                }
            }
        }

        private GameProfile _GameProfile = null;
        public GameProfile GameProfile
        {
            get
            {
                if (_GameProfile == null)
                {
                    Log.Info("[VAS] Loading new profile: " + ProfilePath);

                    try
                    {
                        _GameProfile = GameProfile.FromPath(ProfilePath);
                        ProfileChanged?.Invoke(this, _GameProfile);
                        // Todo: Unset GameVersion if the existing one isn't in the new profile. Maybe.

                        if (File.Exists(ProfilePath))
                        {
                            FSWatcher.Path = Path.GetDirectoryName(ProfilePath);
                            FSWatcher.Filter = Path.GetFileName(ProfilePath + "*");
                        }
                        else
                        {
                            FSWatcher.Path = ProfilePath;
                            FSWatcher.Filter = null;
                        }

                        FSWatcher.EnableRaisingEvents = true;

                        TryStartScanner();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                        ErrorLog += e;
                        _GameProfile = null;
                        _Script = null;
                    }
                }
                return _GameProfile;
            }
        }

        private VASLScript _Script = null;
        public VASLScript Script
        {
            get
            {
                if (_Script == null)
                {
                    try
                    {
                        Log.Info("[VAS] Loading script within profile.");
                        _Script = VASLParser.Parse(GameProfile.RawScript);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                        ErrorLog += e;
                        _GameProfile = null;
                        _Script = null;
                    }
                }
                return _Script;
            }
        }

        // To expand upon
        private string _VideoDevice = string.Empty;
        public string VideoDevice {
            get
            {
                return _VideoDevice;
            }
            internal set
            {
                if (_VideoDevice != value)
                {
                    _VideoDevice = value;
                    VideoDeviceChanged?.Invoke(this, _VideoDevice);
                }
            }
        }

        private string _GameVersion = string.Empty;
        public string GameVersion
        {
            get
            {
                return _GameVersion;
            }
            internal set
            {
                if (_GameVersion != value)
                {
                    _GameVersion = value;
                    GameVersionChanged?.Invoke(this, _GameVersion);
                }
            }
        }

        public IDictionary<string, Geometry> CropGeometries { get; internal set; }
        public IDictionary<string, bool> BasicSettingsState { get; internal set; }
        public IDictionary<string, dynamic> CustomSettingsState { get; internal set; }

        // Temporary. Remove later.
        public Geometry CropGeometry { get { return Scanner.CropGeometry; } set { Scanner.CropGeometry = value; } }

        public Scanner Scanner { get; internal set; }
        public string ErrorLog { get; internal set; }

        private FileSystemWatcher FSWatcher;

        public event EventHandler<GameProfile> ProfileChanged;
        public event EventHandler<string> VideoDeviceChanged;
        public event EventHandler<string> GameVersionChanged;
        //public event EventHandler<Geometry> CropGeometryChanged;

        public VASComponent(LiveSplitState state)
        {
            State = state;

            ComponentUI = new NewComponentSettings(this);

            CropGeometries = new Dictionary<string, Geometry>();
            BasicSettingsState = new Dictionary<string, bool>();
            CustomSettingsState = new Dictionary<string, dynamic>();

            Scanner = new Scanner();
            ErrorLog = string.Empty;

            FSWatcher = new FileSystemWatcher();
            FSWatcher.Changed += async (sender, args) => {
                await Task.Delay(200);
                _GameProfile = null;
                _Script = null;
            };

            //Scanner.NewResult += (sender, dm) => UpdateProfile(sender, dm);
        }

        public override Control GetSettingsControl(LayoutMode mode) => ComponentUI;

        public override XmlNode GetSettings(XmlDocument document)
        {
            XmlElement settingsNode = document.CreateElement("Settings");

            settingsNode.AppendChild(SettingsHelper.ToElement(document, "Version", "0.1"));
            settingsNode.AppendChild(SettingsHelper.ToElement(document, "ProfilePath", ProfilePath));
            settingsNode.AppendChild(SettingsHelper.ToElement(document, "VideoDevice", VideoDevice));
            settingsNode.AppendChild(SettingsHelper.ToElement(document, "GameVersion", GameVersion));
            settingsNode.AppendChild(SettingsHelper.ToElement(document, "CropGeometry", CropGeometry));
            AppendBasicSettingsToXml(document, settingsNode);
            AppendCustomSettingsToXml(document, settingsNode);

            return settingsNode;
        }

        public override void SetSettings(XmlNode settings)
        {
            var element = (XmlElement)settings;
            if (!element.IsEmpty)
            {
                ParseStandardSettingsFromXml(element);
                ParseBasicSettingsFromXml(element);
                ParseCustomScriptSettingsFromXml(element);
            }
            //UpdateProfile(null, null);
        }

        public override void Update(IInvalidator i, LiveSplitState s, float w, float h, LayoutMode m) { }

        public override void Dispose()
        {
            //Scanner.Stop();
            ProfileCleanup();
            FSWatcher?.Dispose();
        }

        private void AppendBasicSettingsToXml(XmlDocument document, XmlNode settingsNode)
        {
            var basicSettings = ComponentUI?.SettingsUI?.BasicSettings;
            if (basicSettings != null && basicSettings.Count > 0)
            {
                foreach (var setting in basicSettings)
                {
                    var key = setting.Key;
                    if (BasicSettingsState.ContainsKey(key.ToLower()))
                    {
                        var value = BasicSettingsState[key.ToLower()];
                        settingsNode.AppendChild(SettingsHelper.ToElement(document, key, value));
                    }
                }
            }
        }

        private void AppendCustomSettingsToXml(XmlDocument document, XmlNode parent)
        {
            XmlElement vaslParent = document.CreateElement("CustomSettings");

            foreach (var state in CustomSettingsState)
            {
                XmlElement element = SettingsHelper.ToElement(document, "Setting", state.Value);
                XmlAttribute id = SettingsHelper.ToAttribute(document, "ID", state.Key);

                element.Attributes.Append(id);
                vaslParent.AppendChild(element);
            }

            parent.AppendChild(vaslParent);
        }

        private void ParseStandardSettingsFromXml(XmlElement element)
        {
            ProfilePath = SettingsHelper.ParseString(element["ProfilePath"], string.Empty);
            VideoDevice = SettingsHelper.ParseString(element["VideoDevice"], string.Empty);
            GameVersion = SettingsHelper.ParseString(element["GameVersion"], string.Empty);

            // Hacky? Probably. Geometry should have proper XML support.
            var geo = Geometry.FromString(element["CropGeometry"].InnerText);
            if (geo.HasSize)
            {
                CropGeometry = geo;
            }
        }

        private void ParseBasicSettingsFromXml(XmlElement element)
        {
            var basicSettings = ComponentUI?.SettingsUI?.BasicSettings;
            if (basicSettings != null && basicSettings.Count > 0)
            {
                foreach (var setting in basicSettings)
                {
                    if (element[setting.Key] != null)
                    {
                        var value = bool.Parse(element[setting.Key].InnerText);

                        if (setting.Value.Enabled)
                        {
                            setting.Value.Checked = value;
                        }

                        BasicSettingsState[setting.Key.ToLower()] = value;
                    }
                }
            }
        }

        private void ParseCustomScriptSettingsFromXml(XmlElement data)
        {
            XmlElement customSettingsNode = data["CustomSettings"];

            if (customSettingsNode != null && customSettingsNode.HasChildNodes)
            {
                foreach (XmlElement element in customSettingsNode.ChildNodes)
                {
                    if (element.Name != "Setting")
                    {
                        continue;
                    }

                    string id = element.Attributes["id"].Value;
                    //string type = element.Attributes["type"].Value;
                    string type = "string"; // Temporary until coercion by the script is done.

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
            //SettingUI.UpdateNodesValues(CustomSettingsState, null);
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
            if (!scriptLoaded)
            {
                BasicSettingsState.Clear();
                CustomSettingsState.Clear();
            }

            var values = new Dictionary<string, dynamic>();

            foreach (VASLSetting setting in settings.OrderedSettings)
            {
                var value = setting.Value;
                if (CustomSettingsState.ContainsKey(setting.Id))
                    value = CustomSettingsState[setting.Id];

                setting.Value = value;

                values.Add(setting.Id, value);
            }

            if (scriptLoaded)
                CustomSettingsState = values;

            ComponentUI.InitVASLSettings(settings, scriptLoaded);
        }

        internal void UpdateProfile(object sender, DeltaManager dm)
        {
            try
            {
                Script.Update(State, dm);
            }
            catch (Exception e)
            {
                Log.Error(e);
                ErrorLog += e;
            }
        }

        private void TryStartScanner()
        {
            if (!string.IsNullOrEmpty(VideoDevice))
            {
                try
                {
                    VASLSettings settings = Script.RunStartup(State);
                    SetVASLSettings(settings);
                    Scanner.AsyncStart();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    ErrorLog += e;
                    ProfileCleanup();
                }
            }
        }

        private void ProfileCleanup()
        {
            try
            {
                if (_Script != null)
                {
                    Script.RunShutdown(State);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                ErrorLog += e;
            }
            finally
            {
                _GameProfile = null;
                _Script = null;
                ResetVASLSettings();
            }
        }
    }
}
