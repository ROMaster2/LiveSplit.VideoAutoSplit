namespace LiveSplit.VAS.UI
{
    partial class Aligner
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
            this.NumHeight = new System.Windows.Forms.NumericUpDown();
            this.NumWidth = new System.Windows.Forms.NumericUpDown();
            this.NumY = new System.Windows.Forms.NumericUpDown();
            this.NumX = new System.Windows.Forms.NumericUpDown();
            this.Label_Y = new System.Windows.Forms.Label();
            this.Label_Height = new System.Windows.Forms.Label();
            this.Label_X = new System.Windows.Forms.Label();
            this.Label_Width = new System.Windows.Forms.Label();
            this.BtnTryAutoAlign = new System.Windows.Forms.Button();
            this.TLPCore = new System.Windows.Forms.TableLayoutPanel();
            this.ControlPanel = new System.Windows.Forms.Panel();
            this.CkbUseExtraPrecision = new System.Windows.Forms.CheckBox();
            this.LblDeltas = new System.Windows.Forms.Label();
            this.CkbViewDelta = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.DdlWatchZone = new System.Windows.Forms.ComboBox();
            this.BtnResetRegion = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.BtnRefreshFrame = new System.Windows.Forms.Button();
            this.ThumbnailBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.NumHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumX)).BeginInit();
            this.TLPCore.SuspendLayout();
            this.ControlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ThumbnailBox)).BeginInit();
            this.SuspendLayout();
            // 
            // NumHeight
            // 
            this.NumHeight.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumHeight.DecimalPlaces = 2;
            this.NumHeight.Location = new System.Drawing.Point(110, 133);
            this.NumHeight.Maximum = new decimal(new int[] {
            4320,
            0,
            0,
            0});
            this.NumHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumHeight.Name = "NumHeight";
            this.NumHeight.Size = new System.Drawing.Size(64, 20);
            this.NumHeight.TabIndex = 220;
            this.NumHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumHeight.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.NumHeight.Validated += new System.EventHandler(this.NumHeight_Validated);
            // 
            // NumWidth
            // 
            this.NumWidth.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumWidth.DecimalPlaces = 2;
            this.NumWidth.Location = new System.Drawing.Point(26, 133);
            this.NumWidth.Maximum = new decimal(new int[] {
            7680,
            0,
            0,
            0});
            this.NumWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumWidth.Name = "NumWidth";
            this.NumWidth.Size = new System.Drawing.Size(64, 20);
            this.NumWidth.TabIndex = 219;
            this.NumWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumWidth.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.NumWidth.Validated += new System.EventHandler(this.NumWidth_Validated);
            // 
            // NumY
            // 
            this.NumY.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumY.DecimalPlaces = 2;
            this.NumY.Location = new System.Drawing.Point(110, 87);
            this.NumY.Maximum = new decimal(new int[] {
            4320,
            0,
            0,
            0});
            this.NumY.Minimum = new decimal(new int[] {
            4320,
            0,
            0,
            -2147483648});
            this.NumY.Name = "NumY";
            this.NumY.Size = new System.Drawing.Size(64, 20);
            this.NumY.TabIndex = 218;
            this.NumY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumY.Validated += new System.EventHandler(this.NumY_Validated);
            // 
            // NumX
            // 
            this.NumX.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NumX.DecimalPlaces = 2;
            this.NumX.Location = new System.Drawing.Point(26, 87);
            this.NumX.Maximum = new decimal(new int[] {
            7680,
            0,
            0,
            0});
            this.NumX.Minimum = new decimal(new int[] {
            7680,
            0,
            0,
            -2147483648});
            this.NumX.Name = "NumX";
            this.NumX.Size = new System.Drawing.Size(64, 20);
            this.NumX.TabIndex = 217;
            this.NumX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumX.Validated += new System.EventHandler(this.NumX_Validated);
            // 
            // Label_Y
            // 
            this.Label_Y.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Label_Y.AutoSize = true;
            this.Label_Y.Location = new System.Drawing.Point(136, 71);
            this.Label_Y.Name = "Label_Y";
            this.Label_Y.Size = new System.Drawing.Size(12, 13);
            this.Label_Y.TabIndex = 223;
            this.Label_Y.Text = "y";
            // 
            // Label_Height
            // 
            this.Label_Height.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Label_Height.AutoSize = true;
            this.Label_Height.Location = new System.Drawing.Point(123, 117);
            this.Label_Height.Name = "Label_Height";
            this.Label_Height.Size = new System.Drawing.Size(38, 13);
            this.Label_Height.TabIndex = 226;
            this.Label_Height.Text = "Height";
            // 
            // Label_X
            // 
            this.Label_X.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Label_X.AutoSize = true;
            this.Label_X.Location = new System.Drawing.Point(54, 71);
            this.Label_X.Name = "Label_X";
            this.Label_X.Size = new System.Drawing.Size(12, 13);
            this.Label_X.TabIndex = 224;
            this.Label_X.Text = "x";
            // 
            // Label_Width
            // 
            this.Label_Width.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Label_Width.AutoSize = true;
            this.Label_Width.Location = new System.Drawing.Point(41, 117);
            this.Label_Width.Name = "Label_Width";
            this.Label_Width.Size = new System.Drawing.Size(35, 13);
            this.Label_Width.TabIndex = 225;
            this.Label_Width.Text = "Width";
            // 
            // BtnTryAutoAlign
            // 
            this.BtnTryAutoAlign.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BtnTryAutoAlign.Location = new System.Drawing.Point(50, 41);
            this.BtnTryAutoAlign.Name = "BtnTryAutoAlign";
            this.BtnTryAutoAlign.Size = new System.Drawing.Size(100, 23);
            this.BtnTryAutoAlign.TabIndex = 227;
            this.BtnTryAutoAlign.Text = "Try Auto-Align";
            this.BtnTryAutoAlign.UseVisualStyleBackColor = true;
            this.BtnTryAutoAlign.Click += new System.EventHandler(this.BtnTryAutoAlign_Click);
            // 
            // TLPCore
            // 
            this.TLPCore.ColumnCount = 2;
            this.TLPCore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLPCore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.TLPCore.Controls.Add(this.ControlPanel, 1, 0);
            this.TLPCore.Controls.Add(this.ThumbnailBox, 0, 0);
            this.TLPCore.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLPCore.Location = new System.Drawing.Point(0, 0);
            this.TLPCore.Margin = new System.Windows.Forms.Padding(5);
            this.TLPCore.Name = "TLPCore";
            this.TLPCore.RowCount = 1;
            this.TLPCore.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLPCore.Size = new System.Drawing.Size(1010, 762);
            this.TLPCore.TabIndex = 233;
            // 
            // ControlPanel
            // 
            this.ControlPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ControlPanel.AutoScroll = true;
            this.ControlPanel.Controls.Add(this.CkbUseExtraPrecision);
            this.ControlPanel.Controls.Add(this.LblDeltas);
            this.ControlPanel.Controls.Add(this.CkbViewDelta);
            this.ControlPanel.Controls.Add(this.label4);
            this.ControlPanel.Controls.Add(this.DdlWatchZone);
            this.ControlPanel.Controls.Add(this.BtnResetRegion);
            this.ControlPanel.Controls.Add(this.label3);
            this.ControlPanel.Controls.Add(this.label2);
            this.ControlPanel.Controls.Add(this.label1);
            this.ControlPanel.Controls.Add(this.BtnRefreshFrame);
            this.ControlPanel.Controls.Add(this.Label_Width);
            this.ControlPanel.Controls.Add(this.Label_X);
            this.ControlPanel.Controls.Add(this.Label_Height);
            this.ControlPanel.Controls.Add(this.Label_Y);
            this.ControlPanel.Controls.Add(this.NumX);
            this.ControlPanel.Controls.Add(this.BtnTryAutoAlign);
            this.ControlPanel.Controls.Add(this.NumY);
            this.ControlPanel.Controls.Add(this.NumHeight);
            this.ControlPanel.Controls.Add(this.NumWidth);
            this.ControlPanel.Location = new System.Drawing.Point(810, 0);
            this.ControlPanel.Margin = new System.Windows.Forms.Padding(0);
            this.ControlPanel.Name = "ControlPanel";
            this.ControlPanel.Size = new System.Drawing.Size(200, 762);
            this.ControlPanel.TabIndex = 0;
            // 
            // CkbUseExtraPrecision
            // 
            this.CkbUseExtraPrecision.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CkbUseExtraPrecision.AutoSize = true;
            this.CkbUseExtraPrecision.Location = new System.Drawing.Point(43, 159);
            this.CkbUseExtraPrecision.Name = "CkbUseExtraPrecision";
            this.CkbUseExtraPrecision.Size = new System.Drawing.Size(118, 17);
            this.CkbUseExtraPrecision.TabIndex = 244;
            this.CkbUseExtraPrecision.Text = "Use Extra Precision";
            this.CkbUseExtraPrecision.UseVisualStyleBackColor = true;
            this.CkbUseExtraPrecision.CheckedChanged += new System.EventHandler(this.CkbUseExtraPrecision_CheckedChanged);
            // 
            // LblDeltas
            // 
            this.LblDeltas.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LblDeltas.Location = new System.Drawing.Point(94, 637);
            this.LblDeltas.Name = "LblDeltas";
            this.LblDeltas.Size = new System.Drawing.Size(106, 116);
            this.LblDeltas.TabIndex = 243;
            this.LblDeltas.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // CkbViewDelta
            // 
            this.CkbViewDelta.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CkbViewDelta.AutoSize = true;
            this.CkbViewDelta.Location = new System.Drawing.Point(11, 637);
            this.CkbViewDelta.Name = "CkbViewDelta";
            this.CkbViewDelta.Size = new System.Drawing.Size(77, 17);
            this.CkbViewDelta.TabIndex = 242;
            this.CkbViewDelta.Text = "View Delta";
            this.CkbViewDelta.UseVisualStyleBackColor = true;
            this.CkbViewDelta.CheckedChanged += new System.EventHandler(this.CkbViewDelta_CheckedChanged);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(48, 594);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 13);
            this.label4.TabIndex = 241;
            this.label4.Text = "Check WatchZone";
            // 
            // DdlWatchZone
            // 
            this.DdlWatchZone.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.DdlWatchZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DdlWatchZone.FormattingEnabled = true;
            this.DdlWatchZone.Location = new System.Drawing.Point(10, 610);
            this.DdlWatchZone.Name = "DdlWatchZone";
            this.DdlWatchZone.Size = new System.Drawing.Size(180, 21);
            this.DdlWatchZone.TabIndex = 240;
            this.DdlWatchZone.SelectedIndexChanged += new System.EventHandler(this.DdlWatchZone_SelectedIndexChanged);
            // 
            // BtnResetRegion
            // 
            this.BtnResetRegion.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BtnResetRegion.Location = new System.Drawing.Point(50, 557);
            this.BtnResetRegion.Name = "BtnResetRegion";
            this.BtnResetRegion.Size = new System.Drawing.Size(96, 23);
            this.BtnResetRegion.TabIndex = 239;
            this.BtnResetRegion.Text = "Reset Region";
            this.BtnResetRegion.UseVisualStyleBackColor = true;
            this.BtnResetRegion.Click += new System.EventHandler(this.BtnResetRegion_Click);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.Location = new System.Drawing.Point(8, 414);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(180, 16);
            this.label3.TabIndex = 238;
            this.label3.Text = "Shrink Capture Region";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.Location = new System.Drawing.Point(8, 297);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(180, 16);
            this.label2.TabIndex = 236;
            this.label2.Text = "Grow Capture Region";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.Location = new System.Drawing.Point(8, 180);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(180, 16);
            this.label1.TabIndex = 234;
            this.label1.Text = "Move Capture Region";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BtnRefreshFrame
            // 
            this.BtnRefreshFrame.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.BtnRefreshFrame.Location = new System.Drawing.Point(50, 12);
            this.BtnRefreshFrame.Name = "BtnRefreshFrame";
            this.BtnRefreshFrame.Size = new System.Drawing.Size(100, 23);
            this.BtnRefreshFrame.TabIndex = 233;
            this.BtnRefreshFrame.Text = "Refresh Frame";
            this.BtnRefreshFrame.UseVisualStyleBackColor = true;
            this.BtnRefreshFrame.Click += new System.EventHandler(this.BtnRefreshFrame_Click);
            // 
            // ThumbnailBox
            // 
            this.ThumbnailBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ThumbnailBox.Location = new System.Drawing.Point(85, 141);
            this.ThumbnailBox.Margin = new System.Windows.Forms.Padding(5);
            this.ThumbnailBox.Name = "ThumbnailBox";
            this.ThumbnailBox.Size = new System.Drawing.Size(640, 480);
            this.ThumbnailBox.TabIndex = 232;
            this.ThumbnailBox.TabStop = false;
            // 
            // Aligner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1010, 762);
            this.Controls.Add(this.TLPCore);
            this.Name = "Aligner";
            this.Text = "Aligner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Aligner_FormClosing);
            this.ResizeEnd += new System.EventHandler(this.Aligner_ResizeEnd);
            ((System.ComponentModel.ISupportInitialize)(this.NumHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumX)).EndInit();
            this.TLPCore.ResumeLayout(false);
            this.ControlPanel.ResumeLayout(false);
            this.ControlPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ThumbnailBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown NumHeight;
        private System.Windows.Forms.NumericUpDown NumWidth;
        private System.Windows.Forms.NumericUpDown NumY;
        private System.Windows.Forms.NumericUpDown NumX;
        private System.Windows.Forms.Label Label_Y;
        private System.Windows.Forms.Label Label_Height;
        private System.Windows.Forms.Label Label_X;
        private System.Windows.Forms.Label Label_Width;
        private System.Windows.Forms.Button BtnTryAutoAlign;
        private System.Windows.Forms.PictureBox ThumbnailBox;
        private System.Windows.Forms.TableLayoutPanel TLPCore;
        private System.Windows.Forms.Panel ControlPanel;
        private System.Windows.Forms.Button BtnRefreshFrame;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button BtnResetRegion;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox DdlWatchZone;
        private System.Windows.Forms.CheckBox CkbViewDelta;
        private System.Windows.Forms.Label LblDeltas;
        private System.Windows.Forms.CheckBox CkbUseExtraPrecision;
    }
}