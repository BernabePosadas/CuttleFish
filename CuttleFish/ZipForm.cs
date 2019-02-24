using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;

namespace CuttleFish
{
    public partial class ZipForm : Form
    {
        byte CompressFlag = 0;
        public List<string> listFiles = new List<string>();
        public ZipForm()
        {
            InitializeComponent();
        }

        private void ZipForm_Load(object sender, EventArgs e)
        {
            compressFile.Enabled = false;
            RemoveFromList.Enabled = false;
            optimalToolStripMenuItem.Checked = true;
            checkStatusLabelStatus();
        }
        private void checkStatusLabelStatus()
        {
            if (ZipStatusLbl.Text == "Ready")
            {
                ZipProgressBar.Visible = false;
            }
            else
            {
                ZipProgressBar.Style = ProgressBarStyle.Continuous;
                ZipProgressBar.Visible = true;
            }
        }
        private void Add_Click(object sender, EventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.Filter = "All Files (*.*)| *.*";
            diag.Multiselect = true;
            diag.Title = "Select Files to add";
            DialogResult res = diag.ShowDialog();
            if (res == DialogResult.OK)
            {
                foreach(string filename in diag.FileNames)
                {
                  if (!checkIfFileAlreadyAdded(Path.GetFileName(filename)))
                  {
                    FileList.Items.Add(Path.GetFileName(filename));
                    listFiles.Add(filename);
                    RemoveFromList.Enabled = true;
                    compressFile.Enabled = true;
                 }
                 else
                  {
                    MessageBox.Show("Error while adding file: " + Path.GetFileName(filename) + "\nthe file was already in the list", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                  }
                }
            }
        }
        private bool checkIfFileAlreadyAdded(string filename)
        {
            return FileList.Items.Contains(filename);
        }

        private void RemoveFromList_Click(object sender, EventArgs e)
        {
            if (FileList.SelectedIndex != -1)
            {
                int ind = FileList.SelectedIndex;
                FileList.Items.RemoveAt(ind);
                listFiles.RemoveAt(ind);
                if (listFiles.Count == 0)
                {
                    RemoveFromList.Enabled = false;
                    compressFile.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("No Selected Item", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void compressFile_Click(object sender, EventArgs e)
        {
            ZipStatusLbl.Text = "Compressing File";
            if (bg1.IsBusy != true)
            {
                bg1.RunWorkerAsync();
            }
            Add.Enabled = false;
            RemoveFromList.Enabled = false;
            compressFile.Enabled = false;
            zipCompressionModeToolStripMenuItem.Enabled = false;
        }
        private void fastestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastestToolStripMenuItem.Checked = true;
            noCompressionToolStripMenuItem.Checked = false;
            optimalToolStripMenuItem.Checked = false;
            CompressFlag = 1;
        }

        private void noCompressionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastestToolStripMenuItem.Checked = false;
            noCompressionToolStripMenuItem.Checked = true;
            optimalToolStripMenuItem.Checked = false;
            CompressFlag = 2;
        }

        private void optimalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastestToolStripMenuItem.Checked = false;
            noCompressionToolStripMenuItem.Checked = false;
            optimalToolStripMenuItem.Checked = true;
            CompressFlag = 0;
        }

        private void ZipForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            if (ZipStatusLbl.Text != "Ready")
            {
                if (MessageBox.Show("Operation still ongoing. Are you Sure you want to stop?", "Cuttlefish", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    if (bg1.WorkerSupportsCancellation)
                    {
                        bg1.CancelAsync();
                    }
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\compressed.zip"))
                    {
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\compressed.zip");
                    }
                }

            }
        }

        private void bg1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            string zipPath = AppDomain.CurrentDomain.BaseDirectory + "\\compressed.zip";
            if(File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }
            byte compressionFlag = CompressFlag;
            try
            {
                if (compressionFlag == 0)
                {

                    using (ZipArchive zipper = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                    {
                        for (int i = 0; i < listFiles.Count; i++)
                        {
                            if (worker.CancellationPending)
                            {
                                e.Cancel = true;
                                break;
                            }
                            else
                            {
                                zipper.CreateEntryFromFile(listFiles[i], Path.GetFileName(listFiles[i]), CompressionLevel.Optimal);
                                System.Threading.Thread.Sleep(100);
                                worker.ReportProgress(i + 1);
                            }
                        }
                        
                        zipper.Dispose();
                    }
                }
                else if (compressionFlag == 1)
                {
                    using (ZipArchive zipper = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                    {
                        for (int i = 0; i < listFiles.Count; i++)
                        {
                            if (worker.CancellationPending)
                            {
                                e.Cancel = true;
                                break;
                            }
                            else
                            {
                                zipper.CreateEntryFromFile(listFiles[i], Path.GetFileName(listFiles[i]), CompressionLevel.Fastest);
                                System.Threading.Thread.Sleep(100);
                                worker.ReportProgress(i + 1);
                            }
                        }

                        zipper.Dispose();
                    }
                }
                else
                {
                    using (ZipArchive zipper = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                    {
                        for (int i = 0; i < listFiles.Count; i++)
                        {
                            if (worker.CancellationPending)
                            {
                                e.Cancel = true;
                                break;
                            }
                            else
                            {
                                zipper.CreateEntryFromFile(listFiles[i], Path.GetFileName(listFiles[i]), CompressionLevel.NoCompression);
                                System.Threading.Thread.Sleep(100);
                                worker.ReportProgress(i + 1);
                            }
                        }

                        zipper.Dispose();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("The System Encountered an Exception \n" + ex, "Compressor", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bg1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ZipProgressBar.Maximum = listFiles.Count;
            ZipProgressBar.Value = e.ProgressPercentage;
            ZipStatusLbl.Text = string.Format("Compressing File {0} of {1}", e.ProgressPercentage, listFiles.Count);
            if (ZipProgressBar.Value == ZipProgressBar.Maximum)
            {
                ZipStatusLbl.Text = "Ready";
            }
            checkStatusLabelStatus();
        }

        private void bg1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                ZipStatusLbl.Text = "Ready";
                checkStatusLabelStatus();
                MessageBox.Show("User cancelled the operation", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.No;
                
            }
            else if (e.Error != null)
            {
                MessageBox.Show("Opps, Encountered an error when compressing file. Please check if the file selected is used by another program. \n\n" + e.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Compression Complete\nFile: compressed.zip\nThe file will be automatically selected as payload file", "Cuttlefish Zip File Compressor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
