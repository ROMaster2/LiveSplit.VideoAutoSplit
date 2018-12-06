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
            this.tlpCore = new System.Windows.Forms.TableLayoutPanel();
            this.tlpVariables = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblHeaderMinimum2 = new System.Windows.Forms.Label();
            this.lblHeaderMaximum2 = new System.Windows.Forms.Label();
            this.lblHeaderValue2 = new System.Windows.Forms.Label();
            this.lblHeaderVariable = new System.Windows.Forms.Label();
            this.tlpFeatures.SuspendLayout();
            this.tlpFeatureHeader.SuspendLayout();
            this.tlpCore.SuspendLayout();
            this.tlpVariables.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpFeatures
            // 
            this.tlpFeatures.ColumnCount = 1;
            this.tlpFeatures.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFeatures.Controls.Add(this.tlpFeatureHeader, 0, 0);
            this.tlpFeatures.Location = new System.Drawing.Point(0, 0);
            this.tlpFeatures.Margin = new System.Windows.Forms.Padding(0);
            this.tlpFeatures.Name = "tlpFeatures";
            this.tlpFeatures.RowCount = 2;
            this.tlpFeatures.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tlpFeatures.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFeatures.Size = new System.Drawing.Size(451, 234);
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
            this.tlpFeatureHeader.Size = new System.Drawing.Size(451, 29);
            this.tlpFeatureHeader.TabIndex = 30;
            // 
            // lblHeaderActive
            // 
            this.lblHeaderActive.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHeaderActive.AutoSize = true;
            this.lblHeaderActive.Location = new System.Drawing.Point(206, 8);
            this.lblHeaderActive.Name = "lblHeaderActive";
            this.lblHeaderActive.Size = new System.Drawing.Size(37, 13);
            this.lblHeaderActive.TabIndex = 31;
            this.lblHeaderActive.Text = "Active";
            this.lblHeaderActive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblHeaderMinimum1
            // 
            this.lblHeaderMinimum1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblHeaderMinimum1.AutoSize = true;
            this.lblHeaderMinimum1.Location = new System.Drawing.Point(400, 8);
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
            this.lblHeaderMaximum1.Location = new System.Drawing.Point(329, 8);
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
            this.lblHeaderValue1.Location = new System.Drawing.Point(278, 8);
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
            this.lblHeaderFeature.Size = new System.Drawing.Size(43, 13);
            this.lblHeaderFeature.TabIndex = 30;
            this.lblHeaderFeature.Text = "Feature";
            this.lblHeaderFeature.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tlpCore
            // 
            this.tlpCore.ColumnCount = 1;
            this.tlpCore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCore.Controls.Add(this.tlpVariables, 0, 1);
            this.tlpCore.Controls.Add(this.tlpFeatures, 0, 0);
            this.tlpCore.Location = new System.Drawing.Point(10, 10);
            this.tlpCore.Margin = new System.Windows.Forms.Padding(0);
            this.tlpCore.Name = "tlpCore";
            this.tlpCore.RowCount = 2;
            this.tlpCore.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCore.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCore.Size = new System.Drawing.Size(451, 469);
            this.tlpCore.TabIndex = 28;
            // 
            // tlpVariables
            // 
            this.tlpVariables.ColumnCount = 1;
            this.tlpVariables.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpVariables.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tlpVariables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpVariables.Location = new System.Drawing.Point(0, 234);
            this.tlpVariables.Margin = new System.Windows.Forms.Padding(0);
            this.tlpVariables.Name = "tlpVariables";
            this.tlpVariables.RowCount = 2;
            this.tlpVariables.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tlpVariables.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpVariables.Size = new System.Drawing.Size(451, 235);
            this.tlpVariables.TabIndex = 28;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.lblHeaderMinimum2, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblHeaderMaximum2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblHeaderValue2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblHeaderVariable, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(451, 29);
            this.tableLayoutPanel1.TabIndex = 31;
            // 
            // lblHeaderMinimum2
            // 
            this.lblHeaderMinimum2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblHeaderMinimum2.AutoSize = true;
            this.lblHeaderMinimum2.Location = new System.Drawing.Point(400, 8);
            this.lblHeaderMinimum2.Name = "lblHeaderMinimum2";
            this.lblHeaderMinimum2.Size = new System.Drawing.Size(48, 13);
            this.lblHeaderMinimum2.TabIndex = 30;
            this.lblHeaderMinimum2.Text = "Minimum";
            this.lblHeaderMinimum2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblHeaderMaximum2
            // 
            this.lblHeaderMaximum2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblHeaderMaximum2.AutoSize = true;
            this.lblHeaderMaximum2.Location = new System.Drawing.Point(329, 8);
            this.lblHeaderMaximum2.Name = "lblHeaderMaximum2";
            this.lblHeaderMaximum2.Size = new System.Drawing.Size(51, 13);
            this.lblHeaderMaximum2.TabIndex = 30;
            this.lblHeaderMaximum2.Text = "Maximum";
            this.lblHeaderMaximum2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblHeaderValue2
            // 
            this.lblHeaderValue2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblHeaderValue2.AutoSize = true;
            this.lblHeaderValue2.Location = new System.Drawing.Point(278, 8);
            this.lblHeaderValue2.Name = "lblHeaderValue2";
            this.lblHeaderValue2.Size = new System.Drawing.Size(34, 13);
            this.lblHeaderValue2.TabIndex = 30;
            this.lblHeaderValue2.Text = "Value";
            this.lblHeaderValue2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblHeaderVariable
            // 
            this.lblHeaderVariable.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblHeaderVariable.AutoSize = true;
            this.lblHeaderVariable.Location = new System.Drawing.Point(3, 8);
            this.lblHeaderVariable.Name = "lblHeaderVariable";
            this.lblHeaderVariable.Size = new System.Drawing.Size(45, 13);
            this.lblHeaderVariable.TabIndex = 30;
            this.lblHeaderVariable.Text = "Variable";
            this.lblHeaderVariable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FeaturesUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpCore);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "FeaturesUI";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.Size = new System.Drawing.Size(468, 486);
            this.tlpFeatures.ResumeLayout(false);
            this.tlpFeatureHeader.ResumeLayout(false);
            this.tlpFeatureHeader.PerformLayout();
            this.tlpCore.ResumeLayout(false);
            this.tlpVariables.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpFeatures;
        private System.Windows.Forms.TableLayoutPanel tlpCore;
        private System.Windows.Forms.TableLayoutPanel tlpVariables;
        private System.Windows.Forms.TableLayoutPanel tlpFeatureHeader;
        private System.Windows.Forms.Label lblHeaderMinimum1;
        private System.Windows.Forms.Label lblHeaderMaximum1;
        private System.Windows.Forms.Label lblHeaderValue1;
        private System.Windows.Forms.Label lblHeaderFeature;
        private System.Windows.Forms.Label lblHeaderActive;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblHeaderMinimum2;
        private System.Windows.Forms.Label lblHeaderMaximum2;
        private System.Windows.Forms.Label lblHeaderValue2;
        private System.Windows.Forms.Label lblHeaderVariable;
    }
}
