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
using LiveSplit.VAS.UI;
using LiveSplit.VAS.Models;
using System.IO.Compression;
using System.Linq;

namespace LiveSplit.UI.Components
{
    public class VASComponent : LogicComponent
    {
        public override string ComponentName => "Video Auto Splitter";

        public VASLScript Script { get; private set; }

        public event EventHandler ProfileChanged;

        private bool doReload;
        private string oldProfilePath;

        private FileSystemWatcher fsWatcher;

        private NewComponentSettings componentSettings;

        private LiveSplitState state;

        private string profilePath
        {
            get
            {
                return componentSettings?.ProfilePath;
            }
        }

        public VASComponent(LiveSplitState state)
        {
            this.state = state;

            componentSettings = new NewComponentSettings();

            fsWatcher = new FileSystemWatcher();
            fsWatcher.Changed += async (sender, args) => {
                await Task.Delay(200);
                doReload = true;
            };

            Scanner.NewResult += (sender, dm) => UpdateProfile(sender, dm);
        }

        public override void Dispose()
        {
            Scanner.Stop();
            ProfileCleanup();

            try
            {
                ProfileChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            fsWatcher?.Dispose();
        }

        public override Control GetSettingsControl(LayoutMode mode)
        {
            return componentSettings;
        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            return componentSettings.GetSettings(document);
        }

        public override void SetSettings(XmlNode settings)
        {
            componentSettings.SetSettings(settings);
            UpdateProfile(null, null);
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height,
            LayoutMode mode)
        { }

        internal void UpdateProfile(object sender, DeltaManager dm)
        {
            if (profilePath != oldProfilePath || doReload)
            {
                try
                {
                    doReload = false;
                    oldProfilePath = profilePath;

                    ProfileCleanup();

                    if (!string.IsNullOrEmpty(profilePath))
                    {
                        LoadProfile();
                    }

                    ProfileChanged?.Invoke(this, EventArgs.Empty);
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
                    Script.Update(state, dm);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

        private void LoadProfile()
        {
            try
            {
                Log.Info("[VASL] Loading new profile: " + profilePath);

                Scanner.GameProfile = GameProfile.FromPath(profilePath);

                if (File.Exists(profilePath))
                {
                    fsWatcher.Path = Path.GetDirectoryName(profilePath);
                    fsWatcher.Filter = Path.GetFileName(profilePath + "*");
                }
                else
                {
                    fsWatcher.Path = profilePath;
                    fsWatcher.Filter = null;
                }

                fsWatcher.EnableRaisingEvents = true;

                Log.Info("[VASL] Loading script within profile.");

                var script = Scanner.GameProfile.RawScript;

                // New script
                Script = VASLParser.Parse(script);

                Script.GameVersionChanged += (sender, version) => componentSettings.SetGameVersion(version);
                componentSettings.SetGameVersion(null);
            }
            catch (Exception ex)
            {
                // Todo: Update UI if Game Profile failed to load.
                Log.Error(ex);
                ProfileCleanup();
            }

            // Give custom VASL settings to GUI, which populates the list and
            // stores the VASLSetting objects which are shared between the GUI
            // and VASLScript
            try
            {
                VASLSettings settings = Script.RunStartup(state);
                componentSettings.SetVASLSettings(settings);
                Scanner.AsyncStart();
            }
            catch (Exception ex)
            {
                // Script already created, but startup failed, so clean up again
                Log.Error(ex);
                ProfileCleanup();
            }
        }

        private void ProfileCleanup()
        {
            Scanner.Stop();
            if (Script == null)
                return;

            try
            {
                Script.RunShutdown(state);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            finally
            {
                componentSettings.SetGameVersion(null);
                componentSettings.ResetVASLSettings();

                // Script should no longer be used, even in case of error
                // (which the VASL shutdown method may contain)
                Script = null;
            }
        }
    }
}
