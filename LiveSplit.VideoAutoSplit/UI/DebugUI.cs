using System;
using System.IO;
using System.Windows.Forms;
using LiveSplit.VAS.VASL;

namespace LiveSplit.VAS.UI
{
    public partial class DebugUI : AbstractUI
    {
        public DebugUI(VASComponent component) : base(component)
        {
            InitializeComponent();
        }

        public override void Rerender()
        {
            txtDebug.Text = Component.EventLog;

            Component.EventLogUpdated += UpdatetxtDebug;
        }

        public override void Derender()
        {
            Component.EventLogUpdated -= UpdatetxtDebug;
        }

        private void UpdatetxtDebug(object sender, string str)
        {
            txtDebug.Invoke((MethodInvoker)delegate
            {
                txtDebug.Text += str;
            });
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtDebug.Clear();
            Component.ClearEventLog();
        }

        private void BtnExport_Click(object sender, EventArgs e)
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
                    var finalLog = Component.EventLog + "\r\nLog saved at " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";
                    File.WriteAllText(ofd.FileName, finalLog);
                }
            }
        }

        // This form contains no settings.
        internal override void InitVASLSettings(VASLSettings settings, bool scriptLoaded) { }
    }
}
