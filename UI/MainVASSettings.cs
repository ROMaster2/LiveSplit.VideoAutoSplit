using System;
using System.Windows.Forms;
using LiveSplit.VAS.VASL;

namespace LiveSplit.VAS.UI
{
    public partial class MainVASSettings : Form
    {
        private VASComponent Component { get; }

        public MainVASSettings()
        {
            InitializeComponent();
        }

        public void AddTab(UserControl userControl, TabPage tab, string name)
        {
            tab.Controls.Add(userControl);
            userControl.Dock = DockStyle.Fill;
            userControl.Name = name;
        }

    }
}
