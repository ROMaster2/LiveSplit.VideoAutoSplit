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

        override public void Rerender()
        {
            txtDebug.Text = Log.ReadAll();

            Log.LogUpdated += UpdatetxtDebug;
        }

        override public void Derender()
        {
            Log.LogUpdated -= UpdatetxtDebug;
        }

        private void UpdatetxtDebug(object sender, string str)
        {
            txtDebug.Invoke((MethodInvoker)delegate
            {
                txtDebug.Text += str;
                txtDebug.ScrollToCaret();
            });
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtDebug.Clear();
            Log.Flush();
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
                    var finalLog = Log.ReadAll() + "\r\nLog saved at " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";
                    File.WriteAllText(ofd.FileName, finalLog);
                }
            }
        }

        // This form contains no settings.
        override internal void InitVASLSettings(VASLSettings s, bool l) { }
    }
}
