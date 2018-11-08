namespace LiveSplit.VAS
{
    partial class ComponentSettings
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
            this.btnCaptureDevice = new System.Windows.Forms.Button();
            this.lblCaptureDevice = new System.Windows.Forms.Label();
            this.boxCaptureDevice = new System.Windows.Forms.ComboBox();
            this.txtGameProfile = new System.Windows.Forms.TextBox();
            this.lblGameProfile = new System.Windows.Forms.Label();
            this.btnSetCaptureRegion = new System.Windows.Forms.Button();
            this.btnGameProfile = new System.Windows.Forms.Button();
            this.tlpCore = new System.Windows.Forms.TableLayoutPanel();
            this.tlpDetectors = new System.Windows.Forms.TableLayoutPanel();
            this.lblFeature01 = new System.Windows.Forms.Label();
            this.lblFeature = new System.Windows.Forms.Label();
            this.lblDetectionRate01 = new System.Windows.Forms.Label();
            this.numSetThreshold01 = new System.Windows.Forms.NumericUpDown();
            this.lblDetectionRate = new System.Windows.Forms.Label();
            this.lblSetThreshold = new System.Windows.Forms.Label();
            this.lblTimeActive = new System.Windows.Forms.Label();
            this.lblAbove01 = new System.Windows.Forms.Label();
            this.lblDetectors = new System.Windows.Forms.Label();
            this.tlpTodo = new System.Windows.Forms.TableLayoutPanel();
            this.btnAutoSplitterSettings = new System.Windows.Forms.Button();
            this.tlpCaptureDevice = new System.Windows.Forms.TableLayoutPanel();
            this.tlpGameProfile = new System.Windows.Forms.TableLayoutPanel();
            this.lblTodo = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkboxStart = new System.Windows.Forms.CheckBox();
            this.checkboxSplit = new System.Windows.Forms.CheckBox();
            this.checkboxReset = new System.Windows.Forms.CheckBox();
            this.lblGameVersion = new System.Windows.Forms.Label();
            this.tlpCore.SuspendLayout();
            this.tlpDetectors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSetThreshold01)).BeginInit();
            this.tlpTodo.SuspendLayout();
            this.tlpCaptureDevice.SuspendLayout();
            this.tlpGameProfile.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCaptureDevice
            // 
            this.btnCaptureDevice.Location = new System.Drawing.Point(309, 3);
            this.btnCaptureDevice.Name = "btnCaptureDevice";
            this.btnCaptureDevice.Size = new System.Drawing.Size(74, 23);
            this.btnCaptureDevice.TabIndex = 19;
            this.btnCaptureDevice.Text = "Refresh";
            this.btnCaptureDevice.UseVisualStyleBackColor = true;
            this.btnCaptureDevice.Click += new System.EventHandler(this.BtnCaptureDevice_Click);
            // 
            // lblCaptureDevice
            // 
            this.lblCaptureDevice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCaptureDevice.AutoSize = true;
            this.lblCaptureDevice.Location = new System.Drawing.Point(3, 30);
            this.lblCaptureDevice.Name = "lblCaptureDevice";
            this.lblCaptureDevice.Size = new System.Drawing.Size(70, 26);
            this.lblCaptureDevice.TabIndex = 18;
            this.lblCaptureDevice.Text = "Capture Device";
            // 
            // boxCaptureDevice
            // 
            this.boxCaptureDevice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.boxCaptureDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.boxCaptureDevice.Enabled = false;
            this.boxCaptureDevice.FormattingEnabled = true;
            this.boxCaptureDevice.Location = new System.Drawing.Point(3, 4);
            this.boxCaptureDevice.Name = "boxCaptureDevice";
            this.boxCaptureDevice.Size = new System.Drawing.Size(300, 21);
            this.boxCaptureDevice.TabIndex = 17;
            this.boxCaptureDevice.SelectedIndexChanged += new System.EventHandler(this.boxCaptureDevice_SelectedIndexChanged);
            // 
            // txtGameProfile
            // 
            this.txtGameProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGameProfile.Location = new System.Drawing.Point(3, 4);
            this.txtGameProfile.MaxLength = 600;
            this.txtGameProfile.Name = "txtGameProfile";
            this.txtGameProfile.ReadOnly = true;
            this.txtGameProfile.Size = new System.Drawing.Size(300, 20);
            this.txtGameProfile.TabIndex = 15;
            // 
            // lblGameProfile
            // 
            this.lblGameProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGameProfile.AutoSize = true;
            this.lblGameProfile.Location = new System.Drawing.Point(3, 8);
            this.lblGameProfile.Name = "lblGameProfile";
            this.lblGameProfile.Size = new System.Drawing.Size(70, 13);
            this.lblGameProfile.TabIndex = 14;
            this.lblGameProfile.Text = "Game Profile";
            // 
            // btnSetCaptureRegion
            // 
            this.btnSetCaptureRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetCaptureRegion.Location = new System.Drawing.Point(3, 3);
            this.btnSetCaptureRegion.Name = "btnSetCaptureRegion";
            this.btnSetCaptureRegion.Size = new System.Drawing.Size(187, 23);
            this.btnSetCaptureRegion.TabIndex = 21;
            this.btnSetCaptureRegion.Text = "Set Capture Region";
            this.btnSetCaptureRegion.UseVisualStyleBackColor = true;
            this.btnSetCaptureRegion.Click += new System.EventHandler(this.BtnSetCaptureRegion_Click);
            // 
            // btnGameProfile
            // 
            this.btnGameProfile.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnGameProfile.Location = new System.Drawing.Point(309, 3);
            this.btnGameProfile.Name = "btnGameProfile";
            this.btnGameProfile.Size = new System.Drawing.Size(74, 23);
            this.btnGameProfile.TabIndex = 13;
            this.btnGameProfile.Text = "Browse...";
            this.btnGameProfile.UseVisualStyleBackColor = true;
            this.btnGameProfile.Click += new System.EventHandler(this.BtnGameProfile_Click);
            // 
            // tlpCore
            // 
            this.tlpCore.ColumnCount = 2;
            this.tlpCore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.tlpCore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCore.Controls.Add(this.flowLayoutPanel2, 1, 4);
            this.tlpCore.Controls.Add(this.tlpDetectors, 1, 3);
            this.tlpCore.Controls.Add(this.lblDetectors, 0, 3);
            this.tlpCore.Controls.Add(this.tlpTodo, 1, 2);
            this.tlpCore.Controls.Add(this.tlpCaptureDevice, 1, 1);
            this.tlpCore.Controls.Add(this.lblGameProfile, 0, 0);
            this.tlpCore.Controls.Add(this.lblCaptureDevice, 0, 1);
            this.tlpCore.Controls.Add(this.tlpGameProfile, 1, 0);
            this.tlpCore.Controls.Add(this.lblTodo, 0, 2);
            this.tlpCore.Location = new System.Drawing.Point(7, 7);
            this.tlpCore.Name = "tlpCore";
            this.tlpCore.RowCount = 5;
            this.tlpCore.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tlpCore.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tlpCore.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tlpCore.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCore.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tlpCore.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpCore.Size = new System.Drawing.Size(462, 498);
            this.tlpCore.TabIndex = 22;
            // 
            // tlpDetectors
            // 
            this.tlpDetectors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpDetectors.ColumnCount = 5;
            this.tlpDetectors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpDetectors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpDetectors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpDetectors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpDetectors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpDetectors.Controls.Add(this.lblFeature01, 0, 1);
            this.tlpDetectors.Controls.Add(this.lblFeature, 0, 0);
            this.tlpDetectors.Controls.Add(this.lblDetectionRate01, 1, 1);
            this.tlpDetectors.Controls.Add(this.numSetThreshold01, 2, 1);
            this.tlpDetectors.Controls.Add(this.lblDetectionRate, 1, 0);
            this.tlpDetectors.Controls.Add(this.lblSetThreshold, 2, 0);
            this.tlpDetectors.Controls.Add(this.lblTimeActive, 3, 0);
            this.tlpDetectors.Controls.Add(this.lblAbove01, 3, 1);
            this.tlpDetectors.Location = new System.Drawing.Point(76, 87);
            this.tlpDetectors.Margin = new System.Windows.Forms.Padding(0);
            this.tlpDetectors.Name = "tlpDetectors";
            this.tlpDetectors.RowCount = 18;
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpDetectors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDetectors.Size = new System.Drawing.Size(386, 382);
            this.tlpDetectors.TabIndex = 26;
            // 
            // lblFeature01
            // 
            this.lblFeature01.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFeature01.Location = new System.Drawing.Point(3, 33);
            this.lblFeature01.Name = "lblFeature01";
            this.lblFeature01.Size = new System.Drawing.Size(71, 13);
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
            this.lblDetectionRate01.Location = new System.Drawing.Point(80, 33);
            this.lblDetectionRate01.Name = "lblDetectionRate01";
            this.lblDetectionRate01.Size = new System.Drawing.Size(71, 13);
            this.lblDetectionRate01.TabIndex = 2;
            this.lblDetectionRate01.Text = "12.3456%";
            this.lblDetectionRate01.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numSetThreshold01
            // 
            this.numSetThreshold01.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.numSetThreshold01.DecimalPlaces = 4;
            this.numSetThreshold01.Location = new System.Drawing.Point(157, 30);
            this.numSetThreshold01.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.numSetThreshold01.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numSetThreshold01.Name = "numSetThreshold01";
            this.numSetThreshold01.Size = new System.Drawing.Size(71, 20);
            this.numSetThreshold01.TabIndex = 1;
            this.numSetThreshold01.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblDetectionRate
            // 
            this.lblDetectionRate.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblDetectionRate.AutoSize = true;
            this.lblDetectionRate.Location = new System.Drawing.Point(98, 1);
            this.lblDetectionRate.Name = "lblDetectionRate";
            this.lblDetectionRate.Size = new System.Drawing.Size(53, 26);
            this.lblDetectionRate.TabIndex = 5;
            this.lblDetectionRate.Text = "Detection Rate";
            this.lblDetectionRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSetThreshold
            // 
            this.lblSetThreshold.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblSetThreshold.AutoSize = true;
            this.lblSetThreshold.Location = new System.Drawing.Point(165, 1);
            this.lblSetThreshold.Name = "lblSetThreshold";
            this.lblSetThreshold.Size = new System.Drawing.Size(54, 26);
            this.lblSetThreshold.TabIndex = 4;
            this.lblSetThreshold.Text = "Set Threshold";
            this.lblSetThreshold.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTimeActive
            // 
            this.lblTimeActive.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblTimeActive.AutoSize = true;
            this.lblTimeActive.Location = new System.Drawing.Point(242, 8);
            this.lblTimeActive.Name = "lblTimeActive";
            this.lblTimeActive.Size = new System.Drawing.Size(63, 13);
            this.lblTimeActive.TabIndex = 6;
            this.lblTimeActive.Text = "Time Active";
            this.lblTimeActive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAbove01
            // 
            this.lblAbove01.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAbove01.Location = new System.Drawing.Point(234, 33);
            this.lblAbove01.Name = "lblAbove01";
            this.lblAbove01.Size = new System.Drawing.Size(71, 13);
            this.lblAbove01.TabIndex = 7;
            this.lblAbove01.Text = "00:00:00.000";
            this.lblAbove01.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDetectors
            // 
            this.lblDetectors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDetectors.AutoSize = true;
            this.lblDetectors.Location = new System.Drawing.Point(3, 87);
            this.lblDetectors.Name = "lblDetectors";
            this.lblDetectors.Size = new System.Drawing.Size(70, 13);
            this.lblDetectors.TabIndex = 25;
            this.lblDetectors.Text = "Detectors";
            // 
            // tlpTodo
            // 
            this.tlpTodo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpTodo.ColumnCount = 2;
            this.tlpTodo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpTodo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpTodo.Controls.Add(this.btnSetCaptureRegion, 0, 0);
            this.tlpTodo.Controls.Add(this.btnAutoSplitterSettings, 1, 0);
            this.tlpTodo.Location = new System.Drawing.Point(76, 58);
            this.tlpTodo.Margin = new System.Windows.Forms.Padding(0);
            this.tlpTodo.Name = "tlpTodo";
            this.tlpTodo.RowCount = 1;
            this.tlpTodo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTodo.Size = new System.Drawing.Size(386, 29);
            this.tlpTodo.TabIndex = 24;
            // 
            // btnAutoSplitterSettings
            // 
            this.btnAutoSplitterSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAutoSplitterSettings.Location = new System.Drawing.Point(196, 3);
            this.btnAutoSplitterSettings.Name = "btnAutoSplitterSettings";
            this.btnAutoSplitterSettings.Size = new System.Drawing.Size(187, 23);
            this.btnAutoSplitterSettings.TabIndex = 22;
            this.btnAutoSplitterSettings.Text = "Auto Splitter Settings";
            this.btnAutoSplitterSettings.UseVisualStyleBackColor = true;
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
            this.tlpCaptureDevice.Location = new System.Drawing.Point(76, 29);
            this.tlpCaptureDevice.Margin = new System.Windows.Forms.Padding(0);
            this.tlpCaptureDevice.Name = "tlpCaptureDevice";
            this.tlpCaptureDevice.RowCount = 1;
            this.tlpCaptureDevice.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCaptureDevice.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tlpCaptureDevice.Size = new System.Drawing.Size(386, 29);
            this.tlpCaptureDevice.TabIndex = 23;
            // 
            // tlpGameProfile
            // 
            this.tlpGameProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpGameProfile.ColumnCount = 2;
            this.tlpGameProfile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpGameProfile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpGameProfile.Controls.Add(this.btnGameProfile, 1, 0);
            this.tlpGameProfile.Controls.Add(this.txtGameProfile, 0, 0);
            this.tlpGameProfile.Location = new System.Drawing.Point(76, 0);
            this.tlpGameProfile.Margin = new System.Windows.Forms.Padding(0);
            this.tlpGameProfile.Name = "tlpGameProfile";
            this.tlpGameProfile.RowCount = 1;
            this.tlpGameProfile.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpGameProfile.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tlpGameProfile.Size = new System.Drawing.Size(386, 29);
            this.tlpGameProfile.TabIndex = 0;
            // 
            // lblTodo
            // 
            this.lblTodo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTodo.AutoSize = true;
            this.lblTodo.Location = new System.Drawing.Point(3, 66);
            this.lblTodo.Name = "lblTodo";
            this.lblTodo.Size = new System.Drawing.Size(70, 13);
            this.lblTodo.TabIndex = 24;
            this.lblTodo.Text = "Todo";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.checkboxStart);
            this.flowLayoutPanel2.Controls.Add(this.checkboxSplit);
            this.flowLayoutPanel2.Controls.Add(this.checkboxReset);
            this.flowLayoutPanel2.Controls.Add(this.lblGameVersion);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(79, 472);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(380, 23);
            this.flowLayoutPanel2.TabIndex = 23;
            // 
            // checkboxStart
            // 
            this.checkboxStart.Enabled = false;
            this.checkboxStart.Location = new System.Drawing.Point(3, 3);
            this.checkboxStart.Name = "checkboxStart";
            this.checkboxStart.Size = new System.Drawing.Size(48, 17);
            this.checkboxStart.TabIndex = 11;
            this.checkboxStart.Text = "Start";
            this.checkboxStart.UseVisualStyleBackColor = true;
            // 
            // checkboxSplit
            // 
            this.checkboxSplit.Enabled = false;
            this.checkboxSplit.Location = new System.Drawing.Point(57, 3);
            this.checkboxSplit.Name = "checkboxSplit";
            this.checkboxSplit.Size = new System.Drawing.Size(46, 17);
            this.checkboxSplit.TabIndex = 0;
            this.checkboxSplit.Text = "Split";
            this.checkboxSplit.UseVisualStyleBackColor = true;
            // 
            // checkboxReset
            // 
            this.checkboxReset.Enabled = false;
            this.checkboxReset.Location = new System.Drawing.Point(109, 3);
            this.checkboxReset.Name = "checkboxReset";
            this.checkboxReset.Size = new System.Drawing.Size(54, 17);
            this.checkboxReset.TabIndex = 0;
            this.checkboxReset.Text = "Reset";
            this.checkboxReset.UseVisualStyleBackColor = true;
            // 
            // lblGameVersion
            // 
            this.lblGameVersion.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblGameVersion.AutoEllipsis = true;
            this.lblGameVersion.Location = new System.Drawing.Point(169, 5);
            this.lblGameVersion.Name = "lblGameVersion";
            this.lblGameVersion.Size = new System.Drawing.Size(208, 13);
            this.lblGameVersion.TabIndex = 10;
            this.lblGameVersion.Text = "Game Version: 1.0";
            this.lblGameVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ComponentSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpCore);
            this.Name = "ComponentSettings";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.Size = new System.Drawing.Size(476, 512);
            this.tlpCore.ResumeLayout(false);
            this.tlpCore.PerformLayout();
            this.tlpDetectors.ResumeLayout(false);
            this.tlpDetectors.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSetThreshold01)).EndInit();
            this.tlpTodo.ResumeLayout(false);
            this.tlpCaptureDevice.ResumeLayout(false);
            this.tlpGameProfile.ResumeLayout(false);
            this.tlpGameProfile.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnCaptureDevice;
        private System.Windows.Forms.Label lblCaptureDevice;
        private System.Windows.Forms.ComboBox boxCaptureDevice;
        private System.Windows.Forms.TextBox txtGameProfile;
        private System.Windows.Forms.Label lblGameProfile;
        private System.Windows.Forms.Button btnSetCaptureRegion;
        private System.Windows.Forms.Button btnGameProfile;
        private System.Windows.Forms.TableLayoutPanel tlpCore;
        private System.Windows.Forms.TableLayoutPanel tlpGameProfile;
        private System.Windows.Forms.TableLayoutPanel tlpCaptureDevice;
        private System.Windows.Forms.Label lblTodo;
        private System.Windows.Forms.TableLayoutPanel tlpTodo;
        private System.Windows.Forms.Button btnAutoSplitterSettings;
        private System.Windows.Forms.Label lblDetectors;
        private System.Windows.Forms.TableLayoutPanel tlpDetectors;
        private System.Windows.Forms.Label lblFeature01;
        private System.Windows.Forms.Label lblFeature;
        private System.Windows.Forms.Label lblDetectionRate01;
        private System.Windows.Forms.NumericUpDown numSetThreshold01;
        private System.Windows.Forms.Label lblDetectionRate;
        private System.Windows.Forms.Label lblSetThreshold;
        private System.Windows.Forms.Label lblTimeActive;
        private System.Windows.Forms.Label lblAbove01;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.CheckBox checkboxStart;
        private System.Windows.Forms.CheckBox checkboxSplit;
        private System.Windows.Forms.CheckBox checkboxReset;
        private System.Windows.Forms.Label lblGameVersion;
    }
}
