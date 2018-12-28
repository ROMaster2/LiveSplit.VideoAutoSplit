using System;
using System.IO;
using System.Windows.Forms;
using LiveSplit.VAS.VASL;

namespace LiveSplit.VAS.UI
{
    public partial class DebugUI : AbstractUI
    {
        private const int UPDATE_RATE = 500; // Milliseconds

        private TextWriter _TextWriter;
        private Timer _UpdateTimer;

        public DebugUI(VASComponent component) : base(component)
        {
            InitializeComponent();
            _TextWriter = new StringWriter();

            _UpdateTimer = new Timer() { Interval = UPDATE_RATE };
            _UpdateTimer.Tick += (sender, args) => UpdatetxtDebug(sender, args);
            _UpdateTimer.Enabled = false;
        }

        override public void Rerender()
        {
            txtDebug.Text = Log.ReadAll();
            Log.LogUpdated += UpdateTextWriter;
            _UpdateTimer.Enabled = true;
        }

        override public void Derender()
        {
            Log.LogUpdated -= UpdateTextWriter;
            _UpdateTimer.Enabled = false;
        }

        private void UpdateTextWriter(object sender, string str)
        {
            _TextWriter.WriteLineAsync(str);
        }

        private void UpdatetxtDebug(object sender, EventArgs e)
        {
            var str = _TextWriter.ToString();

            if (str.Length > 5)
            {
                _TextWriter.Flush();
                txtDebug.AppendText(str);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtDebug.Clear();
            _TextWriter.Flush();
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
