namespace LiveSplit.UI.Components
{
    partial class ComponentUI
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
            this.tabControlCore = new System.Windows.Forms.TabControl();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.tabScanRegion = new System.Windows.Forms.TabPage();
            this.tabFeatures = new System.Windows.Forms.TabPage();
            this.tabDebug = new System.Windows.Forms.TabPage();
            this.tabHotkeys = new System.Windows.Forms.TabPage();
            this.tabControlCore.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlCore
            // 
            this.tabControlCore.Controls.Add(this.tabSettings);
            this.tabControlCore.Controls.Add(this.tabScanRegion);
            this.tabControlCore.Controls.Add(this.tabFeatures);
            this.tabControlCore.Controls.Add(this.tabDebug);
            this.tabControlCore.Controls.Add(this.tabHotkeys);
            this.tabControlCore.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlCore.Location = new System.Drawing.Point(0, 0);
            this.tabControlCore.Margin = new System.Windows.Forms.Padding(0);
            this.tabControlCore.Name = "tabControlCore";
            this.tabControlCore.SelectedIndex = 0;
            this.tabControlCore.Size = new System.Drawing.Size(476, 532);
            this.tabControlCore.TabIndex = 1;
            this.tabControlCore.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControlCore_Selecting);
            // 
            // tabSettings
            // 
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Margin = new System.Windows.Forms.Padding(0);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(468, 506);
            this.tabSettings.TabIndex = 0;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // tabScanRegion
            // 
            this.tabScanRegion.Location = new System.Drawing.Point(4, 22);
            this.tabScanRegion.Margin = new System.Windows.Forms.Padding(0);
            this.tabScanRegion.Name = "tabScanRegion";
            this.tabScanRegion.Size = new System.Drawing.Size(468, 506);
            this.tabScanRegion.TabIndex = 1;
            this.tabScanRegion.Text = "Scan Region";
            this.tabScanRegion.UseVisualStyleBackColor = true;
            // 
            // tabFeatures
            // 
            this.tabFeatures.Location = new System.Drawing.Point(4, 22);
            this.tabFeatures.Margin = new System.Windows.Forms.Padding(0);
            this.tabFeatures.Name = "tabFeatures";
            this.tabFeatures.Size = new System.Drawing.Size(468, 506);
            this.tabFeatures.TabIndex = 2;
            this.tabFeatures.Text = "Features";
            this.tabFeatures.UseVisualStyleBackColor = true;
            // 
            // tabDebug
            // 
            this.tabDebug.Location = new System.Drawing.Point(4, 22);
            this.tabDebug.Margin = new System.Windows.Forms.Padding(0);
            this.tabDebug.Name = "tabDebug";
            this.tabDebug.Size = new System.Drawing.Size(468, 506);
            this.tabDebug.TabIndex = 4;
            this.tabDebug.Text = "Error Log";
            this.tabDebug.UseVisualStyleBackColor = true;
            // 
            // tabHotkeys
            // 
            this.tabHotkeys.Location = new System.Drawing.Point(4, 22);
            this.tabHotkeys.Margin = new System.Windows.Forms.Padding(0);
            this.tabHotkeys.Name = "tabHotkeys";
            this.tabHotkeys.Size = new System.Drawing.Size(468, 506);
            this.tabHotkeys.TabIndex = 5;
            this.tabHotkeys.Text = "Hotkeys";
            this.tabHotkeys.UseVisualStyleBackColor = true;
            // 
            // NewComponentSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControlCore);
            this.Name = "NewComponentSettings";
            this.Size = new System.Drawing.Size(476, 532);
            this.tabControlCore.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlCore;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TabPage tabScanRegion;
        private System.Windows.Forms.TabPage tabFeatures;
        private System.Windows.Forms.TabPage tabDebug;
        private System.Windows.Forms.TabPage tabHotkeys;
    }
}
