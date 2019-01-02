namespace LiveSplit.VAS.UI
{
    partial class MainVASSettings
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
            /*if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);*/
            this.Hide();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainVASSettings));
            this.tabControlCore = new System.Windows.Forms.TabControl();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.tabScanRegion = new System.Windows.Forms.TabPage();
            this.tabFeatures = new System.Windows.Forms.TabPage();
            this.tabDebug = new System.Windows.Forms.TabPage();
            this.tabControlCore.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlCore
            // 
            this.tabControlCore.Controls.Add(this.tabSettings);
            this.tabControlCore.Controls.Add(this.tabScanRegion);
            this.tabControlCore.Controls.Add(this.tabFeatures);
            this.tabControlCore.Controls.Add(this.tabDebug);
            this.tabControlCore.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlCore.Location = new System.Drawing.Point(0, 0);
            this.tabControlCore.Margin = new System.Windows.Forms.Padding(0);
            this.tabControlCore.Name = "tabControlCore";
            this.tabControlCore.SelectedIndex = 0;
            this.tabControlCore.Size = new System.Drawing.Size(787, 922);
            this.tabControlCore.TabIndex = 2;
            // 
            // tabSettings
            // 
            this.tabSettings.Location = new System.Drawing.Point(8, 39);
            this.tabSettings.Margin = new System.Windows.Forms.Padding(0);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(771, 875);
            this.tabSettings.TabIndex = 0;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // tabScanRegion
            // 
            this.tabScanRegion.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.tabScanRegion.Location = new System.Drawing.Point(8, 39);
            this.tabScanRegion.Margin = new System.Windows.Forms.Padding(0);
            this.tabScanRegion.Name = "tabScanRegion";
            this.tabScanRegion.Size = new System.Drawing.Size(784, 403);
            this.tabScanRegion.TabIndex = 1;
            this.tabScanRegion.Text = "Scan Region";
            this.tabScanRegion.UseVisualStyleBackColor = true;
            // 
            // tabFeatures
            // 
            this.tabFeatures.Location = new System.Drawing.Point(8, 39);
            this.tabFeatures.Margin = new System.Windows.Forms.Padding(0);
            this.tabFeatures.Name = "tabFeatures";
            this.tabFeatures.Size = new System.Drawing.Size(784, 403);
            this.tabFeatures.TabIndex = 2;
            this.tabFeatures.Text = "Features";
            this.tabFeatures.UseVisualStyleBackColor = true;
            // 
            // tabDebug
            // 
            this.tabDebug.Location = new System.Drawing.Point(8, 39);
            this.tabDebug.Margin = new System.Windows.Forms.Padding(0);
            this.tabDebug.Name = "tabDebug";
            this.tabDebug.Size = new System.Drawing.Size(784, 403);
            this.tabDebug.TabIndex = 4;
            this.tabDebug.Text = "Error Log";
            this.tabDebug.UseVisualStyleBackColor = true;
            // 
            // MainVASSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 922);
            this.Controls.Add(this.tabControlCore);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainVASSettings";
            this.Text = "VideoAutoSplit Settings";
            this.tabControlCore.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TabControl tabControlCore;
        public System.Windows.Forms.TabPage tabSettings;
        public System.Windows.Forms.TabPage tabScanRegion;
        public System.Windows.Forms.TabPage tabFeatures;
        public System.Windows.Forms.TabPage tabDebug;
    }
}