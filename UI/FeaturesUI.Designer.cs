namespace LiveSplit.VAS.UI
{
    partial class FeaturesUI : AbstractUI
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
            this.tlpFeatures = new System.Windows.Forms.TableLayoutPanel();
            this.tlpFeatureHeader = new System.Windows.Forms.TableLayoutPanel();
            this.lblHeaderActive = new System.Windows.Forms.Label();
            this.lblHeaderMinimum1 = new System.Windows.Forms.Label();
            this.lblHeaderMaximum1 = new System.Windows.Forms.Label();
            this.lblHeaderValue1 = new System.Windows.Forms.Label();
            this.lblHeaderFeature = new System.Windows.Forms.Label();
            this.tlpFeatures.SuspendLayout();
            this.tlpFeatureHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpFeatures
            // 
            this.tlpFeatures.ColumnCount = 1;
            this.tlpFeatures.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFeatures.Controls.Add(this.tlpFeatureHeader, 0, 0);
            this.tlpFeatures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpFeatures.Location = new System.Drawing.Point(7, 7);
            this.tlpFeatures.Margin = new System.Windows.Forms.Padding(0);
            this.tlpFeatures.Name = "tlpFeatures";
            this.tlpFeatures.RowCount = 2;
            this.tlpFeatures.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tlpFeatures.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFeatures.Size = new System.Drawing.Size(454, 472);
            this.tlpFeatures.TabIndex = 27;
            // 
            // tlpFeatureHeader
            // 
            this.tlpFeatureHeader.ColumnCount = 5;
            this.tlpFeatureHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFeatureHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tlpFeatureHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tlpFeatureHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tlpFeatureHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tlpFeatureHeader.Controls.Add(this.lblHeaderActive, 0, 0);
            this.tlpFeatureHeader.Controls.Add(this.lblHeaderMinimum1, 4, 0);
            this.tlpFeatureHeader.Controls.Add(this.lblHeaderMaximum1, 3, 0);
            this.tlpFeatureHeader.Controls.Add(this.lblHeaderValue1, 2, 0);
            this.tlpFeatureHeader.Controls.Add(this.lblHeaderFeature, 0, 0);
            this.tlpFeatureHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpFeatureHeader.Location = new System.Drawing.Point(0, 0);
            this.tlpFeatureHeader.Margin = new System.Windows.Forms.Padding(0);
            this.tlpFeatureHeader.Name = "tlpFeatureHeader";
            this.tlpFeatureHeader.RowCount = 1;
            this.tlpFeatureHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFeatureHeader.Size = new System.Drawing.Size(454, 29);
            this.tlpFeatureHeader.TabIndex = 30;
            // 
            // lblHeaderActive
            // 
            this.lblHeaderActive.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHeaderActive.AutoSize = true;
            this.lblHeaderActive.Location = new System.Drawing.Point(209, 8);
            this.lblHeaderActive.Name = "lblHeaderActive";
            this.lblHeaderActive.Size = new System.Drawing.Size(37, 13);
            this.lblHeaderActive.TabIndex = 31;
            this.lblHeaderActive.Text = "Active";
            this.lblHeaderActive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblHeaderActive.Visible = false;
            // 
            // lblHeaderMinimum1
            // 
            this.lblHeaderMinimum1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblHeaderMinimum1.AutoSize = true;
            this.lblHeaderMinimum1.Location = new System.Drawing.Point(403, 8);
            this.lblHeaderMinimum1.Name = "lblHeaderMinimum1";
            this.lblHeaderMinimum1.Size = new System.Drawing.Size(48, 13);
            this.lblHeaderMinimum1.TabIndex = 30;
            this.lblHeaderMinimum1.Text = "Minimum";
            this.lblHeaderMinimum1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblHeaderMaximum1
            // 
            this.lblHeaderMaximum1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblHeaderMaximum1.AutoSize = true;
            this.lblHeaderMaximum1.Location = new System.Drawing.Point(332, 8);
            this.lblHeaderMaximum1.Name = "lblHeaderMaximum1";
            this.lblHeaderMaximum1.Size = new System.Drawing.Size(51, 13);
            this.lblHeaderMaximum1.TabIndex = 30;
            this.lblHeaderMaximum1.Text = "Maximum";
            this.lblHeaderMaximum1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblHeaderValue1
            // 
            this.lblHeaderValue1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblHeaderValue1.AutoSize = true;
            this.lblHeaderValue1.Location = new System.Drawing.Point(281, 8);
            this.lblHeaderValue1.Name = "lblHeaderValue1";
            this.lblHeaderValue1.Size = new System.Drawing.Size(34, 13);
            this.lblHeaderValue1.TabIndex = 30;
            this.lblHeaderValue1.Text = "Value";
            this.lblHeaderValue1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblHeaderFeature
            // 
            this.lblHeaderFeature.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblHeaderFeature.AutoSize = true;
            this.lblHeaderFeature.Location = new System.Drawing.Point(3, 8);
            this.lblHeaderFeature.Name = "lblHeaderFeature";
            this.lblHeaderFeature.Size = new System.Drawing.Size(45, 13);
            this.lblHeaderFeature.TabIndex = 30;
            this.lblHeaderFeature.Text = "Variable";
            this.lblHeaderFeature.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FeaturesUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.tlpFeatures);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "FeaturesUI";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.Size = new System.Drawing.Size(468, 486);
            this.tlpFeatures.ResumeLayout(false);
            this.tlpFeatureHeader.ResumeLayout(false);
            this.tlpFeatureHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpFeatures;
        private System.Windows.Forms.TableLayoutPanel tlpFeatureHeader;
        private System.Windows.Forms.Label lblHeaderMinimum1;
        private System.Windows.Forms.Label lblHeaderMaximum1;
        private System.Windows.Forms.Label lblHeaderValue1;
        private System.Windows.Forms.Label lblHeaderFeature;
        private System.Windows.Forms.Label lblHeaderActive;
    }
}
