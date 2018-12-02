using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LiveSplit.VAS.VASL;

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

        // This could be better...
        public void SetChildControlSettings(UserControl userControl, TabPage tab, string name)
        {
            tab.Controls.Add(userControl);
            userControl.Dock = DockStyle.Fill;
        }

        private void tabControlCore_Selecting(object sender, TabControlCancelEventArgs e)
        {
            tabScanRegion.SuspendLayout();
            ScanRegionUI.Unrender();
            tabFeatures.SuspendLayout();
            tabDebug.SuspendLayout();
            switch (e.TabPage.Name)
            {
                case "tabScanRegion":
                    tabScanRegion.ResumeLayout(false);
                    ScanRegionUI.Rerender();
                    break;
                case "tabFeatures":
                    tabFeatures.ResumeLayout(false);
                    break;
                case "tabDebug":
                    tabDebug.ResumeLayout(false);
                    break;
                default:
                    break;
            }
        }

        internal void InitVASLSettings(VASLSettings settings, bool scriptLoaded)
        {
            SettingsUI.InitVASLSettings(settings, scriptLoaded);
        }

    }
}
