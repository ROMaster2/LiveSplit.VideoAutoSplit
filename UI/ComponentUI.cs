using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LiveSplit.VAS.VASL;
using LiveSplit;

namespace LiveSplit.UI.Components
{
    public partial class ComponentUI : UserControl
    {
        private readonly VASComponent ParentComponent;

        internal SettingsUI   SettingsUI;
        internal ScanRegionUI ScanRegionUI;
        internal FeaturesUI   FeaturesUI;
        internal DebugUI      DebugUI;

        private string ProfilePath => ParentComponent.ProfilePath;
        private string VideoDevice => ParentComponent.VideoDevice;
        private string GameVersion => ParentComponent.GameVersion;
        private IDictionary<string, bool> BasicSettingsState => ParentComponent.BasicSettingsState;
        private IDictionary<string, dynamic> CustomSettingsState => ParentComponent.CustomSettingsState;

        public ComponentUI(VASComponent parentComponent)
        {
            InitializeComponent();

            ParentComponent = parentComponent;

            SettingsUI   = new SettingsUI(ParentComponent);
            ScanRegionUI = new ScanRegionUI(ParentComponent);
            FeaturesUI   = new FeaturesUI(ParentComponent);
            DebugUI      = new DebugUI(ParentComponent);
            SetChildControlSettings(SettingsUI, tabSettings, "Settings");
            SetChildControlSettings(ScanRegionUI, tabScanRegion, "ScanRegion");
            SetChildControlSettings(FeaturesUI, tabFeatures, "Features");
            SetChildControlSettings(DebugUI, tabDebug, "Debug");

            tabScanRegion.SuspendLayout();
            tabFeatures.SuspendLayout();
            tabDebug.SuspendLayout();
        }

        public void SetChildControlSettings(UserControl userControl, TabPage tab, string name)
        {
            tab.Controls.Add(userControl);
            userControl.Dock = DockStyle.Fill;
            userControl.Name = name;
        }

        internal void InitVASLSettings(VASLSettings settings, bool scriptLoaded)
        {
            SettingsUI.InitVASLSettings(settings, scriptLoaded);
            ScanRegionUI.InitVASLSettings(settings, scriptLoaded);
            FeaturesUI.InitVASLSettings(settings, scriptLoaded);
            DebugUI.InitVASLSettings(settings, scriptLoaded);
        }

        private void ComponentUI_Load(object sender, EventArgs e)
        {
            var grandParent = (TabControl)this.Parent.Parent;
            grandParent.Selecting += Parent_Selecting;
            grandParent.HandleDestroyed += Parent_HandleDestroyed;
            Render();
        }

        private void tabControlCore_Selecting(object sender, TabControlCancelEventArgs e) => Render();
        private void Parent_Selecting(object sender, TabControlCancelEventArgs e) => Render();
        private void Parent_HandleDestroyed(object sender, EventArgs e) => Render(true);

        private void Render(bool forceDerender = false)
        {
            RenderUI(SettingsUI, forceDerender);
            RenderUI(ScanRegionUI, forceDerender);
            RenderUI(FeaturesUI, forceDerender);
            RenderUI(DebugUI, forceDerender);
        }

        private void RenderUI(AbstractUI ui, bool forceDerender)
        {
            var grandParent = (TabControl)this.Parent.Parent;
            var parent = (TabPage)this.Parent;
            if (grandParent.SelectedTab == parent &&
                tabControlCore.SelectedTab == ui.PageParent &&
                !forceDerender)
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

    }
}
