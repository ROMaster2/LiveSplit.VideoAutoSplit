namespace LiveSplit.VAS.UI
{
    partial class SettingsUI
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tlpCore = new System.Windows.Forms.TableLayoutPanel();
            this.lblAdvanced = new System.Windows.Forms.Label();
            this.tlpCaptureDevice = new System.Windows.Forms.TableLayoutPanel();
            this.boxCaptureDevice = new System.Windows.Forms.ComboBox();
            this.btnCaptureDevice = new System.Windows.Forms.Button();
            this.lblGameProfile = new System.Windows.Forms.Label();
            this.lblCaptureDevice = new System.Windows.Forms.Label();
            this.tlpGameProfile = new System.Windows.Forms.TableLayoutPanel();
            this.btnGameProfile = new System.Windows.Forms.Button();
            this.txtGameProfile = new System.Windows.Forms.TextBox();
            this.lblOptions = new System.Windows.Forms.Label();
            this.tlpOptions = new System.Windows.Forms.TableLayoutPanel();
            this.ckbReset = new System.Windows.Forms.CheckBox();
            this.boxGameVersion = new System.Windows.Forms.ComboBox();
            this.ckbSplit = new System.Windows.Forms.CheckBox();
            this.ckbStart = new System.Windows.Forms.CheckBox();
            this.lblGameVersion = new System.Windows.Forms.Label();
            this.tlpAdvanced = new System.Windows.Forms.TableLayoutPanel();
            this.btnResetToDefault = new System.Windows.Forms.Button();
            this.btnUncheckAll = new System.Windows.Forms.Button();
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.treeCustomSettings = new LiveSplit.VAS.Models.DropDownTreeView();
            this.tlpCore.SuspendLayout();
            this.tlpCaptureDevice.SuspendLayout();
            this.tlpGameProfile.SuspendLayout();
            this.tlpOptions.SuspendLayout();
            this.tlpAdvanced.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpCore
            // 
            this.tlpCore.ColumnCount = 2;
            this.tlpCore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpCore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCore.Controls.Add(this.lblAdvanced, 0, 3);
            this.tlpCore.Controls.Add(this.tlpCaptureDevice, 1, 1);
            this.tlpCore.Controls.Add(this.lblGameProfile, 0, 0);
            this.tlpCore.Controls.Add(this.lblCaptureDevice, 0, 1);
            this.tlpCore.Controls.Add(this.tlpGameProfile, 1, 0);
            this.tlpCore.Controls.Add(this.lblOptions, 0, 2);
            this.tlpCore.Controls.Add(this.tlpOptions, 1, 2);
            this.tlpCore.Controls.Add(this.tlpAdvanced, 1, 4);
            this.tlpCore.Controls.Add(this.treeCustomSettings, 1, 3);
            this.tlpCore.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpCore.Location = new System.Drawing.Point(7, 7);
            this.tlpCore.Name = "tlpCore";
            this.tlpCore.RowCount = 5;
            this.tlpCore.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCore.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCore.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCore.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCore.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCore.Size = new System.Drawing.Size(454, 472);
            this.tlpCore.TabIndex = 0;
            // 
            // lblAdvanced
            // 
            this.lblAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAdvanced.Location = new System.Drawing.Point(3, 95);
            this.lblAdvanced.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
            this.lblAdvanced.Name = "lblAdvanced";
            this.lblAdvanced.Size = new System.Drawing.Size(81, 13);
            this.lblAdvanced.TabIndex = 0;
            this.lblAdvanced.Text = "Advanced";
            // 
            // tlpCaptureDevice
            // 
            this.tlpCaptureDevice.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpCaptureDevice.ColumnCount = 2;
            this.tlpCaptureDevice.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCaptureDevice.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpCaptureDevice.Controls.Add(this.boxCaptureDevice, 0, 0);
            this.tlpCaptureDevice.Controls.Add(this.btnCaptureDevice, 1, 0);
            this.tlpCaptureDevice.Location = new System.Drawing.Point(87, 29);
            this.tlpCaptureDevice.Margin = new System.Windows.Forms.Padding(0);
            this.tlpCaptureDevice.Name = "tlpCaptureDevice";
            this.tlpCaptureDevice.RowCount = 1;
            this.tlpCaptureDevice.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCaptureDevice.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tlpCaptureDevice.Size = new System.Drawing.Size(367, 29);
            this.tlpCaptureDevice.TabIndex = 0;
            // 
            // boxCaptureDevice
            // 
            this.boxCaptureDevice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.boxCaptureDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.boxCaptureDevice.Enabled = false;
            this.boxCaptureDevice.FormattingEnabled = true;
            this.boxCaptureDevice.Location = new System.Drawing.Point(3, 4);
            this.boxCaptureDevice.Name = "boxCaptureDevice";
            this.boxCaptureDevice.Size = new System.Drawing.Size(281, 21);
            this.boxCaptureDevice.TabIndex = 3;
            this.boxCaptureDevice.SelectedIndexChanged += new System.EventHandler(this.BoxCaptureDevice_SelectedIndexChanged);
            // 
            // btnCaptureDevice
            // 
            this.btnCaptureDevice.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCaptureDevice.Location = new System.Drawing.Point(290, 3);
            this.btnCaptureDevice.Name = "btnCaptureDevice";
            this.btnCaptureDevice.Size = new System.Drawing.Size(74, 23);
            this.btnCaptureDevice.TabIndex = 4;
            this.btnCaptureDevice.Text = "Refresh";
            this.btnCaptureDevice.UseVisualStyleBackColor = true;
            this.btnCaptureDevice.Click += new System.EventHandler(this.BtnCaptureDevice_Click);
            // 
            // lblGameProfile
            // 
            this.lblGameProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGameProfile.AutoSize = true;
            this.lblGameProfile.Location = new System.Drawing.Point(3, 8);
            this.lblGameProfile.Name = "lblGameProfile";
            this.lblGameProfile.Size = new System.Drawing.Size(81, 13);
            this.lblGameProfile.TabIndex = 0;
            this.lblGameProfile.Text = "Game Profile";
            // 
            // lblCaptureDevice
            // 
            this.lblCaptureDevice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCaptureDevice.AutoSize = true;
            this.lblCaptureDevice.Location = new System.Drawing.Point(3, 37);
            this.lblCaptureDevice.Name = "lblCaptureDevice";
            this.lblCaptureDevice.Size = new System.Drawing.Size(81, 13);
            this.lblCaptureDevice.TabIndex = 0;
            this.lblCaptureDevice.Text = "Capture Device";
            // 
            // tlpGameProfile
            // 
            this.tlpGameProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpGameProfile.ColumnCount = 2;
            this.tlpGameProfile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpGameProfile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpGameProfile.Controls.Add(this.btnGameProfile, 1, 0);
            this.tlpGameProfile.Controls.Add(this.txtGameProfile, 0, 0);
            this.tlpGameProfile.Location = new System.Drawing.Point(87, 0);
            this.tlpGameProfile.Margin = new System.Windows.Forms.Padding(0);
            this.tlpGameProfile.Name = "tlpGameProfile";
            this.tlpGameProfile.RowCount = 1;
            this.tlpGameProfile.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpGameProfile.Size = new System.Drawing.Size(367, 29);
            this.tlpGameProfile.TabIndex = 0;
            // 
            // btnGameProfile
            // 
            this.btnGameProfile.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnGameProfile.Location = new System.Drawing.Point(290, 3);
            this.btnGameProfile.Name = "btnGameProfile";
            this.btnGameProfile.Size = new System.Drawing.Size(74, 23);
            this.btnGameProfile.TabIndex = 2;
            this.btnGameProfile.Text = "Browse...";
            this.btnGameProfile.UseVisualStyleBackColor = true;
            this.btnGameProfile.Click += new System.EventHandler(this.BtnGameProfile_Click);
            // 
            // txtGameProfile
            // 
            this.txtGameProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGameProfile.Location = new System.Drawing.Point(3, 4);
            this.txtGameProfile.MaxLength = 600;
            this.txtGameProfile.Name = "txtGameProfile";
            this.txtGameProfile.ReadOnly = true;
            this.txtGameProfile.Size = new System.Drawing.Size(281, 20);
            this.txtGameProfile.TabIndex = 1;
            // 
            // lblOptions
            // 
            this.lblOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOptions.AutoSize = true;
            this.lblOptions.Location = new System.Drawing.Point(3, 66);
            this.lblOptions.Name = "lblOptions";
            this.lblOptions.Size = new System.Drawing.Size(81, 13);
            this.lblOptions.TabIndex = 0;
            this.lblOptions.Text = "Options";
            // 
            // tlpOptions
            // 
            this.tlpOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpOptions.ColumnCount = 5;
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpOptions.Controls.Add(this.ckbReset, 2, 0);
            this.tlpOptions.Controls.Add(this.boxGameVersion, 4, 0);
            this.tlpOptions.Controls.Add(this.ckbSplit, 1, 0);
            this.tlpOptions.Controls.Add(this.ckbStart, 0, 0);
            this.tlpOptions.Controls.Add(this.lblGameVersion, 3, 0);
            this.tlpOptions.Location = new System.Drawing.Point(87, 58);
            this.tlpOptions.Margin = new System.Windows.Forms.Padding(0);
            this.tlpOptions.Name = "tlpOptions";
            this.tlpOptions.RowCount = 1;
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpOptions.Size = new System.Drawing.Size(367, 29);
            this.tlpOptions.TabIndex = 0;
            // 
            // ckbReset
            // 
            this.ckbReset.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbReset.AutoSize = true;
            this.ckbReset.Enabled = false;
            this.ckbReset.Location = new System.Drawing.Point(109, 4);
            this.ckbReset.Name = "ckbReset";
            this.ckbReset.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.ckbReset.Size = new System.Drawing.Size(54, 20);
            this.ckbReset.TabIndex = 7;
            this.ckbReset.Text = "Reset";
            this.ckbReset.UseVisualStyleBackColor = true;
            this.ckbReset.CheckedChanged += new System.EventHandler(this.MethodCheckbox_CheckedChanged);
            // 
            // boxGameVersion
            // 
            this.boxGameVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.boxGameVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.boxGameVersion.Enabled = false;
            this.boxGameVersion.FormattingEnabled = true;
            this.boxGameVersion.Location = new System.Drawing.Point(248, 4);
            this.boxGameVersion.MaxDropDownItems = 64;
            this.boxGameVersion.Name = "boxGameVersion";
            this.boxGameVersion.Size = new System.Drawing.Size(116, 21);
            this.boxGameVersion.TabIndex = 8;
            // 
            // ckbSplit
            // 
            this.ckbSplit.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbSplit.AutoSize = true;
            this.ckbSplit.Enabled = false;
            this.ckbSplit.Location = new System.Drawing.Point(57, 4);
            this.ckbSplit.Name = "ckbSplit";
            this.ckbSplit.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.ckbSplit.Size = new System.Drawing.Size(46, 20);
            this.ckbSplit.TabIndex = 6;
            this.ckbSplit.Text = "Split";
            this.ckbSplit.UseVisualStyleBackColor = true;
            this.ckbSplit.CheckedChanged += new System.EventHandler(this.MethodCheckbox_CheckedChanged);
            // 
            // ckbStart
            // 
            this.ckbStart.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbStart.AutoSize = true;
            this.ckbStart.Enabled = false;
            this.ckbStart.Location = new System.Drawing.Point(3, 4);
            this.ckbStart.Name = "ckbStart";
            this.ckbStart.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.ckbStart.Size = new System.Drawing.Size(48, 20);
            this.ckbStart.TabIndex = 5;
            this.ckbStart.Text = "Start";
            this.ckbStart.UseVisualStyleBackColor = true;
            this.ckbStart.CheckedChanged += new System.EventHandler(this.MethodCheckbox_CheckedChanged);
            // 
            // lblGameVersion
            // 
            this.lblGameVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGameVersion.AutoSize = true;
            this.lblGameVersion.Location = new System.Drawing.Point(169, 8);
            this.lblGameVersion.Name = "lblGameVersion";
            this.lblGameVersion.Size = new System.Drawing.Size(73, 13);
            this.lblGameVersion.TabIndex = 1;
            this.lblGameVersion.Text = "Game Version";
            // 
            // tlpAdvanced
            // 
            this.tlpAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpAdvanced.ColumnCount = 4;
            this.tlpAdvanced.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAdvanced.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpAdvanced.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpAdvanced.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpAdvanced.Controls.Add(this.btnResetToDefault, 3, 0);
            this.tlpAdvanced.Controls.Add(this.btnUncheckAll, 2, 0);
            this.tlpAdvanced.Controls.Add(this.btnCheckAll, 1, 0);
            this.tlpAdvanced.Location = new System.Drawing.Point(87, 443);
            this.tlpAdvanced.Margin = new System.Windows.Forms.Padding(0);
            this.tlpAdvanced.Name = "tlpAdvanced";
            this.tlpAdvanced.RowCount = 1;
            this.tlpAdvanced.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAdvanced.Size = new System.Drawing.Size(367, 29);
            this.tlpAdvanced.TabIndex = 0;
            // 
            // btnResetToDefault
            // 
            this.btnResetToDefault.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnResetToDefault.Location = new System.Drawing.Point(265, 3);
            this.btnResetToDefault.Name = "btnResetToDefault";
            this.btnResetToDefault.Size = new System.Drawing.Size(99, 23);
            this.btnResetToDefault.TabIndex = 12;
            this.btnResetToDefault.Text = "Reset to Default";
            this.btnResetToDefault.UseVisualStyleBackColor = true;
            this.btnResetToDefault.Click += new System.EventHandler(this.BtnResetToDefault_Click);
            // 
            // btnUncheckAll
            // 
            this.btnUncheckAll.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnUncheckAll.Location = new System.Drawing.Point(184, 3);
            this.btnUncheckAll.Name = "btnUncheckAll";
            this.btnUncheckAll.Size = new System.Drawing.Size(75, 23);
            this.btnUncheckAll.TabIndex = 11;
            this.btnUncheckAll.Text = "Uncheck All";
            this.btnUncheckAll.UseVisualStyleBackColor = true;
            this.btnUncheckAll.Click += new System.EventHandler(this.BtnUncheckAll_Click);
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCheckAll.Location = new System.Drawing.Point(116, 3);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(62, 23);
            this.btnCheckAll.TabIndex = 10;
            this.btnCheckAll.Text = "Check All";
            this.btnCheckAll.UseVisualStyleBackColor = true;
            this.btnCheckAll.Click += new System.EventHandler(this.BtnCheckAll_Click);
            // 
            // treeCustomSettings
            // 
            this.treeCustomSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeCustomSettings.CheckBoxes = true;
            this.treeCustomSettings.Location = new System.Drawing.Point(90, 90);
            this.treeCustomSettings.Name = "treeCustomSettings";
            this.treeCustomSettings.ShowNodeToolTips = true;
            this.treeCustomSettings.Size = new System.Drawing.Size(361, 350);
            this.treeCustomSettings.TabIndex = 9;
            this.treeCustomSettings.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.SettingsTree_BeforeCheck);
            this.treeCustomSettings.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.SettingsTree_AfterCheck);
            this.treeCustomSettings.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.SettingsTree_NodeMouseClick);
            // 
            // SettingsUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpCore);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "SettingsUI";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.Size = new System.Drawing.Size(468, 486);
            this.VisibleChanged += new System.EventHandler(this.SettingsUI_VisibleChanged);
            this.tlpCore.ResumeLayout(false);
            this.tlpCore.PerformLayout();
            this.tlpCaptureDevice.ResumeLayout(false);
            this.tlpGameProfile.ResumeLayout(false);
            this.tlpGameProfile.PerformLayout();
            this.tlpOptions.ResumeLayout(false);
            this.tlpOptions.PerformLayout();
            this.tlpAdvanced.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpCore;
        private System.Windows.Forms.CheckBox ckbStart;
        private System.Windows.Forms.CheckBox ckbSplit;
        private System.Windows.Forms.CheckBox ckbReset;
        private System.Windows.Forms.Label lblAdvanced;
        private System.Windows.Forms.TableLayoutPanel tlpCaptureDevice;
        private System.Windows.Forms.ComboBox boxCaptureDevice;
        private System.Windows.Forms.Button btnCaptureDevice;
        private System.Windows.Forms.Label lblGameProfile;
        private System.Windows.Forms.Label lblCaptureDevice;
        private System.Windows.Forms.TableLayoutPanel tlpGameProfile;
        private System.Windows.Forms.Button btnGameProfile;
        private System.Windows.Forms.TextBox txtGameProfile;
        private System.Windows.Forms.Label lblOptions;
        private System.Windows.Forms.TableLayoutPanel tlpOptions;
        private System.Windows.Forms.TableLayoutPanel tlpAdvanced;
        private System.Windows.Forms.Button btnResetToDefault;
        private System.Windows.Forms.Button btnUncheckAll;
        private System.Windows.Forms.Button btnCheckAll;
        private System.Windows.Forms.ComboBox boxGameVersion;
        private LiveSplit.VAS.Models.DropDownTreeView treeCustomSettings;
        private System.Windows.Forms.Label lblGameVersion;
    }
}
