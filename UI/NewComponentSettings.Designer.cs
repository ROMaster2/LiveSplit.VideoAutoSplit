namespace LiveSplit.VAS.UI
{
    partial class NewComponentSettings
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.tabScanRegion = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabDebug = new System.Windows.Forms.TabPage();
            this.tabHotkeys = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabSettings);
            this.tabControl1.Controls.Add(this.tabScanRegion);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabDebug);
            this.tabControl1.Controls.Add(this.tabHotkeys);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(476, 512);
            this.tabControl1.TabIndex = 1;
            // 
            // tabSettings
            // 
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(468, 486);
            this.tabSettings.TabIndex = 0;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // tabScanRegion
            // 
            this.tabScanRegion.Location = new System.Drawing.Point(4, 22);
            this.tabScanRegion.Name = "tabScanRegion";
            this.tabScanRegion.Padding = new System.Windows.Forms.Padding(3);
            this.tabScanRegion.Size = new System.Drawing.Size(468, 480);
            this.tabScanRegion.TabIndex = 1;
            this.tabScanRegion.Text = "Scan Region";
            this.tabScanRegion.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(468, 480);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Features";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabDebug
            // 
            this.tabDebug.Location = new System.Drawing.Point(4, 22);
            this.tabDebug.Name = "tabDebug";
            this.tabDebug.Padding = new System.Windows.Forms.Padding(3);
            this.tabDebug.Size = new System.Drawing.Size(468, 480);
            this.tabDebug.TabIndex = 4;
            this.tabDebug.Text = "Debug";
            this.tabDebug.UseVisualStyleBackColor = true;
            // 
            // tabHotkeys
            // 
            this.tabHotkeys.Location = new System.Drawing.Point(4, 22);
            this.tabHotkeys.Name = "tabHotkeys";
            this.tabHotkeys.Size = new System.Drawing.Size(468, 480);
            this.tabHotkeys.TabIndex = 5;
            this.tabHotkeys.Text = "Hotkeys";
            this.tabHotkeys.UseVisualStyleBackColor = true;
            // 
            // NewComponentSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "NewComponentSettings";
            this.Size = new System.Drawing.Size(476, 512);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TabPage tabScanRegion;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabDebug;
        private System.Windows.Forms.TabPage tabHotkeys;
    }
}
