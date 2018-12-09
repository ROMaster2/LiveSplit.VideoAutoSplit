using System;
using System.Collections.Generic;
using LiveSplit.Options;

namespace LiveSplit.VAS.VASL
{
    public class VASLSetting
    {
        public string Id { get; private set; }
        public string Label { get; private set; }
        public string Type { get; private set; }
        public dynamic Value { get; set; }
        public dynamic DefaultValue { get; private set; }
        public string Parent { get; private set; }
        public string ToolTip { get; set; }

        public VASLSetting(string id, dynamic defaultValue, string label, string parent)
        {
            Id = id;
            Value = defaultValue;
            DefaultValue = defaultValue;
            Label = label;
            Parent = parent;
            Type = ((object)defaultValue).GetType().ToString();
        }

        public override string ToString()
        {
            return Label;
        }
    }

    public class VASLSettings
    {
        public IDictionary<string, VASLSetting> Settings { get; set; }
        public IList<VASLSetting> OrderedSettings { get; private set; }

        public IDictionary<string, VASLSetting> BasicSettings { get; private set; }

        public VASLSettingsBuilder Builder;
        public VASLSettingsReader Reader;

        public VASLSettings()
        {
            Settings = new Dictionary<string, VASLSetting>();
            OrderedSettings = new List<VASLSetting>();
            BasicSettings = new Dictionary<string, VASLSetting>();
            Builder = new VASLSettingsBuilder(this);
            Reader = new VASLSettingsReader(this);
        }

        public void AddSetting(string name, dynamic defaultValue, string description, string parent)
        {
            if (description == null)
            {
                description = name;
            }

            if (parent != null && !Settings.ContainsKey(parent))
            {
                throw new ArgumentException($"Parent for setting '{name}' is not a setting: {parent}");
            }

            if (Settings.ContainsKey(name))
            {
                throw new ArgumentException($"Setting '{name}' was already added");
            }

            var setting = new VASLSetting(name, defaultValue, description, parent);
            Settings.Add(name, setting);
            OrderedSettings.Add(setting);
        }

        public dynamic GetSettingValue(string name)
        {
            // Don't cause error if setting doesn't exist, but still inform script
            // author since that usually shouldn't happen.
            if (Settings.ContainsKey(name))
            {
                return GetSettingValueRecursive(Settings[name]);
            }

            Log.Info("[VASL] Custom Setting Key doesn't exist: " + name);

            return false;
        }

        public void AddBasicSetting(string name)
        {
            BasicSettings.Add(name, new VASLSetting(name, true, "", null));
        }

        public bool GetBasicSettingValue(string name)
        {
            if (BasicSettings.ContainsKey(name))
            {
                return BasicSettings[name].Value;
            }

            return false;
        }

        public bool IsBasicSettingPresent(string name)
        {
            return BasicSettings.ContainsKey(name);
        }

        /// <summary>
        /// Returns true only if this setting and all it's parent settings are true.
        /// </summary>
        private dynamic GetSettingValueRecursive(VASLSetting setting)
        {
            if (!setting.Value)
            {
                return false;
            }

            if (setting.Parent == null)
            {
                return setting.Value;
            }

            return GetSettingValueRecursive(Settings[setting.Parent]);
        }
    }

    /// <summary>
    /// Interface for adding settings via the VASL Script.
    /// </summary>
    public class VASLSettingsBuilder
    {
        public string CurrentDefaultParent { get; set; }
        private readonly VASLSettings _s;

        public VASLSettingsBuilder(VASLSettings s)
        {
            _s = s;
        }

        public void Add(string id, dynamic default_value, string description = null, string parent = null)
        {
            if (parent == null)
            {
                parent = CurrentDefaultParent;
            }

            _s.AddSetting(id, default_value, description, parent);
        }

        public void SetToolTip(string id, string text)
        {
            if (!_s.Settings.ContainsKey(id))
            {
                throw new ArgumentException($"Can't set tooltip, '{id}' is not a setting");
            }

            _s.Settings[id].ToolTip = text;
        }
    }

    /// <summary>
    /// Interface for reading settings via the VASL Script.
    /// </summary>
    public class VASLSettingsReader
    {
        private readonly VASLSettings _s;

        public VASLSettingsReader(VASLSettings s)
        {
            _s = s;
        }

        public dynamic this[string id]
        {
            get { return _s.GetSettingValue(id); }
        }

        public bool ContainsKey(string key)
        {
            return _s.Settings.ContainsKey(key);
        }

        public bool StartEnabled
        {
            get { return _s.GetBasicSettingValue("start"); }
        }

        public bool ResetEnabled
        {
            get { return _s.GetBasicSettingValue("reset"); }
        }

        public bool SplitEnabled
        {
            get { return _s.GetBasicSettingValue("split"); }
        }
    }
}
