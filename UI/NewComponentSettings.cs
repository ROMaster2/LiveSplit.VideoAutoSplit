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
using LiveSplit.Model;
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

        public NewComponentSettings(VASComponent parentComponent)
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
            userControl.Location = new Point(0, 0);
            userControl.Margin = new Padding(0);
            userControl.Name = name;
            userControl.Padding = new Padding(7);
            userControl.Size = new System.Drawing.Size(468, 506);
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
