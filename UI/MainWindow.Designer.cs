namespace LiveSplit.VAS.Forms
{
    partial class MainWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnGameProfile = new System.Windows.Forms.Button();
            this.lblGameProfile = new System.Windows.Forms.Label();
            this.txtGameProfile = new System.Windows.Forms.TextBox();
            this.tlpDetectors = new System.Windows.Forms.TableLayoutPanel();
            this.lblFeature01 = new System.Windows.Forms.Label();
            this.lblFeature = new System.Windows.Forms.Label();
            this.lblDetectionRate01 = new System.Windows.Forms.Label();
            this.numSetThreshold01 = new System.Windows.Forms.NumericUpDown();
            this.lblDetectionRate = new System.Windows.Forms.Label();
            this.lblSetThreshold = new System.Windows.Forms.Label();
            this.lblTimeActive = new System.Windows.Forms.Label();
            this.lblAbove01 = new System.Windows.Forms.Label();
            this.BoxCaptureDevice = new System.Windows.Forms.ComboBox();
            this.lblCaptureDevice = new System.Windows.Forms.Label();
            this.btnCaptureDevice = new System.Windows.Forms.Button();
            this.BtnSetCaptureRegion = new System.Windows.Forms.Button();
            this.lblDetectors = new System.Windows.Forms.Label();
            this.tlpDetectors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSetThreshold01)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGameProfile
            // 
            this.btnGameProfile.Location = new System.Drawing.Point(12, 25);
            this.btnGameProfile.Name = "btnGameProfile";
            this.btnGameProfile.Size = new System.Drawing.Size(75, 21);
            this.btnGameProfile.TabIndex = 0;
            this.btnGameProfile.Text = "Browse...";
            this.btnGameProfile.UseVisualStyleBackColor = true;
            this.btnGameProfile.Click += new System.EventHandler(this.BtnGameProfile_Click);
            // 
            // lblGameProfile
            // 
            this.lblGameProfile.AutoSize = true;
            this.lblGameProfile.Location = new System.Drawing.Point(12, 9);
            this.lblGameProfile.Name = "lblGameProfile";
            this.lblGameProfile.Size = new System.Drawing.Size(67, 13);
            this.lblGameProfile.TabIndex = 1;
            this.lblGameProfile.Text = "Game Profile";
            // 
            // txtGameProfile
            // 
            this.txtGameProfile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGameProfile.Location = new System.Drawing.Point(86, 26);
            this.txtGameProfile.MaxLength = 600;
            this.txtGameProfile.Name = "txtGameProfile";
            this.txtGameProfile.ReadOnly = true;
            this.txtGameProfile.Size = new System.Drawing.Size(299, 20);
            this.txtGameProfile.TabIndex = 2;
            // 
            // tlpDetectors
            // 
            this.tlpDetectors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpDetectors.ColumnCount = 4;
            this.tlpDetectors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpDetectors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpDetectors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpDetectors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpDetectors.Controls.Add(this.lblFeature01, 0, 1);
            this.tlpDetectors.Controls.Add(this.lblFeature, 0, 0);
            this.tlpDetectors.Controls.Add(this.lblDetectionRate01, 1, 1);
            this.tlpDetectors.Controls.Add(this.numSetThreshold01, 2, 1);
            this.tlpDetectors.Controls.Add(this.lblDetectionRate, 1, 0);
            this.tlpDetectors.Controls.Add(this.lblSetThreshold, 2, 0);
            this.tlpDetectors.Controls.Add(this.lblTimeActive, 3, 0);
            this.tlpDetectors.Controls.Add(this.lblAbove01, 3, 1);
            this.tlpDetectors.Location = new System.Drawing.Point(12, 141);
            this.tlpDetectors.Name = "tlpDetectors";
            this.tlpDetectors.RowCount = 20;
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.263158F));
            this.tlpDetectors.Size = new System.Drawing.Size(373, 430);
            this.tlpDetectors.TabIndex = 3;
            // 
            // lblFeature01
            // 
            this.lblFeature01.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFeature01.Location = new System.Drawing.Point(3, 34);
            this.lblFeature01.Name = "lblFeature01";
            this.lblFeature01.Size = new System.Drawing.Size(87, 13);
            this.lblFeature01.TabIndex = 0;
            this.lblFeature01.Text = "Example.png";
            this.lblFeature01.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFeature
            // 
            this.lblFeature.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblFeature.AutoSize = true;
            this.lblFeature.Location = new System.Drawing.Point(3, 8);
            this.lblFeature.Name = "lblFeature";
            this.lblFeature.Size = new System.Drawing.Size(43, 13);
            this.lblFeature.TabIndex = 3;
            this.lblFeature.Text = "Feature";
            this.lblFeature.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDetectionRate01
            // 
            this.lblDetectionRate01.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDetectionRate01.Location = new System.Drawing.Point(96, 34);
            this.lblDetectionRate01.Name = "lblDetectionRate01";
            this.lblDetectionRate01.Size = new System.Drawing.Size(87, 13);
            this.lblDetectionRate01.TabIndex = 2;
            this.lblDetectionRate01.Text = "12.3456%";
            this.lblDetectionRate01.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numSetThreshold01
            // 
            this.numSetThreshold01.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.numSetThreshold01.DecimalPlaces = 4;
            this.numSetThreshold01.Location = new System.Drawing.Point(189, 31);
            this.numSetThreshold01.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.numSetThreshold01.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numSetThreshold01.Name = "numSetThreshold01";
            this.numSetThreshold01.Size = new System.Drawing.Size(87, 20);
            this.numSetThreshold01.TabIndex = 1;
            this.numSetThreshold01.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblDetectionRate
            // 
            this.lblDetectionRate.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblDetectionRate.AutoSize = true;
            this.lblDetectionRate.Location = new System.Drawing.Point(104, 8);
            this.lblDetectionRate.Name = "lblDetectionRate";
            this.lblDetectionRate.Size = new System.Drawing.Size(79, 13);
            this.lblDetectionRate.TabIndex = 5;
            this.lblDetectionRate.Text = "Detection Rate";
            this.lblDetectionRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSetThreshold
            // 
            this.lblSetThreshold.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblSetThreshold.AutoSize = true;
            this.lblSetThreshold.Location = new System.Drawing.Point(196, 8);
            this.lblSetThreshold.Name = "lblSetThreshold";
            this.lblSetThreshold.Size = new System.Drawing.Size(73, 13);
            this.lblSetThreshold.TabIndex = 4;
            this.lblSetThreshold.Text = "Set Threshold";
            this.lblSetThreshold.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTimeActive
            // 
            this.lblTimeActive.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblTimeActive.AutoSize = true;
            this.lblTimeActive.Location = new System.Drawing.Point(307, 8);
            this.lblTimeActive.Name = "lblTimeActive";
            this.lblTimeActive.Size = new System.Drawing.Size(63, 13);
            this.lblTimeActive.TabIndex = 6;
            this.lblTimeActive.Text = "Time Active";
            this.lblTimeActive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAbove01
            // 
            this.lblAbove01.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAbove01.Location = new System.Drawing.Point(282, 34);
            this.lblAbove01.Name = "lblAbove01";
            this.lblAbove01.Size = new System.Drawing.Size(88, 13);
            this.lblAbove01.TabIndex = 7;
            this.lblAbove01.Text = "00:00:00.000";
            this.lblAbove01.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // BoxCaptureDevice
            // 
            this.BoxCaptureDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BoxCaptureDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BoxCaptureDevice.Enabled = false;
            this.BoxCaptureDevice.FormattingEnabled = true;
            this.BoxCaptureDevice.Location = new System.Drawing.Point(87, 66);
            this.BoxCaptureDevice.Name = "BoxCaptureDevice";
            this.BoxCaptureDevice.Size = new System.Drawing.Size(298, 21);
            this.BoxCaptureDevice.TabIndex = 4;
            this.BoxCaptureDevice.SelectedIndexChanged += new System.EventHandler(this.BoxCaptureDevice_SelectedIndexChanged);
            // 
            // lblCaptureDevice
            // 
            this.lblCaptureDevice.AutoSize = true;
            this.lblCaptureDevice.Location = new System.Drawing.Point(12, 49);
            this.lblCaptureDevice.Name = "lblCaptureDevice";
            this.lblCaptureDevice.Size = new System.Drawing.Size(81, 13);
            this.lblCaptureDevice.TabIndex = 5;
            this.lblCaptureDevice.Text = "Capture Device";
            // 
            // btnCaptureDevice
            // 
            this.btnCaptureDevice.Location = new System.Drawing.Point(12, 65);
            this.btnCaptureDevice.Name = "btnCaptureDevice";
            this.btnCaptureDevice.Size = new System.Drawing.Size(75, 23);
            this.btnCaptureDevice.TabIndex = 6;
            this.btnCaptureDevice.Text = "Refresh";
            this.btnCaptureDevice.UseVisualStyleBackColor = true;
            this.btnCaptureDevice.Click += new System.EventHandler(this.BtnCaptureDevice_Click);
            // 
            // BtnSetCaptureRegion
            // 
            this.BtnSetCaptureRegion.Location = new System.Drawing.Point(12, 93);
            this.BtnSetCaptureRegion.Name = "BtnSetCaptureRegion";
            this.BtnSetCaptureRegion.Size = new System.Drawing.Size(373, 23);
            this.BtnSetCaptureRegion.TabIndex = 12;
            this.BtnSetCaptureRegion.Text = "Set Capture Region";
            this.BtnSetCaptureRegion.UseVisualStyleBackColor = true;
            this.BtnSetCaptureRegion.Click += new System.EventHandler(this.BtnSetCaptureRegion_Click);
            // 
            // lblDetectors
            // 
            this.lblDetectors.AutoSize = true;
            this.lblDetectors.Location = new System.Drawing.Point(12, 125);
            this.lblDetectors.Name = "lblDetectors";
            this.lblDetectors.Size = new System.Drawing.Size(53, 13);
            this.lblDetectors.TabIndex = 9;
            this.lblDetectors.Text = "Detectors";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 580);
            this.Controls.Add(this.lblDetectors);
            this.Controls.Add(this.btnCaptureDevice);
            this.Controls.Add(this.lblCaptureDevice);
            this.Controls.Add(this.BoxCaptureDevice);
            this.Controls.Add(this.tlpDetectors);
            this.Controls.Add(this.txtGameProfile);
            this.Controls.Add(this.lblGameProfile);
            this.Controls.Add(this.BtnSetCaptureRegion);
            this.Controls.Add(this.btnGameProfile);
            this.MinimumSize = new System.Drawing.Size(413, 320);
            this.Name = "MainWindow";
            this.Text = "Video Autosplitter Prototype by ROMaster2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.tlpDetectors.ResumeLayout(false);
            this.tlpDetectors.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSetThreshold01)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGameProfile;
        private System.Windows.Forms.Label lblGameProfile;
        private System.Windows.Forms.TextBox txtGameProfile;
        private System.Windows.Forms.TableLayoutPanel tlpDetectors;
        private System.Windows.Forms.Label lblFeature01;
        private System.Windows.Forms.NumericUpDown numSetThreshold01;
        private System.Windows.Forms.Label lblDetectionRate01;
        private System.Windows.Forms.Label lblFeature;
        private System.Windows.Forms.Label lblDetectionRate;
        private System.Windows.Forms.Label lblSetThreshold;
        private System.Windows.Forms.ComboBox BoxCaptureDevice;
        private System.Windows.Forms.Label lblCaptureDevice;
        private System.Windows.Forms.Button btnCaptureDevice;
        private System.Windows.Forms.Label lblDetectors;
        private System.Windows.Forms.Label lblTimeActive;
        private System.Windows.Forms.Label lblAbove01;
        private System.Windows.Forms.Button BtnSetCaptureRegion;
    }
}

