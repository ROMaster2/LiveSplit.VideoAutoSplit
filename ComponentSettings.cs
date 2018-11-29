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
using LiveSplit.UI;
using LiveSplit.UI.Components;
using LiveSplit.VAS.Models;
using LiveSplit.VAS.UI;
using LiveSplit.VAS.VASL;

namespace LiveSplit.VAS
{
    public partial class ComponentSettings : UserControl
    {
        public string ScriptPath { get; set; }

        // if true, next path loaded from settings will be ignored
        private bool _ignore_next_path_setting;

        private Dictionary<string, CheckBox> _basic_settings;

        // Save the state of settings independant of actual VASLSetting objects
        // or the actual GUI components (checkboxes). This is used to restore
        // the state when the script is first loaded (because settings are
        // loaded before the script) or reloaded.
        //
        // State is synchronized with the VASLSettings when a script is
        // successfully loaded, as well as when the checkboxes/tree check
        // state is changed by the user or program. It is also updated
        // when loaded from XML.
        //
        // State is stored from the current script, or the last loaded script
        // if no script is currently loaded.

        // Start/Reset/Split checkboxes
        private Dictionary<string, bool> _basic_settings_state;

        // Custom settings
        private Dictionary<string, bool> _custom_settings_state;

        private readonly VASComponent Component;

        public ComponentSettings(VASComponent component)
        {
            Component = component;
            InitializeComponent();
            FillboxCaptureDevice();

            ScriptPath = string.Empty;

            this.txtGameProfile.DataBindings.Add("Text", this, "ScriptPath", false,
                DataSourceUpdateMode.OnPropertyChanged);

            SetGameVersion(null);
            UpdateCustomSettingsVisibility();
            
            _basic_settings = new Dictionary<string, CheckBox>
            {
                // Capitalized names for saving it in XML.
                ["Start"] = checkboxStart,
                ["Reset"] = checkboxReset,
                ["Split"] = checkboxSplit
            };
            
            _basic_settings_state = new Dictionary<string, bool>();
            _custom_settings_state = new Dictionary<string, bool>();
        }

        public ComponentSettings(VASComponent component, string scriptPath)
            : this(component)
        {
            ScriptPath = scriptPath;
            _ignore_next_path_setting = true;
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            XmlElement settings_node = document.CreateElement("Settings");

            settings_node.AppendChild(SettingsHelper.ToElement(document, "Version",    "0.1"));
            settings_node.AppendChild(SettingsHelper.ToElement(document, "ScriptPath", ScriptPath));
            // Todo: Handle when Scanner.CropGeometry == Scanner.VideoGeometry.
            settings_node.AppendChild(SettingsHelper.ToElement(document, "CropX",      Scanner.CropGeometry.X));
            settings_node.AppendChild(SettingsHelper.ToElement(document, "CropY",      Scanner.CropGeometry.Y));
            settings_node.AppendChild(SettingsHelper.ToElement(document, "CropWidth",  Scanner.CropGeometry.Width));
            settings_node.AppendChild(SettingsHelper.ToElement(document, "CropHeight", Scanner.CropGeometry.Height));
            AppendBasicSettingsToXml(document, settings_node);
            AppendCustomSettingsToXml(document, settings_node);

            return settings_node;
        }

        // Loads the settings of this component from Xml. This might happen more than once
        // (e.g. when the settings dialog is cancelled, to restore previous settings).
        public void SetSettings(XmlNode settings)
        {
            var element = (XmlElement)settings;
            if (!element.IsEmpty)
            {
                if (!_ignore_next_path_setting)
                    ScriptPath = SettingsHelper.ParseString(element["ScriptPath"], string.Empty);
                _ignore_next_path_setting = false;

                var geo = new Geometry()
                {
                    X =      SettingsHelper.ParseDouble(element["CropX"], 0),
                    Y =      SettingsHelper.ParseDouble(element["CropY"], 0),
                    Width =  SettingsHelper.ParseDouble(element["CropWidth"], 0),
                    Height = SettingsHelper.ParseDouble(element["CropHeight"], 0)
                };

                ParseBasicSettingsFromXml(element);
                ParseCustomSettingsFromXml(element);

                try
                {
                    Scanner.CropGeometry = geo;
                    Scanner.GameProfile = GameProfile.FromPath(ScriptPath);
                }
                catch (Exception e)
                {
                    LiveSplit.Options.Log.Error(e); // Change, probably
                }
            }
        }

        public void SetGameVersion(string version)
        {
            //this.lblGameVersion.Text = string.IsNullOrEmpty(version) ? "" : "Game Version: " + version;
        }

        /// <summary>
        /// Populates the component with the settings defined in the VASL script.
        /// </summary>
        public void SetVASLSettings(VASLSettings settings)
        {
            InitVASLSettings(settings, true);
        }

        /// <summary>
        /// Empties the GUI of all settings (but still keeps settings state
        /// for the next script load).
        /// </summary>
        public void ResetVASLSettings()
        {
            if (IsHandleCreated)
            {
                BeginInvoke((MethodInvoker)delegate ()
                {
                    InitVASLSettings(new VASLSettings(), false);
                });
            }
        }

        // Todo: Later
        private void InitVASLSettings(VASLSettings settings, bool script_loaded)
        {
            if (string.IsNullOrWhiteSpace(ScriptPath))
            {
                _basic_settings_state.Clear();
                _custom_settings_state.Clear();
            }

            UpdateCustomSettingsVisibility();
            InitBasicSettings(settings);
        }


        private void AppendBasicSettingsToXml(XmlDocument document, XmlNode settings_node)
        {
            if (_basic_settings != null && _basic_settings.Count > 0)
            foreach (var item in _basic_settings)
            {
                if (_basic_settings_state.ContainsKey(item.Key.ToLower()))
                {
                    var value = _basic_settings_state[item.Key.ToLower()];
                    settings_node.AppendChild(SettingsHelper.ToElement(document, item.Key, value));
                }
            }
        }

        private void AppendCustomSettingsToXml(XmlDocument document, XmlNode parent)
        {
            XmlElement VASL_parent = document.CreateElement("CustomSettings");

            foreach (var setting in _custom_settings_state)
            {
                XmlElement element = SettingsHelper.ToElement(document, "Setting", setting.Value);
                XmlAttribute id = SettingsHelper.ToAttribute(document, "id", setting.Key);
                // In case there are other setting types in the future
                XmlAttribute type = SettingsHelper.ToAttribute(document, "type", "bool");

                element.Attributes.Append(id);
                element.Attributes.Append(type);
                VASL_parent.AppendChild(element);
            }

            parent.AppendChild(VASL_parent);
        }

        private void ParseBasicSettingsFromXml(XmlElement element)
        {
            foreach (var item in _basic_settings)
            {
                if (element[item.Key] != null)
                {
                    var value = bool.Parse(element[item.Key].InnerText);

                    // If component is not enabled, don't check setting
                    if (item.Value.Enabled)
                        item.Value.Checked = value;

                    _basic_settings_state[item.Key.ToLower()] = value;
                }
            }
        }

        /// <summary>
        /// Parses custom settings, stores them and updates the checked state of already added tree nodes.
        /// </summary>
        /// 
        private void ParseCustomSettingsFromXml(XmlElement data)
        {
            XmlElement custom_settings_node = data["CustomSettings"];

            if (custom_settings_node != null && custom_settings_node.HasChildNodes)
            {
                foreach (XmlElement element in custom_settings_node.ChildNodes)
                {
                    if (element.Name != "Setting")
                        continue;

                    string id = element.Attributes["id"].Value;
                    string type = element.Attributes["type"].Value;

                    if (id != null && type == "bool")
                    {
                        bool value = SettingsHelper.ParseBool(element);
                        _custom_settings_state[id] = value;
                    }
                }
            }

            // Update tree with loaded state (in case the tree is already populated)
            UpdateNodesCheckedState(_custom_settings_state);
        }

        private void InitBasicSettings(VASLSettings settings)
        {
            foreach (var item in _basic_settings)
            {
                string name = item.Key.ToLower();
                CheckBox checkbox = item.Value;

                if (settings.IsBasicSettingPresent(name))
                {
                    VASLSetting setting = settings.BasicSettings[name];
                    checkbox.Enabled = true;
                    checkbox.Tag = setting;
                    var value = setting.Value;

                    if (_basic_settings_state.ContainsKey(name))
                        value = _basic_settings_state[name];

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

        private void UpdateCustomSettingsVisibility()
        {/*
            bool show = this.treeCustomSettings.GetNodeCount(false) > 0;
            this.treeCustomSettings.Visible = show;
            this.btnResetToDefault.Visible = show;
            this.btnCheckAll.Visible = show;
            this.btnUncheckAll.Visible = show;
            this.labelCustomSettings.Visible = show;*/
        }

        /// <summary>
        /// Generic update on all given nodes and their childnodes, ignoring childnodes for
        /// nodes where the Func returns false.
        /// </summary>
        /// 
        private void UpdateNodesInTree(Func<TreeNode, bool> func, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                bool include_child_nodes = func(node);
                if (include_child_nodes)
                    UpdateNodesInTree(func, node.Nodes);
            }
        }

        /// <summary>
        /// Update the checked state of all given nodes and their childnodes based on the return
        /// value of the given Func.
        /// </summary>
        /// <param name="nodes">If nodes is null, all nodes of the custom settings tree are affected.</param>
        /// 
        private void UpdateNodesCheckedState(Func<VASLSetting, bool> func, TreeNodeCollection nodes = null)
        {/*
            if (nodes == null)
                nodes = this.treeCustomSettings.Nodes;

            UpdateNodesInTree(node => {
                var setting = (VASLSetting)node.Tag;
                bool check = func(setting);

                if (node.Checked != check)
                    node.Checked = check;

                return true;
            }, nodes);*/
        }

        /// <summary>
        /// Update the checked state of all given nodes and their childnodes
        /// based on a dictionary of setting values.
        /// </summary>
        /// 
        private void UpdateNodesCheckedState(Dictionary<string, bool> setting_values, TreeNodeCollection nodes = null)
        {
            if (setting_values == null)
                return;

            UpdateNodesCheckedState(setting => {
                string id = setting.Id;

                if (setting_values.ContainsKey(id))
                    return setting_values[id];

                return setting.Value;
            }, nodes);
        }

        private void UpdateNodeCheckedState(Func<VASLSetting, bool> func, TreeNode node)
        {
            var setting = (VASLSetting)node.Tag;
            bool check = func(setting);

            if (node.Checked != check)
                node.Checked = check;
        }

        /// <summary>
        /// If the given node is unchecked, grays out all childnodes.
        /// </summary>
        private void UpdateGrayedOut(TreeNode node)
        {
            // Only change color of childnodes if this node isn't already grayed out
            if (node.ForeColor != SystemColors.GrayText)
            {
                UpdateNodesInTree(n => {
                    n.ForeColor = node.Checked ? SystemColors.WindowText : SystemColors.GrayText;
                    return n.Checked || !node.Checked;
                }, node.Nodes);
            }
        }


        // Events

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "Auto Split Script (*.VASL)|*.VASL|All Files (*.*)|*.*"
            };
            if (File.Exists(ScriptPath))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(ScriptPath);
                dialog.FileName = Path.GetFileName(ScriptPath);
            }

            if (dialog.ShowDialog() == DialogResult.OK)
                ScriptPath = this.txtGameProfile.Text = dialog.FileName;
        }

        // Basic Setting checked/unchecked
        //
        // This detects both changes made by the user and by the program, so this should
        // change the state in _basic_settings_state fine as well.
        private void methodCheckbox_CheckedChanged(object sender, EventArgs e)
        {

        }

        // Custom Setting checked/unchecked (only after initially building the tree)
        private void settingsTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // Update value in the VASLSetting object, which also changes it in the VASL script
            VASLSetting setting = (VASLSetting)e.Node.Tag;
            setting.Value = e.Node.Checked;
            _custom_settings_state[setting.Id] = setting.Value;

            UpdateGrayedOut(e.Node);
        }

        private void settingsTree_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = e.Node.ForeColor == SystemColors.GrayText;
        }

        // Custom Settings Button Events

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            UpdateNodesCheckedState(s => true);
        }

        private void btnUncheckAll_Click(object sender, EventArgs e)
        {
            UpdateNodesCheckedState(s => false);
        }

        private void btnResetToDefault_Click(object sender, EventArgs e)
        {
            UpdateNodesCheckedState(s => s.DefaultValue);
        }


        // Custom Settings Context Menu Events

        private void settingsTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Select clicked node (not only with left-click) for use with context menu
            //this.treeCustomSettings.SelectedNode = e.Node;
        }

        private void cmiCheckBranch_Click(object sender, EventArgs e)
        {/*
            UpdateNodesCheckedState(s => true, this.treeCustomSettings.SelectedNode.Nodes);
            UpdateNodeCheckedState(s => true, this.treeCustomSettings.SelectedNode);*/
        }

        private void cmiUncheckBranch_Click(object sender, EventArgs e)
        {/*
            UpdateNodesCheckedState(s => false, this.treeCustomSettings.SelectedNode.Nodes);
            UpdateNodeCheckedState(s => false, this.treeCustomSettings.SelectedNode);*/
        }

        private void cmiResetBranchToDefault_Click(object sender, EventArgs e)
        {/*
            UpdateNodesCheckedState(s => s.DefaultValue, this.treeCustomSettings.SelectedNode.Nodes);
            UpdateNodeCheckedState(s => s.DefaultValue, this.treeCustomSettings.SelectedNode);*/
        }

        private void cmiExpandBranch_Click(object sender, EventArgs e)
        {/*
            this.treeCustomSettings.SelectedNode.ExpandAll();
            this.treeCustomSettings.SelectedNode.EnsureVisible();*/
        }

        private void cmiCollapseBranch_Click(object sender, EventArgs e)
        {/*
            this.treeCustomSettings.SelectedNode.Collapse();
            this.treeCustomSettings.SelectedNode.EnsureVisible();*/
        }

        private void cmiCollapseTreeToSelection_Click(object sender, EventArgs e)
        {/*
            TreeNode selected = this.treeCustomSettings.SelectedNode;
            this.treeCustomSettings.CollapseAll();
            this.treeCustomSettings.SelectedNode = selected;
            selected.EnsureVisible();*/
        }

        private void cmiExpandTree_Click(object sender, EventArgs e)
        {/*
            this.treeCustomSettings.ExpandAll();
            this.treeCustomSettings.SelectedNode.EnsureVisible();*/
        }

        private void cmiCollapseTree_Click(object sender, EventArgs e)
        {/*
            this.treeCustomSettings.CollapseAll();*/
        }

        private void cmiResetSettingToDefault_Click(object sender, EventArgs e)
        {/*
            UpdateNodeCheckedState(s => s.DefaultValue, this.treeCustomSettings.SelectedNode);*/
        }

        // Temporary until better UI solution
        private void BtnGameProfile_Click(object sender, EventArgs e)
        {
            if (ModifierKeys.HasFlag(Keys.Shift))
                SetGameProfileWithFolder();
            else
                SetGameProfileWithFile();
        }

        private void SetGameProfileWithFile()
        {
            using (var ofd = new OpenFileDialog()
            {
                Filter = "VASL files (*.vasl)|*.vasl|ZIP files|*.zip|All files (*.*)|*.*",
                Title = "Load a Game Profile"
            })
            {
                if (File.Exists(ScriptPath))
                {
                    ofd.InitialDirectory = Path.GetDirectoryName(ScriptPath);
                    ofd.FileName = Path.GetFileName(ScriptPath);
                }

                if (ofd.ShowDialog() == DialogResult.OK && ofd.CheckFileExists == true)
                {
                    retry:
                    var gp = GameProfile.FromPath(ofd.FileName);

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
                        //Scanner.GameProfile = gp;
                        ScriptPath = this.txtGameProfile.Text = ofd.FileName;
                        //Scanner.AsyncStart();
                        Component.UpdateScript(null, null);
                    }
                }
            }
        }

        private void SetGameProfileWithFolder()
        {
            using (var fbd = new FolderBrowserDialog() { ShowNewFolderButton = false } )
            {
                if (Directory.Exists(ScriptPath))
                {
                    fbd.SelectedPath = ScriptPath;
                }
                else if (File.Exists(ScriptPath))
                {
                    fbd.SelectedPath = Path.GetDirectoryName(ScriptPath);
                }

                if (fbd.ShowDialog() == DialogResult.OK && Directory.Exists(fbd.SelectedPath))
                {
                    retry:
                    var gp = GameProfile.FromPath(fbd.SelectedPath);

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
                        ScriptPath = this.txtGameProfile.Text = fbd.SelectedPath;
                        Component.UpdateScript(null, null);
                    }
                }
            }
        }

        private void BtnCaptureDevice_Click(object sender, EventArgs e)
        {
        retry:
            var success = FillboxCaptureDevice();
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

        private bool FillboxCaptureDevice()
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count > 0)
            {
                boxCaptureDevice.Enabled = true;
                string selectedItem = string.Empty;
                int selectedIndex = 0;
                if (boxCaptureDevice.SelectedIndex > -1)
                {
                    selectedItem = (string)boxCaptureDevice.SelectedItem;
                }

                boxCaptureDevice.Items.Clear();
                for (var i = 0; i < videoDevices.Count; i++)
                {
                    boxCaptureDevice.Items.Add(videoDevices[i].Name);
                    if (videoDevices[i].Name == selectedItem)
                    {
                        selectedIndex = i;
                    }
                }
                boxCaptureDevice.SelectedIndex = selectedIndex;
                return true;
            }
            else
            {
                boxCaptureDevice.Items.Clear();
                boxCaptureDevice.Enabled = false;
                return false;
            }
        }

        private void boxCaptureDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            Scanner.Stop();
        retry:
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            var matches = videoDevices.Where(v => v.Name == boxCaptureDevice.Text);
            if (matches.Count() > 0)
            {
                var match = matches.First();
                Scanner.SetVideoSource(match.MonikerString);
                //Properties.Settings.Default.VideoDevice = boxCaptureDevice.Text;
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
                    FillboxCaptureDevice();
                }
            }
            Scanner.AsyncStart();
        }

        private void BtnSetCaptureRegion_Click(object sender, EventArgs e)
        {
            using (Aligner w = new Aligner())
            {
                w.ShowDialog();

            }
        }


    }
}
