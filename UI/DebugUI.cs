using System;
using System.IO;
using System.Windows.Forms;
using LiveSplit.VAS.VASL;

namespace LiveSplit.UI.Components
{
    public partial class DebugUI : AbstractUI
    {
        private readonly VASComponent ParentComponent;

        public DebugUI(VASComponent parentComponent)
        {
            InitializeComponent();

            ParentComponent = parentComponent;

            txtDebug.Text = ParentComponent.EventLog;

            ParentComponent.EventLogUpdated += (sender, str) => txtDebug.Text += str;
        }

        override public void Rerender() { }
        override public void Derender() { }
        override internal void InitVASLSettings(VASLSettings s, bool l) { }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtDebug.Clear();
            ParentComponent.ClearEventLog();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            using (var ofd = new SaveFileDialog()
            {
                AddExtension = true,
                DefaultExt = ".log",
                FileName = "VASEvents",
                Filter = "Log files (*.log)|*.log|All files (*.*)|*.*",
                Title = "Save VAS event log...",
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(ofd.FileName))
                {
                    File.WriteAllText(ofd.FileName, txtDebug.Text);
                }
            }
        }
    }
}
