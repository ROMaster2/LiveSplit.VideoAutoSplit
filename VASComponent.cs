using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using LiveSplit.VAS.Models;
using LiveSplit.VAS.Models.Delta;
using LiveSplit.VAS.UI;
using LiveSplit.VAS.VASL;

namespace LiveSplit.VAS
{
    public class VASComponent : LogicComponent
    {
        public static readonly Version Version = typeof(VASComponent).Assembly.GetName().Version;

        public override string ComponentName => "Video Auto Splitter";

        private LiveSplitState State;

        private ComponentUI ComponentUI;

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
                    // This is so the event logger for ProfileCleanup doesn't run at start.
                    if (!string.IsNullOrEmpty(_ProfilePath))
                    {
                        ProfileCleanup();
                    }
                    try
                    {
                        _ProfilePath = value;
                        ProfileChanged?.Invoke(this, GameProfile); // Invokes getter
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Profile path failed to set.");
                        _ProfilePath = string.Empty;
                        ProfileCleanup();
                    }
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
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(_ProfilePath))
                        {
                            Log.Info("Loading new game profile: " + ProfilePath);

                            _GameProfile = GameProfile.FromPath(ProfilePath);
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

                            Log.Info("Game profile successfully loaded!");

                            // Todo: Log this separately. Just don't want to atm.
                            VASLSettings settings = Script.RunStartup(State);
                            SetVASLSettings(settings);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Error loading Game profile:");
                        ProfileCleanup();
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
                        var gp = GameProfile; // Invoke getter
                        Log.Info("Loading VASL script within profile...");
                        _Script = new VASLScript(gp.RawScript, GameVersion);
                        Log.Info("VASL script successfully loaded!");
                        TryStartScanner();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Error loading VASL script:");
                        ProfileCleanup();
                    }
                }
                return _Script;
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
                    _Script = null; // Testing
                }
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
                    TryStartScanner();
                }
            }
        }

        public IDictionary<string, Geometry> CropGeometries { get; internal set; }
        public IDictionary<string, bool> BasicSettingsState { get; internal set; }
        public IDictionary<string, dynamic> CustomSettingsState { get; internal set; }

        // Temporary. Remove later.
        public Geometry CropGeometry { get { return Scanner.CropGeometry; } set { Scanner.CropGeometry = value; } }

        public Scanner Scanner { get; internal set; }

        private FileSystemWatcher FSWatcher;

        public event EventHandler<GameProfile> ProfileChanged;
        public event EventHandler<string> VideoDeviceChanged;
        public event EventHandler<string> GameVersionChanged;
        //public event EventHandler<Geometry> CropGeometryChanged;

        public VASComponent(LiveSplitState state)
        {
            Log.Info("Establishing Video Auto Splitter, standby...");

            State = state;

            ComponentUI = new ComponentUI(this);

            CropGeometries = new Dictionary<string, Geometry>();
            BasicSettingsState = new Dictionary<string, bool>();
            CustomSettingsState = new Dictionary<string, dynamic>();

            Scanner = new Scanner(this);

            FSWatcher = new FileSystemWatcher();
            FSWatcher.Changed +=  (sender, args) => {
                ProfileCleanup();
                ProfileChanged?.Invoke(this, GameProfile);
            };

            Scanner.NewResult += (sender, dm) => RunScript(sender, dm);
        }

        public override Control GetSettingsControl(LayoutMode mode) => ComponentUI;

        #region Save Settings

        public override XmlNode GetSettings(XmlDocument document)
        {
            XmlElement settingsNode = document.CreateElement("Settings");

            settingsNode.AppendChild(SettingsHelper.ToElement(document, "Version", VASComponent.Version));
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

        #endregion Save Settings

        #region Load Settings

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

        private void ParseStandardSettingsFromXml(XmlElement element)
        {
            ProfilePath = SettingsHelper.ParseString(element["ProfilePath"], string.Empty);
            GameVersion = SettingsHelper.ParseString(element["GameVersion"], string.Empty);

            // Hacky? Probably. Geometry should have proper XML support.
            var geo = Geometry.FromString(element["CropGeometry"].InnerText);
            if (geo.HasSize) CropGeometry = geo;

            VideoDevice = SettingsHelper.ParseString(element["VideoDevice"], string.Empty);
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
                    if (!String.Equals(element.Name, "Setting")) continue;

                    string id = element.Attributes["id"]?.Value;
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

        #endregion Load Settings

        #region VASL Settings

        private void InitVASLSettings(VASLSettings settings, bool scriptLoaded)
        {
            if (!scriptLoaded)
            {
                BasicSettingsState.Clear();
                CustomSettingsState.Clear();
            }
            else
            {
                var values = new Dictionary<string, dynamic>();

                foreach (var setting in settings.OrderedSettings)
                {
                    var value = setting.Value;
                    if (CustomSettingsState.ContainsKey(setting.Id))
                        value = CustomSettingsState[setting.Id];

                    setting.Value = value;

                    values.Add(setting.Id, value);
                }

                CustomSettingsState = values;
            }

            ComponentUI.InitVASLSettings(settings, scriptLoaded);
        }

        public void SetVASLSettings(VASLSettings settings)
        {
            InitVASLSettings(settings, true);
        }

        public void ResetVASLSettings()
        {
            InitVASLSettings(new VASLSettings(), false);
        }

        #endregion VASL Settings

        public bool IsScriptLoaded()
        {
            return _Script != null;
        }

        internal void RunScript(object sender, DeltaOutput d)
        {
            try
            {
                Script.Update(State, d);
            }
            catch (Exception e)
            {
                Log.Error(e, "VASL Script failed to process frame.");
            }
        }

        private void TryStartScanner()
        {
            try
            {
                if (!string.IsNullOrEmpty(VideoDevice))
                {
                    Scanner.AsyncStart();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Scanner failed to initialize");
                ProfileCleanup();
            }
        }

        private void ProfileCleanup()
        {
            Log.Info("Cleaning up profile...");
            try
            {
                FSWatcher.EnableRaisingEvents = false;
                if (_Script != null)
                {
                    Script.RunShutdown(State);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to run shutdown on script, skipping.");
            }
            finally
            {
                Scanner.Stop();
                _GameProfile = null;
                _Script = null;
                ResetVASLSettings();
            }
            Log.Info("Profile cleanup finished.");
        }

        public override void Dispose()
        {
            Log.Info("Disposing...");
            _ProfilePath = null;
            ProfileCleanup();
            Scanner.Dispose();
            FSWatcher?.Dispose();
            ComponentUI.Dispose();
            Log.Info("Closing...");
            Log.Flush();
        }

        public override void Update(IInvalidator i, LiveSplitState s, float w, float h, LayoutMode m) { }
    }
}
