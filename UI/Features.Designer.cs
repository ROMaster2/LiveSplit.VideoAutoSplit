namespace LiveSplit.UI.Components
{
    partial class FeaturesUI
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
            this.lblMinimum1 = new System.Windows.Forms.Label();
            this.lblMaximum1 = new System.Windows.Forms.Label();
            this.lblFeature = new System.Windows.Forms.Label();
            this.lblActive = new System.Windows.Forms.Label();
            this.lblConfidence = new System.Windows.Forms.Label();
            this.tlpCore = new System.Windows.Forms.TableLayoutPanel();
            this.tlpVariables = new System.Windows.Forms.TableLayoutPanel();
            this.lblVariable = new System.Windows.Forms.Label();
            this.lblValue = new System.Windows.Forms.Label();
            this.lblMaximum2 = new System.Windows.Forms.Label();
            this.lblMinimum2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tlpFeatures.SuspendLayout();
            this.tlpCore.SuspendLayout();
            this.tlpVariables.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpFeatures
            // 
            this.tlpFeatures.ColumnCount = 5;
            this.tlpFeatures.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFeatures.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpFeatures.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tlpFeatures.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tlpFeatures.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tlpFeatures.Controls.Add(this.label4, 4, 1);
            this.tlpFeatures.Controls.Add(this.label3, 3, 1);
            this.tlpFeatures.Controls.Add(this.lblMinimum1, 4, 0);
            this.tlpFeatures.Controls.Add(this.lblMaximum1, 3, 0);
            this.tlpFeatures.Controls.Add(this.lblFeature, 0, 0);
            this.tlpFeatures.Controls.Add(this.label1, 0, 1);
            this.tlpFeatures.Controls.Add(this.checkBox1, 1, 1);
            this.tlpFeatures.Controls.Add(this.lblActive, 1, 0);
            this.tlpFeatures.Controls.Add(this.lblConfidence, 2, 0);
            this.tlpFeatures.Controls.Add(this.label2, 2, 1);
            this.tlpFeatures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpFeatures.Location = new System.Drawing.Point(0, 0);
            this.tlpFeatures.Margin = new System.Windows.Forms.Padding(0);
            this.tlpFeatures.Name = "tlpFeatures";
            this.tlpFeatures.RowCount = 7;
            this.tlpFeatures.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tlpFeatures.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpFeatures.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpFeatures.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpFeatures.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpFeatures.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpFeatures.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFeatures.Size = new System.Drawing.Size(451, 234);
            this.tlpFeatures.TabIndex = 27;
            // 
            // lblMinimum1
            // 
            this.lblMinimum1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblMinimum1.AutoSize = true;
            this.lblMinimum1.Location = new System.Drawing.Point(400, 8);
            this.lblMinimum1.Name = "lblMinimum1";
            this.lblMinimum1.Size = new System.Drawing.Size(48, 13);
            this.lblMinimum1.TabIndex = 17;
            this.lblMinimum1.Text = "Minimum";
            this.lblMinimum1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMaximum1
            // 
            this.lblMaximum1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblMaximum1.AutoSize = true;
            this.lblMaximum1.Location = new System.Drawing.Point(329, 8);
            this.lblMaximum1.Name = "lblMaximum1";
            this.lblMaximum1.Size = new System.Drawing.Size(51, 13);
            this.lblMaximum1.TabIndex = 17;
            this.lblMaximum1.Text = "Maximum";
            this.lblMaximum1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            // lblActive
            // 
            this.lblActive.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblActive.AutoSize = true;
            this.lblActive.Location = new System.Drawing.Point(207, 8);
            this.lblActive.Name = "lblActive";
            this.lblActive.Size = new System.Drawing.Size(37, 13);
            this.lblActive.TabIndex = 8;
            this.lblActive.Text = "Active";
            this.lblActive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblConfidence
            // 
            this.lblConfidence.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblConfidence.AutoSize = true;
            this.lblConfidence.Location = new System.Drawing.Point(251, 8);
            this.lblConfidence.Name = "lblConfidence";
            this.lblConfidence.Size = new System.Drawing.Size(61, 13);
            this.lblConfidence.TabIndex = 5;
            this.lblConfidence.Text = "Confidence";
            this.lblConfidence.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.tlpVariables.ColumnCount = 4;
            this.tlpVariables.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpVariables.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tlpVariables.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tlpVariables.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tlpVariables.Controls.Add(this.lblVariable, 0, 0);
            this.tlpVariables.Controls.Add(this.lblValue, 1, 0);
            this.tlpVariables.Controls.Add(this.lblMaximum2, 2, 0);
            this.tlpVariables.Controls.Add(this.lblMinimum2, 3, 0);
            this.tlpVariables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpVariables.Location = new System.Drawing.Point(0, 234);
            this.tlpVariables.Margin = new System.Windows.Forms.Padding(0);
            this.tlpVariables.Name = "tlpVariables";
            this.tlpVariables.RowCount = 7;
            this.tlpVariables.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tlpVariables.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpVariables.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpVariables.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpVariables.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpVariables.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tlpVariables.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpVariables.Size = new System.Drawing.Size(451, 235);
            this.tlpVariables.TabIndex = 28;
            // 
            // lblVariable
            // 
            this.lblVariable.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblVariable.AutoSize = true;
            this.lblVariable.Location = new System.Drawing.Point(3, 8);
            this.lblVariable.Name = "lblVariable";
            this.lblVariable.Size = new System.Drawing.Size(45, 13);
            this.lblVariable.TabIndex = 3;
            this.lblVariable.Text = "Variable";
            this.lblVariable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblValue
            // 
            this.lblValue.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(278, 8);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(34, 13);
            this.lblValue.TabIndex = 5;
            this.lblValue.Text = "Value";
            this.lblValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMaximum2
            // 
            this.lblMaximum2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblMaximum2.AutoSize = true;
            this.lblMaximum2.Location = new System.Drawing.Point(329, 8);
            this.lblMaximum2.Name = "lblMaximum2";
            this.lblMaximum2.Size = new System.Drawing.Size(51, 13);
            this.lblMaximum2.TabIndex = 13;
            this.lblMaximum2.Text = "Maximum";
            this.lblMaximum2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMinimum2
            // 
            this.lblMinimum2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblMinimum2.AutoSize = true;
            this.lblMinimum2.Location = new System.Drawing.Point(400, 8);
            this.lblMinimum2.Name = "lblMinimum2";
            this.lblMinimum2.Size = new System.Drawing.Size(48, 13);
            this.lblMinimum2.TabIndex = 14;
            this.lblMinimum2.Text = "Minimum";
            this.lblMinimum2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(170, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Example1\\Example2\\Example.png";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(217, 32);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.checkBox1.Size = new System.Drawing.Size(17, 14);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(258, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "12.3456%";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(326, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "12.3456%";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(394, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "12.3456%";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Features
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpCore);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "Features";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.Size = new System.Drawing.Size(468, 486);
            this.tlpFeatures.ResumeLayout(false);
            this.tlpFeatures.PerformLayout();
            this.tlpCore.ResumeLayout(false);
            this.tlpVariables.ResumeLayout(false);
            this.tlpVariables.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpFeatures;
        private System.Windows.Forms.Label lblFeature;
        private System.Windows.Forms.Label lblConfidence;
        private System.Windows.Forms.Label lblActive;
        private System.Windows.Forms.TableLayoutPanel tlpCore;
        private System.Windows.Forms.TableLayoutPanel tlpVariables;
        private System.Windows.Forms.Label lblVariable;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.Label lblMaximum2;
        private System.Windows.Forms.Label lblMinimum2;
        private System.Windows.Forms.Label lblMaximum1;
        private System.Windows.Forms.Label lblMinimum1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label4;
    }
}
