using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Accord.Video.DirectShow;
using LiveSplit.VAS.Models;
using LiveSplit.VAS.VASL;

namespace LiveSplit.VAS.UI
{
    public partial class SettingsUI : AbstractUI
    {
        internal readonly Dictionary<string, CheckBox> BasicSettings;

        private string _ProfilePath { get { return Component.ProfilePath; } set { Component.ProfilePath = value; } }
        private string _VideoDevice { get { return Component.VideoDevice; } set { Component.VideoDevice = value; } }
        private string _GameVersion { get { return Component.GameVersion; } set { Component.GameVersion = value; } }
        private IDictionary<string, bool> _BasicSettingsState => Component.BasicSettingsState;
        private IDictionary<string, dynamic> _CustomSettingsState => Component.CustomSettingsState;

        public SettingsUI(VASComponent component) : base(component)
        {
            InitializeComponent();
            UpdateCustomSettingsVisibility();

            BasicSettings = new Dictionary<string, CheckBox>
            {
                // Capitalized names for saving it in XML.
                ["Start"] = ckbStart,
                ["Split"] = ckbSplit,
                ["Reset"] = ckbReset,
            };

            Component.ProfileChanged += (o, e) => txtGameProfile.Text = _ProfilePath;
        }

        public override void Rerender() => FillboxCaptureDevice();

        public override void Derender() => boxCaptureDevice.Items.Clear();

        internal override void InitVASLSettings(VASLSettings settings, bool scriptLoaded)
        {
            treeCustomSettings.BeginUpdate();
            treeCustomSettings.Nodes.Clear();

            var flat = new Dictionary<string, DropDownTreeNode>();

            foreach (VASLSetting setting in settings.OrderedSettings)
            {
                var value = setting.Value;
                if (_CustomSettingsState.ContainsKey(setting.Id))
                {
                    value = _CustomSettingsState[setting.Id];
                }

                var node = new DropDownTreeNode(setting.Label)
                {
                    Tag = setting,
                    ToolTipText = setting.ToolTip
                };

                node.ComboBox.Text = value.ToString();
                setting.Value = value;

                if (setting.Parent == null) treeCustomSettings.Nodes.Add(node);
                else if (flat.ContainsKey(setting.Parent)) flat[setting.Parent].Nodes.Add(node);

                flat.Add(setting.Id, node);
            }

            foreach (var item in flat)
            {
                if (!item.Value.Checked) UpdateGrayedOut(item.Value);
            }

            treeCustomSettings.ExpandAll();
            treeCustomSettings.EndUpdate();

            // Scroll up to the top
            if (treeCustomSettings.Nodes.Count > 0) treeCustomSettings.Nodes[0].EnsureVisible();

            UpdateCustomSettingsVisibility();
            InitBasicSettings(settings);
        }

        internal bool FillboxCaptureDevice()
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count > 0)
            {
                boxCaptureDevice.Enabled = true;

                int selectedIndex = boxCaptureDevice.SelectedIndex;
                boxCaptureDevice.Items.Clear();

                if (!string.IsNullOrEmpty(_VideoDevice))
                {
                    var savedDevices = videoDevices.Where(d => d.ToString() == _VideoDevice);
                    if (savedDevices.Any())
                    {
                        var savedDevice = savedDevices.First();
                        selectedIndex = videoDevices.IndexOf(savedDevice);
                    }
                }

                for (var i = 0; i < videoDevices.Count; i++)
                {
                    boxCaptureDevice.Items.Add(videoDevices[i]);
                }

                //if (boxCaptureDevice.SelectedIndex != selectedIndex)
                boxCaptureDevice.SelectedIndex = selectedIndex;
                return true;
            }
            else
            {
                boxCaptureDevice.Enabled = false;
                boxCaptureDevice.Items.Clear();
                return false;
            }
        }

        private void UpdateCustomSettingsVisibility()
        {
            bool show = treeCustomSettings.GetNodeCount(false) > 0;
            treeCustomSettings.Visible = show;
            btnResetToDefault.Visible = show;
            btnCheckAll.Visible = show;
            btnUncheckAll.Visible = show;
            lblAdvanced.Visible = show;
        }

        private void UpdateNodesInTree(Func<DropDownTreeNode, bool> func, TreeNodeCollection nodes)
        {
            foreach (DropDownTreeNode node in nodes)
            {
                bool include_child_nodes = func(node);
                if (include_child_nodes)
                {
                    UpdateNodesInTree(func, node.Nodes);
                }
            }
        }

        internal void UpdateNodesValues(Func<VASLSetting, dynamic> func, TreeNodeCollection nodes = null)
        {
            if (nodes == null)
            {
                nodes = treeCustomSettings.Nodes;
            }

            UpdateNodesInTree(node =>
            {
                var setting = (VASLSetting)node.Tag;
                dynamic value = func(setting);

                if (node.ComboBox.Text != value.ToString())
                {
                    node.ComboBox.Text = value.ToString();
                }

                return true;
            }, nodes);
        }

        internal void UpdateNodesValues(IDictionary<string, dynamic> settingValues, TreeNodeCollection nodes = null)
        {
            if (settingValues == null)
            {
                return;
            }

            UpdateNodesValues(setting =>
            {
                string id = setting.Id;

                if (settingValues.ContainsKey(id))
                {
                    return settingValues[id];
                }

                return setting.Value;
            }, nodes);
        }

        internal void UpdateNodeValue(Func<VASLSetting, dynamic> func, DropDownTreeNode node)
        {
            var setting = (VASLSetting)node.Tag;
            dynamic value = func(setting);

            if (node.ComboBox.Text != value.ToString()) node.ComboBox.Text = value.ToString();
        }

        private void UpdateGrayedOut(DropDownTreeNode node)
        {
            if (node.ForeColor != SystemColors.GrayText)
            {
                UpdateNodesInTree(n =>
                {
                    n.ForeColor = node.Checked ? SystemColors.WindowText : SystemColors.GrayText;
                    return n.Checked || !node.Checked;
                }, node.Nodes);
            }
        }

        private void InitBasicSettings(VASLSettings settings)
        {
            foreach (var item in BasicSettings)
            {
                string name = item.Key.ToLower();
                CheckBox checkbox = item.Value;

                if (settings.IsBasicSettingPresent(name))
                {
                    VASLSetting setting = settings.BasicSettings[name];
                    checkbox.Enabled = true;
                    checkbox.Tag = setting;
                    var value = setting.Value;

                    if (_BasicSettingsState.ContainsKey(name))
                    {
                        value = _BasicSettingsState[name];
                    }

                    checkbox.Checked = value;
                    setting.Value = value;
                }
                else
                {
                    checkbox.Tag = null;
                    checkbox.Enabled = false;
                    checkbox.Checked = false;
                }
            }
        }

        private void BtnGameProfile_Click(object sender, EventArgs e)
        {
            if ((ModifierKeys & Keys.Shift) != 0)
            {
                SetGameProfileWithFolder();
            }
            else
            {
                SetGameProfileWithFile();
            }
        }

        private void SetGameProfileWithFile()
        {
            using (var ofd = new OpenFileDialog()
            {
                Filter = "VASL files (*.vasl)|*.vasl|ZIP files|*.zip|All files (*.*)|*.*",
                Title = "Load a Game Profile"
            })
            {
                if (File.Exists(_ProfilePath))
                {
                    ofd.InitialDirectory = Path.GetDirectoryName(_ProfilePath);
                    ofd.FileName = Path.GetFileName(_ProfilePath);
                }
                else if (Directory.Exists(_ProfilePath))
                {
                    ofd.InitialDirectory = Path.GetDirectoryName(_ProfilePath);
                }

                if (ofd.ShowDialog() == DialogResult.OK && ofd.CheckFileExists)
                {
                    TryLoadGameProfile(ofd.FileName);
                }
            }
        }

        private void SetGameProfileWithFolder()
        {
            using (var fbd = new FolderBrowserDialog() { ShowNewFolderButton = false })
            {
                if (Directory.Exists(_ProfilePath))
                {
                    fbd.SelectedPath = _ProfilePath;
                }
                else if (File.Exists(_ProfilePath))
                {
                    fbd.SelectedPath = Path.GetDirectoryName(_ProfilePath);
                }

                if (fbd.ShowDialog() == DialogResult.OK && Directory.Exists(fbd.SelectedPath))
                {
                    TryLoadGameProfile(fbd.SelectedPath);
                }
            }
        }

        // @TODO: https://imgs.xkcd.com/comics/goto.png
        private void TryLoadGameProfile(string filePath)
        {
retry:
            var gp = GameProfile.FromPath(filePath);

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
                _ProfilePath = txtGameProfile.Text = filePath;
                //Component.UpdateScript(null, null);
            }
        }

        // Basic Setting checked/unchecked
        //
        // This detects both changes made by the user and by the program, so this should
        // change the state in _basic_settings_state fine as well.
        private void MethodCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            var checkbox = (CheckBox)sender;
            var setting = (VASLSetting)checkbox.Tag;

            if (setting != null)
            {
                _BasicSettingsState[setting.Id] = setting.Value = checkbox.Checked;
            }
        }

        // Custom Setting checked/unchecked (only after initially building the tree)
        private void SettingsTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // Update value in the ASLSetting object, which also changes it in the ASL script
            var node = (DropDownTreeNode)e.Node;
            VASLSetting setting = (VASLSetting)node.Tag;
            setting.Value = node.Checked;
            _CustomSettingsState[setting.Id] = setting.Value;

            UpdateGrayedOut(node);
        }

        private void SettingsTree_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = e.Node.ForeColor == SystemColors.GrayText;
        }

        // Custom Settings Button Events

        private void BtnCheckAll_Click(object sender, EventArgs e)
        {
            UpdateNodesValues(_ => true);
        }

        private void BtnUncheckAll_Click(object sender, EventArgs e)
        {
            UpdateNodesValues(_ => false);
        }

        private void BtnResetToDefault_Click(object sender, EventArgs e)
        {
            UpdateNodesValues(s => s.DefaultValue);
        }

        // Custom Settings Context Menu Events

        private void SettingsTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Select clicked node (not only with left-click) for use with context menu
            treeCustomSettings.SelectedNode = e.Node;
        }

        private void CmiCheckBranch_Click(object sender, EventArgs e)
        {
            UpdateNodesValues(_ => true, treeCustomSettings.SelectedNode.Nodes);
            UpdateNodeValue(_ => true, (DropDownTreeNode)treeCustomSettings.SelectedNode);
        }

        private void CmiUncheckBranch_Click(object sender, EventArgs e)
        {
            UpdateNodesValues(_ => false, treeCustomSettings.SelectedNode.Nodes);
            UpdateNodeValue(_ => false, (DropDownTreeNode)treeCustomSettings.SelectedNode);
        }

        private void CmiResetBranchToDefault_Click(object sender, EventArgs e)
        {
            UpdateNodesValues(s => s.DefaultValue, treeCustomSettings.SelectedNode.Nodes);
            UpdateNodeValue(s => s.DefaultValue, (DropDownTreeNode)treeCustomSettings.SelectedNode);
        }

        private void CmiExpandBranch_Click(object sender, EventArgs e)
        {
            treeCustomSettings.SelectedNode.ExpandAll();
            treeCustomSettings.SelectedNode.EnsureVisible();
        }

        private void CmiCollapseBranch_Click(object sender, EventArgs e)
        {
            treeCustomSettings.SelectedNode.Collapse();
            treeCustomSettings.SelectedNode.EnsureVisible();
        }

        private void CmiCollapseTreeToSelection_Click(object sender, EventArgs e)
        {
            TreeNode selected = treeCustomSettings.SelectedNode;
            treeCustomSettings.CollapseAll();
            treeCustomSettings.SelectedNode = selected;
            selected.EnsureVisible();
        }

        private void CmiExpandTree_Click(object sender, EventArgs e)
        {
            treeCustomSettings.ExpandAll();
            treeCustomSettings.SelectedNode.EnsureVisible();
        }

        private void CmiCollapseTree_Click(object sender, EventArgs e)
        {
            treeCustomSettings.CollapseAll();
        }

        private void CmiResetSettingToDefault_Click(object sender, EventArgs e)
        {
            UpdateNodeValue(s => s.DefaultValue, (DropDownTreeNode)treeCustomSettings.SelectedNode);
        }

        // @TODO: https://imgs.xkcd.com/comics/goto.png
        private void BtnCaptureDevice_Click(object sender, EventArgs e)
        {
retry:
            var success = FillboxCaptureDevice();
            if (!success)
            {
                DialogResult dr = MessageBox.Show(
                    "No video capture devices detected.",
                    "Information",
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Information
                    );

                if (dr == DialogResult.Retry)
                {
                    goto retry;
                }
            }
        }

        // @TODO: https://imgs.xkcd.com/comics/goto.png
        private void BoxCaptureDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            // May need to tweak more
            if (boxCaptureDevice.SelectedItem.ToString() == _VideoDevice)
            {
                return;
            }

            if (Component.Scanner.IsVideoSourceRunning())
            {
                Component.Scanner.Stop();
            }

retry:
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            var matches = videoDevices.Where(v => v.ToString() == boxCaptureDevice.Text);
            if (matches.Any())
            {
                var match = matches.First();
                //Component.Scanner.SetVideoSource(match.MonikerString);
                _VideoDevice = match.ToString();
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
                    FillboxCaptureDevice();
                }
            }
            Component.Scanner.AsyncStart();
        }

        private void SettingsUI_VisibleChanged(object sender, EventArgs e)
        {
            Rerender();
        }
    }
}
