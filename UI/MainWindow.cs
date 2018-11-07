using Accord.Video;
using Accord.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveSplit.VFM.Models;

namespace LiveSplit.VFM.Forms
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            FillBoxCaptureDevice();
            /*if (BoxCaptureDevice.Items.Contains(Properties.Settings.Default.VideoDevice))
            {
                var idx = BoxCaptureDevice.Items.IndexOf(Properties.Settings.Default.VideoDevice);
                BoxCaptureDevice.SelectedIndex = idx;
            }
            var savedProfile = Properties.Settings.Default.GameProfile;
            if (File.Exists(savedProfile))
            {
                try
                {
                    Scanner.GameProfile = GameProfile.FromZip(savedProfile);
                    txtGameProfile.Text = savedProfile;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Game Profile failed to load.");
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                }
                finally
                {
                    if (Scanner.GameProfile == null) txtGameProfile.Text = null;
                }
            }
            TryStart();

            Scanner.CropGeometry = new Geometry(
               Properties.Settings.Default.CropX,
               Properties.Settings.Default.CropY,
               Properties.Settings.Default.CropWidth,
               Properties.Settings.Default.CropHeight);*/
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Scanner.Stop();
            Application.Exit();
        }

        private void BtnGameProfile_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog() { Filter = "Zip Files|*.zip", Title = "Load a Game Profile" })
            {
                if (ofd.ShowDialog() == DialogResult.OK && ofd.CheckFileExists == true)
                {
                retry:
                    var gp = GameProfile.FromZip(ofd.FileName);

                    if (gp == null)
                    {
                        DialogResult dr = MessageBox.Show(
                            "Failed to load Game Profile.",
                            "Error",
                            MessageBoxButtons.RetryCancel,
                            MessageBoxIcon.Error
                            );

                        if (dr == DialogResult.Retry)
                        {
                            goto retry;
                        }
                    }
                    else
                    {
                        Scanner.GameProfile = gp;
                        txtGameProfile.Text = ofd.FileName;
                        TryStart();
                        //Properties.Settings.Default.GameProfile = ofd.FileName;
                        //Properties.Settings.Default.Save();
                    }
                }
            }
        }

        private void BtnCaptureDevice_Click(object sender, EventArgs e)
        {
        retry:
            var success = FillBoxCaptureDevice();
            if (!success)
            {
                DialogResult dr = MessageBox.Show(
                    "No video capture devices detected.",
                    "Error",
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Error
                    );

                if (dr == DialogResult.Retry)
                {
                    goto retry;
                }
            }
        }

        private bool FillBoxCaptureDevice()
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count > 0)
            {
                BoxCaptureDevice.Enabled = true;
                string selectedItem = null;
                int selectedIndex = 0;
                if (BoxCaptureDevice.SelectedIndex > -1)
                {
                    selectedItem = (string)BoxCaptureDevice.SelectedItem;
                }

                BoxCaptureDevice.Items.Clear();
                for (var i = 0; i < videoDevices.Count; i++)
                {
                    BoxCaptureDevice.Items.Add(videoDevices[i].Name);
                    if (videoDevices[i].Name == selectedItem)
                    {
                        selectedIndex = i;
                    }
                }
                BoxCaptureDevice.SelectedIndex = selectedIndex;
                return true;
            }
            else
            {
                BoxCaptureDevice.Items.Clear();
                BoxCaptureDevice.Enabled = false;
                return false;
            }
        }

        private void BoxCaptureDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            Scanner.Stop();
        retry:
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            var matches = videoDevices.Where(v => v.Name == BoxCaptureDevice.Text);
            if (matches.Count() > 0)
            {
                var match = matches.First();
                Scanner.SetVideoSource(match.MonikerString);
                lblCaptureDevice.Text = "Capture Device - " + Scanner.VideoGeometry.ToString();
                //Properties.Settings.Default.VideoDevice = BoxCaptureDevice.Text;
                //Properties.Settings.Default.Save();
            }
            else
            {
                lblCaptureDevice.Text = "Capture Device";
                DialogResult dr = MessageBox.Show(
                    "Selected video capture device cannont be found. Has it been unplugged?",
                    "Error",
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Error
                    );

                if (dr == DialogResult.Retry)
                {
                    goto retry;
                }
                else
                {
                    FillBoxCaptureDevice();
                }
            }
            TryStart();
        }

        private void TryStart()
        {
            if (Scanner.GameProfile != null && Scanner.IsVideoSourceValid())
            {
                Scanner.Start();
            }
        }

        private void BtnSetCaptureRegion_Click(object sender, EventArgs e)
        {
            Aligner w = new Aligner();
            w.ShowDialog();
        }

    }
}
