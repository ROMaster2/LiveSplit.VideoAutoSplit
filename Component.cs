using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.VAS.VASL;
using LiveSplit.VAS;
using LiveSplit.VAS.Forms;
using LiveSplit.VAS.Models;
using System.IO.Compression;
using System.Linq;

namespace LiveSplit.UI.Components
{
    public class VASComponent : LogicComponent
    {
        public override string ComponentName => "Video Auto Splitter";

        public VASLScript Script { get; private set; }

        public event EventHandler ScriptChanged;

        private bool _do_reload;
        private string _old_script_path;

        private FileSystemWatcher _fs_watcher;

        private ComponentSettings _settings;

        private LiveSplitState _state;

        public VASComponent(LiveSplitState state)
        {
            _state = state;

            _settings = new ComponentSettings(this);

            _fs_watcher = new FileSystemWatcher();
            _fs_watcher.Changed += async (sender, args) => {
                await Task.Delay(200);
                _do_reload = true;
            };

            Scanner.NewResult += (sender, dm) => UpdateScript(sender, dm);
        }

        public VASComponent(LiveSplitState state, string script_path)
            : this(state)
        {
            _settings = new ComponentSettings(this, script_path);
        }

        public override void Dispose()
        {
            Scanner.Stop();
            ScriptCleanup();

            try
            {
                ScriptChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            _fs_watcher?.Dispose();
        }

        public override Control GetSettingsControl(LayoutMode mode)
        {
            return _settings;
        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            return _settings.GetSettings(document);
        }

        public override void SetSettings(XmlNode settings)
        {
            _settings.SetSettings(settings);
            UpdateScript(null, null);
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height,
            LayoutMode mode)
        { }

        internal void UpdateScript(object sender, DeltaManager dm)
        {
            if (_settings.ScriptPath != _old_script_path || _do_reload)
            {
                try
                {
                    _do_reload = false;
                    _old_script_path = _settings.ScriptPath;

                    ScriptCleanup();

                    if (string.IsNullOrEmpty(_settings.ScriptPath))
                    {
                        _fs_watcher.EnableRaisingEvents = false;
                    }
                    else
                    {
                        LoadScript();
                    }

                    ScriptChanged?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }

            if (Script != null)
            {
                try
                {
                    Script.Update(_state, dm);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

        private void LoadScript()
        {
            try {
                Log.Info("[VASL] Loading new profile: " + _settings.ScriptPath);

                var gp = GameProfile.FromZip(_settings.ScriptPath);
                Scanner.GameProfile = gp;

                Log.Info("[VASL] Loading new script: " + _settings.ScriptPath);

                _fs_watcher.Path = Path.GetDirectoryName(_settings.ScriptPath);
                _fs_watcher.Filter = Path.GetFileName(_settings.ScriptPath + ".tmp");
                _fs_watcher.EnableRaisingEvents = true;

                var archive = ZipFile.OpenRead(_settings.ScriptPath);
                var entries = archive.Entries;
                var stream = entries.Where(z => z.Name == "script.asl").First().Open();
                var script = new StreamReader(stream).ReadToEnd();

                // New script
                Script = VASLParser.Parse(script);

                Script.GameVersionChanged += (sender, version) => _settings.SetGameVersion(version);
                _settings.SetGameVersion(null);
            }
            catch (Exception ex)
            {
                // Todo: Update UI if Game Profile failed to load.
                Log.Error(ex);
                ScriptCleanup();
            }

            // Give custom VASL settings to GUI, which populates the list and
            // stores the VASLSetting objects which are shared between the GUI
            // and VASLScript
            try
            {
                VASLSettings settings = Script.RunStartup(_state);
                _settings.SetVASLSettings(settings);
                Scanner.AsyncStart();
            }
            catch (Exception ex)
            {
                // Script already created, but startup failed, so clean up again
                Log.Error(ex);
                ScriptCleanup();
            }
        }

        private void ScriptCleanup()
        {
            Scanner.Stop();
            if (Script == null)
                return;

            try
            {
                Script.RunShutdown(_state);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            finally
            {
                _settings.SetGameVersion(null);
                _settings.ResetVASLSettings();

                // Script should no longer be used, even in case of error
                // (which the VASL shutdown method may contain)
                Script = null;
            }
        }
    }
}
