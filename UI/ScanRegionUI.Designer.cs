namespace LiveSplit.UI.Components
{
    partial class ScanRegionUI
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblScreen = new System.Windows.Forms.Label();
            this.boxScreen = new System.Windows.Forms.ComboBox();
            this.lblDelta = new System.Windows.Forms.Label();
            this.ckbShowComparison = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.boxPreviewType = new System.Windows.Forms.ComboBox();
            this.boxPreviewFeature = new System.Windows.Forms.ComboBox();
            this.lblPreviewType = new System.Windows.Forms.Label();
            this.lblPreviewFeature = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblHeight = new System.Windows.Forms.Label();
            this.lblWidth = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.numX = new System.Windows.Forms.NumericUpDown();
            this.numHeight = new System.Windows.Forms.NumericUpDown();
            this.numY = new System.Windows.Forms.NumericUpDown();
            this.numWidth = new System.Windows.Forms.NumericUpDown();
            this.lblX = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.tlpCore.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpCore
            // 
            this.tlpCore.ColumnCount = 1;
            this.tlpCore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCore.Controls.Add(this.panel1, 0, 0);
            this.tlpCore.Controls.Add(this.pictureBox, 0, 1);
            this.tlpCore.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpCore.Location = new System.Drawing.Point(7, 7);
            this.tlpCore.Name = "tlpCore";
            this.tlpCore.RowCount = 2;
            this.tlpCore.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpCore.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCore.Size = new System.Drawing.Size(454, 472);
            this.tlpCore.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblScreen);
            this.panel1.Controls.Add(this.boxScreen);
            this.panel1.Controls.Add(this.lblDelta);
            this.panel1.Controls.Add(this.ckbShowComparison);
            this.panel1.Controls.Add(this.tableLayoutPanel2);
            this.panel1.Controls.Add(this.btnReset);
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(454, 100);
            this.panel1.TabIndex = 0;
            // 
            // lblScreen
            // 
            this.lblScreen.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblScreen.AutoSize = true;
            this.lblScreen.Location = new System.Drawing.Point(8, 3);
            this.lblScreen.Name = "lblScreen";
            this.lblScreen.Size = new System.Drawing.Size(72, 13);
            this.lblScreen.TabIndex = 230;
            this.lblScreen.Text = "Game Screen";
            this.lblScreen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblScreen.Visible = false;
            // 
            // boxScreen
            // 
            this.boxScreen.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.boxScreen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.boxScreen.Enabled = false;
            this.boxScreen.FormattingEnabled = true;
            this.boxScreen.Location = new System.Drawing.Point(3, 19);
            this.boxScreen.Name = "boxScreen";
            this.boxScreen.Size = new System.Drawing.Size(80, 21);
            this.boxScreen.TabIndex = 229;
            this.boxScreen.Visible = false;
            // 
            // lblDelta
            // 
            this.lblDelta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDelta.Location = new System.Drawing.Point(347, 77);
            this.lblDelta.Name = "lblDelta";
            this.lblDelta.Size = new System.Drawing.Size(100, 16);
            this.lblDelta.TabIndex = 228;
            this.lblDelta.Text = "12.3456%";
            this.lblDelta.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ckbShowComparison
            // 
            this.ckbShowComparison.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ckbShowComparison.AutoSize = true;
            this.ckbShowComparison.Enabled = false;
            this.ckbShowComparison.Location = new System.Drawing.Point(230, 76);
            this.ckbShowComparison.Name = "ckbShowComparison";
            this.ckbShowComparison.Size = new System.Drawing.Size(111, 17);
            this.ckbShowComparison.TabIndex = 227;
            this.ckbShowComparison.Text = "Show Comparison";
            this.ckbShowComparison.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.boxPreviewType, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.boxPreviewFeature, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblPreviewType, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblPreviewFeature, 0, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 44);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(227, 55);
            this.tableLayoutPanel2.TabIndex = 226;
            // 
            // boxPreviewType
            // 
            this.boxPreviewType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.boxPreviewType.FormattingEnabled = true;
            this.boxPreviewType.Location = new System.Drawing.Point(103, 3);
            this.boxPreviewType.Name = "boxPreviewType";
            this.boxPreviewType.Size = new System.Drawing.Size(121, 21);
            this.boxPreviewType.TabIndex = 224;
            // 
            // boxPreviewFeature
            // 
            this.boxPreviewFeature.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.boxPreviewFeature.Enabled = false;
            this.boxPreviewFeature.FormattingEnabled = true;
            this.boxPreviewFeature.Location = new System.Drawing.Point(103, 30);
            this.boxPreviewFeature.Name = "boxPreviewFeature";
            this.boxPreviewFeature.Size = new System.Drawing.Size(121, 21);
            this.boxPreviewFeature.TabIndex = 225;
            // 
            // lblPreviewType
            // 
            this.lblPreviewType.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPreviewType.AutoSize = true;
            this.lblPreviewType.Location = new System.Drawing.Point(25, 7);
            this.lblPreviewType.Name = "lblPreviewType";
            this.lblPreviewType.Size = new System.Drawing.Size(72, 13);
            this.lblPreviewType.TabIndex = 226;
            this.lblPreviewType.Text = "Preview Type";
            // 
            // lblPreviewFeature
            // 
            this.lblPreviewFeature.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPreviewFeature.AutoSize = true;
            this.lblPreviewFeature.Location = new System.Drawing.Point(13, 34);
            this.lblPreviewFeature.Name = "lblPreviewFeature";
            this.lblPreviewFeature.Size = new System.Drawing.Size(84, 13);
            this.lblPreviewFeature.TabIndex = 227;
            this.lblPreviewFeature.Text = "Preview Feature";
            // 
            // btnReset
            // 
            this.btnReset.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnReset.Location = new System.Drawing.Point(372, 17);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 223;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.lblHeight, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblWidth, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblY, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.numX, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.numHeight, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.numY, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.numWidth, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblX, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(86, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(280, 42);
            this.tableLayoutPanel1.TabIndex = 222;
            // 
            // lblHeight
            // 
            this.lblHeight.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHeight.AutoSize = true;
            this.lblHeight.Location = new System.Drawing.Point(226, 0);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(38, 13);
            this.lblHeight.TabIndex = 225;
            this.lblHeight.Text = "Height";
            this.lblHeight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblWidth
            // 
            this.lblWidth.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblWidth.AutoSize = true;
            this.lblWidth.Location = new System.Drawing.Point(157, 0);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(35, 13);
            this.lblWidth.TabIndex = 224;
            this.lblWidth.Text = "Width";
            this.lblWidth.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblY
            // 
            this.lblY.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(98, 0);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(14, 13);
            this.lblY.TabIndex = 223;
            this.lblY.Text = "Y";
            this.lblY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numX
            // 
            this.numX.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.numX.DecimalPlaces = 2;
            this.numX.Location = new System.Drawing.Point(3, 17);
            this.numX.Maximum = new decimal(new int[] {
            7680,
            0,
            0,
            0});
            this.numX.Minimum = new decimal(new int[] {
            7680,
            0,
            0,
            -2147483648});
            this.numX.Name = "numX";
            this.numX.Size = new System.Drawing.Size(64, 20);
            this.numX.TabIndex = 218;
            this.numX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // numHeight
            // 
            this.numHeight.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.numHeight.DecimalPlaces = 2;
            this.numHeight.Location = new System.Drawing.Point(213, 17);
            this.numHeight.Maximum = new decimal(new int[] {
            7680,
            0,
            0,
            0});
            this.numHeight.Minimum = new decimal(new int[] {
            7680,
            0,
            0,
            -2147483648});
            this.numHeight.Name = "numHeight";
            this.numHeight.Size = new System.Drawing.Size(64, 20);
            this.numHeight.TabIndex = 221;
            this.numHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // numY
            // 
            this.numY.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.numY.DecimalPlaces = 2;
            this.numY.Location = new System.Drawing.Point(73, 17);
            this.numY.Maximum = new decimal(new int[] {
            7680,
            0,
            0,
            0});
            this.numY.Minimum = new decimal(new int[] {
            7680,
            0,
            0,
            -2147483648});
            this.numY.Name = "numY";
            this.numY.Size = new System.Drawing.Size(64, 20);
            this.numY.TabIndex = 219;
            this.numY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // numWidth
            // 
            this.numWidth.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.numWidth.DecimalPlaces = 2;
            this.numWidth.Location = new System.Drawing.Point(143, 17);
            this.numWidth.Maximum = new decimal(new int[] {
            7680,
            0,
            0,
            0});
            this.numWidth.Minimum = new decimal(new int[] {
            7680,
            0,
            0,
            -2147483648});
            this.numWidth.Name = "numWidth";
            this.numWidth.Size = new System.Drawing.Size(64, 20);
            this.numWidth.TabIndex = 220;
            this.numWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblX
            // 
            this.lblX.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(28, 0);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(14, 13);
            this.lblX.TabIndex = 222;
            this.lblX.Text = "X";
            this.lblX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox.Location = new System.Drawing.Point(27, 136);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(5);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(400, 300);
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            // 
            // ScanRegion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpCore);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ScanRegion";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.Size = new System.Drawing.Size(468, 486);
            this.tlpCore.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpCore;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.NumericUpDown numX;
        private System.Windows.Forms.NumericUpDown numHeight;
        private System.Windows.Forms.NumericUpDown numY;
        private System.Windows.Forms.NumericUpDown numWidth;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.ComboBox boxPreviewFeature;
        private System.Windows.Forms.ComboBox boxPreviewType;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lblDelta;
        private System.Windows.Forms.CheckBox ckbShowComparison;
        private System.Windows.Forms.Label lblPreviewType;
        private System.Windows.Forms.Label lblPreviewFeature;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label lblScreen;
        private System.Windows.Forms.ComboBox boxScreen;
    }
}
