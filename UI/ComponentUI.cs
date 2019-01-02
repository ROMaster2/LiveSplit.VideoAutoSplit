using System;
using System.Windows.Forms;
using LiveSplit.VAS.VASL;

namespace LiveSplit.VAS.UI
{
    public partial class ComponentUI : UserControl
    {
        private VASComponent Component { get; }

        // Children are hardcoded until a better solution is found.
        // tbh it's probably easier to read anyway.
        internal SettingsUI   SettingsUI   { get; }
        internal ScanRegionUI ScanRegionUI { get; }
        internal FeaturesUI   FeaturesUI   { get; }
        internal DebugUI      DebugUI      { get; }

        public MainVASSettings SettingsWindow;

        public ComponentUI(VASComponent component)
        {
            InitializeComponent();

            Component = component;

            // The tabs are still created in this window so that the entire codebase doesn't have to be rewritten
            SettingsUI   = new SettingsUI(Component);
            ScanRegionUI = new ScanRegionUI(Component);
            FeaturesUI   = new FeaturesUI(Component);
            DebugUI      = new DebugUI(Component);

            SettingsWindow = new MainVASSettings();

            SettingsWindow.AddTab(SettingsUI, SettingsWindow.tabSettings, "Settings");
            SettingsWindow.AddTab(ScanRegionUI, SettingsWindow.tabScanRegion, "ScanRegion");
            SettingsWindow.AddTab(FeaturesUI, SettingsWindow.tabFeatures, "Features");
            SettingsWindow.AddTab(DebugUI, SettingsWindow.tabDebug, "Debug");

            this.Dock = DockStyle.Fill;
        }

        internal void InitVASLSettings(VASLSettings settings, bool scriptLoaded)
        {
            SettingsUI.InitVASLSettings(settings, scriptLoaded);
            ScanRegionUI.InitVASLSettings(settings, scriptLoaded);
            FeaturesUI.InitVASLSettings(settings, scriptLoaded);
            DebugUI.InitVASLSettings(settings, scriptLoaded);
        }

        

        // Some of the interfaces are very 'active', so they should only be enabled when the user is actually using one.
        #region Renderers

        private void ComponentUI_Load(object sender, EventArgs e)
        {
            var mainSettings = SettingsWindow.tabControlCore;
            mainSettings.Selecting += Parent_Selecting;
            mainSettings.HandleDestroyed += Parent_HandleDestroyed;
            Render();
        }

        private void tabControlCore_Selecting(object sender, TabControlCancelEventArgs e) => Render();
        private void Parent_Selecting(object sender, TabControlCancelEventArgs e) => Render();
        private void Parent_HandleDestroyed(object sender, EventArgs e) => Render(true);

        private void Render(bool forceDerender = false)
        {
            if (SettingsWindow.Visible)
            {
                RenderUI(SettingsUI, forceDerender);
                RenderUI(ScanRegionUI, forceDerender);
                RenderUI(FeaturesUI, forceDerender);
                RenderUI(DebugUI, forceDerender);
            }
        }

        private void RenderUI(AbstractUI ui, bool forceDerender)
        {
            if (SettingsWindow.tabControlCore.SelectedTab == ui.Parent && !forceDerender)
            {
                ui.ResumeLayout(false);
                ui.Rerender();
            }
            else
            {
                ui.Derender();
                ui.SuspendLayout();
            }
        }

        #endregion Renderers

        private void btnSettings_Click(object sender, EventArgs e)
        {
            SettingsWindow.Show();
        }
    }
}
