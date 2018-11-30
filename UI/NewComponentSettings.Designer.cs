namespace LiveSplit.UI.Components
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
            this.tabControlCore = new System.Windows.Forms.TabControl();
            this.tabOptions = new System.Windows.Forms.TabPage();
            this.options = new LiveSplit.UI.Components.Options();
            this.tabScanRegion = new System.Windows.Forms.TabPage();
            this.scanRegion = new LiveSplit.UI.Components.ScanRegion();
            this.tabFeatures = new System.Windows.Forms.TabPage();
            this.features = new LiveSplit.UI.Components.Features();
            this.tabDebug = new System.Windows.Forms.TabPage();
            this.debug = new LiveSplit.UI.Components.Debug();
            this.tabHotkeys = new System.Windows.Forms.TabPage();
            this.tabControlCore.SuspendLayout();
            this.tabOptions.SuspendLayout();
            this.tabScanRegion.SuspendLayout();
            this.tabFeatures.SuspendLayout();
            this.tabDebug.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlCore
            // 
            this.tabControlCore.Controls.Add(this.tabOptions);
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
            // tabOptions
            // 
            this.tabOptions.Controls.Add(this.options);
            this.tabOptions.Location = new System.Drawing.Point(4, 22);
            this.tabOptions.Margin = new System.Windows.Forms.Padding(0);
            this.tabOptions.Name = "tabOptions";
            this.tabOptions.Size = new System.Drawing.Size(468, 506);
            this.tabOptions.TabIndex = 0;
            this.tabOptions.Text = "Settings";
            this.tabOptions.UseVisualStyleBackColor = true;
            // 
            // options
            // 
            this.options.Dock = System.Windows.Forms.DockStyle.Fill;
            this.options.GameVersion = "";
            this.options.Location = new System.Drawing.Point(0, 0);
            this.options.Margin = new System.Windows.Forms.Padding(0);
            this.options.Name = "options";
            this.options.Padding = new System.Windows.Forms.Padding(7);
            this.options.ProfilePath = null;
            this.options.Size = new System.Drawing.Size(468, 506);
            this.options.TabIndex = 0;
            this.options.VideoDevice = null;
            // 
            // tabScanRegion
            // 
            this.tabScanRegion.Controls.Add(this.scanRegion);
            this.tabScanRegion.Location = new System.Drawing.Point(4, 22);
            this.tabScanRegion.Margin = new System.Windows.Forms.Padding(0);
            this.tabScanRegion.Name = "tabScanRegion";
            this.tabScanRegion.Size = new System.Drawing.Size(468, 506);
            this.tabScanRegion.TabIndex = 1;
            this.tabScanRegion.Text = "Scan Region";
            this.tabScanRegion.UseVisualStyleBackColor = true;
            // 
            // scanRegion
            // 
            this.scanRegion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scanRegion.Location = new System.Drawing.Point(0, 0);
            this.scanRegion.Margin = new System.Windows.Forms.Padding(0);
            this.scanRegion.Name = "scanRegion";
            this.scanRegion.Padding = new System.Windows.Forms.Padding(7);
            this.scanRegion.Size = new System.Drawing.Size(468, 506);
            this.scanRegion.TabIndex = 0;
            // 
            // tabFeatures
            // 
            this.tabFeatures.Controls.Add(this.features);
            this.tabFeatures.Location = new System.Drawing.Point(4, 22);
            this.tabFeatures.Margin = new System.Windows.Forms.Padding(0);
            this.tabFeatures.Name = "tabFeatures";
            this.tabFeatures.Size = new System.Drawing.Size(468, 506);
            this.tabFeatures.TabIndex = 2;
            this.tabFeatures.Text = "Features";
            this.tabFeatures.UseVisualStyleBackColor = true;
            // 
            // features
            // 
            this.features.Dock = System.Windows.Forms.DockStyle.Fill;
            this.features.Location = new System.Drawing.Point(0, 0);
            this.features.Margin = new System.Windows.Forms.Padding(0);
            this.features.Name = "features";
            this.features.Padding = new System.Windows.Forms.Padding(7);
            this.features.Size = new System.Drawing.Size(468, 506);
            this.features.TabIndex = 0;
            // 
            // tabDebug
            // 
            this.tabDebug.Controls.Add(this.debug);
            this.tabDebug.Location = new System.Drawing.Point(4, 22);
            this.tabDebug.Margin = new System.Windows.Forms.Padding(0);
            this.tabDebug.Name = "tabDebug";
            this.tabDebug.Size = new System.Drawing.Size(468, 506);
            this.tabDebug.TabIndex = 4;
            this.tabDebug.Text = "Error Log";
            this.tabDebug.UseVisualStyleBackColor = true;
            // 
            // debug
            // 
            this.debug.Dock = System.Windows.Forms.DockStyle.Fill;
            this.debug.ErrorLog = null;
            this.debug.Location = new System.Drawing.Point(0, 0);
            this.debug.Margin = new System.Windows.Forms.Padding(0);
            this.debug.Name = "debug";
            this.debug.Padding = new System.Windows.Forms.Padding(7);
            this.debug.Size = new System.Drawing.Size(468, 506);
            this.debug.TabIndex = 0;
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
            this.tabOptions.ResumeLayout(false);
            this.tabScanRegion.ResumeLayout(false);
            this.tabFeatures.ResumeLayout(false);
            this.tabDebug.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlCore;
        private System.Windows.Forms.TabPage tabOptions;
        private System.Windows.Forms.TabPage tabScanRegion;
        private System.Windows.Forms.TabPage tabFeatures;
        private System.Windows.Forms.TabPage tabDebug;
        private System.Windows.Forms.TabPage tabHotkeys;
        private Options options;
        private ScanRegion scanRegion;
        private Features features;
        private Debug debug;
    }
}
