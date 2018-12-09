using System;
using System.Windows.Forms;
using LiveSplit.VAS.VASL;

namespace LiveSplit.VAS.UI
{
    public partial class ComponentUI : UserControl
    {
        private VASComponent _Component { get; set; }

        // Children are hardcoded until a better solution is found.
        // tbh it's probably easier to read anyway.
        internal SettingsUI SettingsUI { get; private set; }
        internal ScanRegionUI ScanRegionUI { get; private set; }
        internal FeaturesUI FeaturesUI { get; private set; }
        internal DebugUI DebugUI { get; private set; }

        internal ComponentUI(VASComponent component)
        {
            InitializeComponent();

            _Component = component;

            SettingsUI = new SettingsUI(_Component);
            ScanRegionUI = new ScanRegionUI(_Component);
            FeaturesUI = new FeaturesUI(_Component);
            DebugUI = new DebugUI(_Component);
            SetChildControlSettings(SettingsUI, tabSettings, "Settings");
            SetChildControlSettings(ScanRegionUI, tabScanRegion, "ScanRegion");
            SetChildControlSettings(FeaturesUI, tabFeatures, "Features");
            SetChildControlSettings(DebugUI, tabDebug, "Debug");

            tabScanRegion.SuspendLayout();
            tabFeatures.SuspendLayout();
            tabDebug.SuspendLayout();
        }

        internal void InitVASLSettings(VASLSettings settings, bool scriptLoaded)
        {
            SettingsUI.InitVASLSettings(settings, scriptLoaded);
            ScanRegionUI.InitVASLSettings(settings, scriptLoaded);
            FeaturesUI.InitVASLSettings(settings, scriptLoaded);
            DebugUI.InitVASLSettings(settings, scriptLoaded);
        }

        private void SetChildControlSettings(UserControl userControl, TabPage tab, string name)
        {
            tab.Controls.Add(userControl);
            userControl.Dock = DockStyle.Fill;
            userControl.Name = name;
        }

        // Some of the interfaces are very 'active', so they should only be enabled when the user is actually using one.
        #region Renderers

        private void ComponentUI_Load(object sender, EventArgs e)
        {
            var grandParent = (TabControl)Parent.Parent;
            grandParent.Selecting += Parent_Selecting;
            grandParent.HandleDestroyed += Parent_HandleDestroyed;
            Render();
        }

        private void TabControlCore_Selecting(object sender, TabControlCancelEventArgs e) => Render();
        private void Parent_Selecting(object sender, TabControlCancelEventArgs e) => Render();
        private void Parent_HandleDestroyed(object sender, EventArgs e) => Render(true);

        private void Render(bool forceDerender = false)
        {
            RenderUI(SettingsUI, forceDerender);
            RenderUI(ScanRegionUI, forceDerender);
            RenderUI(FeaturesUI, forceDerender);
            RenderUI(DebugUI, forceDerender);
        }

        // Bad naming consistancy
        private void RenderUI(AbstractUI ui, bool forceDerender)
        {
            var grandParent = (TabControl)Parent.Parent;
            var parent = (TabPage)Parent;

            if (grandParent.SelectedTab == parent && tabControlCore.SelectedTab == ui.PageParent && !forceDerender)
            {
                ui.ResumeLayout(false);
                ui.Rerender();
            }
            else
            {
                ui.SuspendLayout();
                ui.Derender();
            }
        }

        #endregion Renderers
    }
}
