namespace CuttleFish
{
    partial class ZipForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ZipForm));
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ZipStatusLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.ZipProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.compressionSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zipCompressionModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optimalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fastestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noCompressionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FileList = new System.Windows.Forms.ListBox();
            this.Add = new System.Windows.Forms.Button();
            this.RemoveFromList = new System.Windows.Forms.Button();
            this.compressFile = new System.Windows.Forms.Button();
            this.bg1 = new System.ComponentModel.BackgroundWorker();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "File List: ";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ZipStatusLbl,
            this.ZipProgressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 285);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(622, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // ZipStatusLbl
            // 
            this.ZipStatusLbl.Name = "ZipStatusLbl";
            this.ZipStatusLbl.Size = new System.Drawing.Size(39, 17);
            this.ZipStatusLbl.Text = "Ready";
            // 
            // ZipProgressBar
            // 
            this.ZipProgressBar.Name = "ZipProgressBar";
            this.ZipProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.compressionSettingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(622, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // compressionSettingsToolStripMenuItem
            // 
            this.compressionSettingsToolStripMenuItem.BackColor = System.Drawing.Color.Transparent;
            this.compressionSettingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zipCompressionModeToolStripMenuItem});
            this.compressionSettingsToolStripMenuItem.Name = "compressionSettingsToolStripMenuItem";
            this.compressionSettingsToolStripMenuItem.Size = new System.Drawing.Size(134, 20);
            this.compressionSettingsToolStripMenuItem.Text = "Compression Settings";
            // 
            // zipCompressionModeToolStripMenuItem
            // 
            this.zipCompressionModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optimalToolStripMenuItem,
            this.fastestToolStripMenuItem,
            this.noCompressionToolStripMenuItem});
            this.zipCompressionModeToolStripMenuItem.Name = "zipCompressionModeToolStripMenuItem";
            this.zipCompressionModeToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.zipCompressionModeToolStripMenuItem.Text = "Zip Compression Mode";
            // 
            // optimalToolStripMenuItem
            // 
            this.optimalToolStripMenuItem.Name = "optimalToolStripMenuItem";
            this.optimalToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.optimalToolStripMenuItem.Text = "Optimal";
            this.optimalToolStripMenuItem.Click += new System.EventHandler(this.optimalToolStripMenuItem_Click);
            // 
            // fastestToolStripMenuItem
            // 
            this.fastestToolStripMenuItem.Name = "fastestToolStripMenuItem";
            this.fastestToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.fastestToolStripMenuItem.Text = "Fastest";
            this.fastestToolStripMenuItem.Click += new System.EventHandler(this.fastestToolStripMenuItem_Click);
            // 
            // noCompressionToolStripMenuItem
            // 
            this.noCompressionToolStripMenuItem.Name = "noCompressionToolStripMenuItem";
            this.noCompressionToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.noCompressionToolStripMenuItem.Text = "No Compression";
            this.noCompressionToolStripMenuItem.Click += new System.EventHandler(this.noCompressionToolStripMenuItem_Click);
            // 
            // FileList
            // 
            this.FileList.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FileList.FormattingEnabled = true;
            this.FileList.ItemHeight = 20;
            this.FileList.Location = new System.Drawing.Point(16, 56);
            this.FileList.Name = "FileList";
            this.FileList.Size = new System.Drawing.Size(586, 164);
            this.FileList.TabIndex = 5;
            // 
            // Add
            // 
            this.Add.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Add.Location = new System.Drawing.Point(35, 235);
            this.Add.Name = "Add";
            this.Add.Size = new System.Drawing.Size(162, 38);
            this.Add.TabIndex = 6;
            this.Add.Text = "Add File";
            this.Add.UseVisualStyleBackColor = true;
            this.Add.Click += new System.EventHandler(this.Add_Click);
            // 
            // RemoveFromList
            // 
            this.RemoveFromList.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RemoveFromList.Location = new System.Drawing.Point(219, 235);
            this.RemoveFromList.Name = "RemoveFromList";
            this.RemoveFromList.Size = new System.Drawing.Size(156, 38);
            this.RemoveFromList.TabIndex = 7;
            this.RemoveFromList.Text = "Remove From List";
            this.RemoveFromList.UseVisualStyleBackColor = true;
            this.RemoveFromList.Click += new System.EventHandler(this.RemoveFromList_Click);
            // 
            // compressFile
            // 
            this.compressFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.compressFile.Location = new System.Drawing.Point(402, 235);
            this.compressFile.Name = "compressFile";
            this.compressFile.Size = new System.Drawing.Size(165, 38);
            this.compressFile.TabIndex = 8;
            this.compressFile.Text = "Compress";
            this.compressFile.UseVisualStyleBackColor = true;
            this.compressFile.Click += new System.EventHandler(this.compressFile_Click);
            // 
            // bg1
            // 
            this.bg1.WorkerReportsProgress = true;
            this.bg1.WorkerSupportsCancellation = true;
            this.bg1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bg1_DoWork);
            this.bg1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bg1_ProgressChanged);
            this.bg1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bg1_RunWorkerCompleted);
            // 
            // ZipForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 307);
            this.Controls.Add(this.compressFile);
            this.Controls.Add(this.RemoveFromList);
            this.Controls.Add(this.Add);
            this.Controls.Add(this.FileList);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ZipForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Compressor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ZipForm_FormClosing);
            this.Load += new System.EventHandler(this.ZipForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel ZipStatusLbl;
        private System.Windows.Forms.ToolStripProgressBar ZipProgressBar;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem compressionSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zipCompressionModeToolStripMenuItem;
        private System.Windows.Forms.ListBox FileList;
        private System.Windows.Forms.Button Add;
        private System.Windows.Forms.Button RemoveFromList;
        private System.Windows.Forms.Button compressFile;
        private System.Windows.Forms.ToolStripMenuItem fastestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noCompressionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optimalToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker bg1;
    }
}