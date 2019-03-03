using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;


namespace CuttleFish
{
    public partial class FileEmbed : Form
    {
        FileIO io = new FileIO();
        AVIFileREader vidreader = new AVIFileREader();
        int embededSize = 0;
        List<string> fileList = new List<string>();
        byte[] sha256 = null;
        bool removeMetadata = false;
        bool append = false;
        bool appendingError = false;
        string appendingPassword = "";
        string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CuttleFish\\ExtractedFile";
        public FileEmbed()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            long gb2 = 1024 * 1024;
            gb2 *= 1024 * 2;
            fileList = new List<string>();
            if (multipleFileModeToolStripMenuItem.Checked)
            {
                ZipForm zipFrm = new ZipForm();
                zipFrm.ShowDialog();
                if (zipFrm.DialogResult == DialogResult.OK)
                {
                    fileList = zipFrm.listFiles;
                    lblFilename.Text = "Loading.....";
                    lblSize.Text = "Loading.....";

                    txtPath.Text = AppDomain.CurrentDomain.BaseDirectory + "\\compressed.zip";
                    io.ReadFileSize(txtPath.Text);
                    lblFilename.Text = "compressed.zip";
                    lblSize.Text = displayFileSize(io.getSize());
                    if (io.ExceptionEncountered)
                    {
                        lblFilename.Text = "";
                        lblSize.Text = "";
                        txtPath.Text = "";
                    }
                    else if (io.getSize() > gb2)
                    {
                        MessageBox.Show(string.Format("the file size of the selected file is more than 2GB({0:0,000} bytes). Please use a smaller payload file.", gb2), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        lblFilename.Text = "";
                        lblSize.Text = "";
                        txtPath.Text = "";
                    }
                }
            }
            else
            {
                OpenFileDialog diag = new OpenFileDialog();
                diag.Filter = "All Files (*.*)| *.*";
                diag.Title = "Select a Payload File";
                DialogResult res = diag.ShowDialog();
                if (res == DialogResult.OK)
                {
                    lblFilename.Text = "Loading.....";
                    lblSize.Text = "Loading.....";

                    txtPath.Text = diag.FileName;
                    io.ReadFileSize(txtPath.Text);
                    lblFilename.Text = diag.SafeFileName;
                    lblSize.Text = displayFileSize(io.getSize());
                    if (io.ExceptionEncountered)
                    {
                        lblFilename.Text = "";
                        lblSize.Text = "";
                        txtPath.Text = "";
                    }
                    else if (io.getSize() > gb2)
                    {
                        MessageBox.Show(string.Format("the file size of the selected file is more than 2GB({0:0,000} bytes). Please use a smaller payload file.", gb2), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        lblFilename.Text = "";
                        lblSize.Text = "";
                        txtPath.Text = "";
                    }
                }
            }
             
            
        }
        private void sReportLbl_TextChanged(object sender, EventArgs e)
        {
            if (sReportLbl.Text.Equals("Ready"))
            {
                btnBrowse.Enabled = true;
                button1.Enabled = true;
                BeginEmbedding.Enabled = true;
                progBar.Visible = false;
                BeginEmbedding.Enabled = true;
                BeginExtraction.Enabled = true;
                VideoCoverExtractInput.Enabled = true;
            }
            else
            {
                btnBrowse.Enabled = false;
                button1.Enabled = false;
                BeginEmbedding.Enabled = false;
                progBar.Visible = true;
                BeginEmbedding.Enabled = false;
                BeginExtraction.Enabled = false;
                SaveLocationBtn.Enabled = false;
                VideoCoverExtractInput.Enabled = false;
            }
        }

        private void FileEmbed_Load(object sender, EventArgs e)
        {
            lblFilename.Text = "";
            lblSize.Text = "";
            vFilename.Text = "";
            vCodec.Text = "";
            vFrame.Text = "";
            Supportedlbl.Text = "";
            vSize.Text = "";
            groupBox12.Visible = false;
            SaveLocationBtn.Enabled = false;
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CuttleFish\\"))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CuttleFish\\");
            }
            
        }
        private string displayFileSize(long size)
        {
            UOMConverter converter = new UOMConverter();
            string a = "";
            if (size < 1024)
            {
                if (size > 1000)
                {
                    a = String.Format("{0:0,000} bytes", size);
                }
                else
                {
                    a = "" + size + " bytes";
                }
            }
            else if(size > 1024 && size < 1024 * 1024)
            {
                if (size > 1024 * 1000)
                {
                    a = String.Format("{0:0,0.00} KB", converter.byteToKilo(size));
                }
                else
                {
                    a = String.Format("{0:0.00} KB", converter.byteToKilo(size));
                }
            }
            else if (size > 1024 * 1024 && size < 1024 * 1024 * 1024)
            {
                if (size > 1024 * 1024 * 1000)
                {
                    a = String.Format("{0:0,0.00} MB", converter.byteToMega(size));
                }
                else
                {
                    a = String.Format("{0:0.00} MB", converter.byteToMega(size));
                }
            }
            else
            {
                long GB1000 = 1024 * 1024 * 1024;
                GB1000 *= 1000;
                if (size > (GB1000))
                {
                    a = String.Format("{0:0,0.00} GB", converter.byteToGiga(size));
                }
                else
                {
                    a = String.Format("{0:0.00} GB", converter.byteToGiga(size));
                }
            }
            return a;
        }
        private void ProgressReport(int progress)
        {
            progBar.Value = progress;
            if (progBar.Value == progBar.Maximum)
            {
                progBar.Value = 0;
                sReportLbl.Text = "Embedding.... ";
            }
        }
        private void initializeProgress(int Steps)
        {
            progBar.Minimum = 0;
            progBar.Maximum = Steps;
            sReportLbl.Text = "Reading and Encoding data.......";
        }
        private void reportEmbedProgress(long embededDataRepored)
        {
            progBar.Minimum = 0;
            progBar.Maximum = 100;
            sReportLbl.Text = "Embedding.... " + embededDataRepored + " bytes out of " + vidreader.embedsize2 +" bytes";
            double a = (vidreader.embedsize2 / (double)100);
            int b = (int)(embededDataRepored / a);
            progBar.Value = b;
            if (embededDataRepored == vidreader.embedsize2)
            {
                progBar.Value = 0;
                sReportLbl.Text = "Ready";

            }
        }
        private async void Encryptbtn_Click(object sender, EventArgs e)
        {
            bool formNotFilledUp = false;
            bool metaCleanErrorFlag = false;
            appendingError = false;
            ErrProvide.Clear();
            menuStrip1.Enabled = false;
            if (txtPath.Text == "")
            {
                ErrProvide.SetError(txtPath, "Select a File to embed");
                formNotFilledUp = true;
            }
            if(vidPath.Text == "")
            {
                ErrProvide.SetError(vidPath, "Select a Cover ");
                formNotFilledUp = true;
            }
            if (!formNotFilledUp)
            {
                if (removeMetadata)
                {
                    if (append)
                    {
                        if (requireAPasswordToolStripMenuItem.Checked)
                        {
                            BeginExtract(appendingPassword, true);
                        }
                        else
                        {
                            BeginExtract(true);
                        }
                    }
                    else if (!append)
                    {
                        sReportLbl.Text = "Cleaning Video Cover......";
                        try
                        {
                            await vidreader.RemoveMetadata(vidPath.Text);
                        }
                        catch (Exception ErrorMessage)
                        {
                            MessageBox.Show(string.Format("Opps, Unable to access the video cover. Please check if the video cover file exist or not in use by another program \n\n{0} ", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            metaCleanErrorFlag = true;
                        }
                        sReportLbl.Text = "Ready";
                    }
                    
                }
                if (metaCleanErrorFlag)
                {
                    txtPath.Text = "";
                    vidPath.Text = "";
                }
                else if (!append)
                {
                    BeginEmbed(false);
                }
            }
            else if(formNotFilledUp)
            {
                MessageBox.Show("Please fill up the neccessary inputs", "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
         }
        private async void BeginEmbed(bool fromBackgroundWorker)
        {
            Stopwatch time = new Stopwatch();
            if (Supportedlbl.Text == "Yes")
            {
                if (vidreader.getVideoStreamSize() >= (io.getSize() * 4))
                {
                    if (bitfastToolStripMenuItem.Checked)
                    {
                        if (requireAPasswordToolStripMenuItem.Checked)
                        {
                            if (!fromBackgroundWorker)
                            {
                                SetPasswordPasswordPopUp setPass = new SetPasswordPasswordPopUp("Set Password");
                                setPass.ShowDialog();
                                if (setPass.DialogResult == DialogResult.OK)
                                {
                                    time.Start();
                                    string password = setPass.password;
                                    CryptoRelated.SHA256Hasher hasher = new CryptoRelated.SHA256Hasher();
                                    byte[] hashedpass = hasher.getHash(password);
                                    byte[] keystream = new byte[2];
                                    Random keygen = new Random();
                                    keystream[0] = (byte)keygen.Next(0, 255);
                                    keystream[1] = (byte)keygen.Next(0, 255);
                                    byte repeatcount = (byte)keygen.Next(1, 5);
                                    var dataProgressIndicator = new Progress<int>(ProgressReport);
                                    var dataProgressIndicator2 = new Progress<int>(initializeProgress);
                                    time.Start();
                                    await io.ReadData(txtPath.Text, keystream, repeatcount, dataProgressIndicator2, dataProgressIndicator, password);
                                    var dataProgressIndicator3 = new Progress<long>(reportEmbedProgress);
                                    byte[] format = Encoding.ASCII.GetBytes(Path.GetExtension(txtPath.Text));
                                    List<byte[]> data = io.getData();
                                    bool itsRGB16 = false;
                                    try
                                    {
                                        await vidreader.ReadAndEmbedVideoStream(vidPath.Text, io.getData(), dataProgressIndicator3, io.getSize(), format, keystream, repeatcount, itsRGB16, hashedpass);
                                        time.Stop();
                                        TimeSpan span = time.Elapsed;
                                        string ElapsedTime = "The Time Elapsed is ";
                                        if (span.Hours > 0)
                                        {
                                            ElapsedTime += span.Hours + " hours ";
                                        }
                                        if (span.Minutes > 0)
                                        {
                                            ElapsedTime += span.Minutes + " minutes ";
                                        }
                                        if (span.Seconds > 0)
                                        {
                                            ElapsedTime += span.Seconds + " seconds ";
                                        }
                                        ElapsedTime += span.Milliseconds + " milliseconds";
                                        MessageBox.Show(string.Format("File Embeded Successfully!!\n{0}", ElapsedTime), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        cleanLocalAppFile();
                                        requireAPasswordToolStripMenuItem.Enabled = true;
                                        if (deleteAfterEmbeddingToolStripMenuItem.Checked)
                                        {
                                            if (fileList.Count != 0)
                                            {
                                                for (int i = 0; i < fileList.Count; i++)
                                                {
                                                    File.Delete(fileList[i]);
                                                }
                                                fileList.Clear();
                                            }
                                            else
                                            {
                                                File.Delete(txtPath.Text);
                                            }
                                        }
                                    }
                                    catch (Exception ErrorMessage)
                                    {
                                        MessageBox.Show(string.Format("Opps! We encountered an error while embedding. Please check if the payload file or video file exist and not used by another program and restart the operation. \n\n{0}", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    txtPath.Text = "";
                                    vidPath.Text = "";
                                }
                            }
                            else
                            {
                                time.Start();
                                string password = appendingPassword;
                                CryptoRelated.SHA256Hasher hasher = new CryptoRelated.SHA256Hasher();
                                byte[] hashedpass = hasher.getHash(password);
                                byte[] keystream = new byte[2];
                                Random keygen = new Random();
                                keystream[0] = (byte)keygen.Next(0, 255);
                                keystream[1] = (byte)keygen.Next(0, 255);
                                byte repeatcount = (byte)keygen.Next(1, 5);
                                var dataProgressIndicator = new Progress<int>(ProgressReport);
                                var dataProgressIndicator2 = new Progress<int>(initializeProgress);
                                time.Start();
                                await io.ReadData(txtPath.Text, keystream, repeatcount, dataProgressIndicator2, dataProgressIndicator, password);
                                var dataProgressIndicator3 = new Progress<long>(reportEmbedProgress);
                                byte[] format = Encoding.ASCII.GetBytes(Path.GetExtension(txtPath.Text));
                                List<byte[]> data = io.getData();
                                bool itsRGB16 = false;
                                try
                                {
                                    await vidreader.ReadAndEmbedVideoStream(vidPath.Text, io.getData(), dataProgressIndicator3, io.getSize(), format, keystream, repeatcount, itsRGB16, hashedpass);
                                    time.Stop();
                                    TimeSpan span = time.Elapsed;
                                    string ElapsedTime = "The Time Elapsed is ";
                                    if (span.Hours > 0)
                                    {
                                        ElapsedTime += span.Hours + " hours ";
                                    }
                                    if (span.Minutes > 0)
                                    {
                                        ElapsedTime += span.Minutes + " minutes ";
                                    }
                                    if (span.Seconds > 0)
                                    {
                                        ElapsedTime += span.Seconds + " seconds ";
                                    }
                                    ElapsedTime += span.Milliseconds + " milliseconds";
                                    MessageBox.Show(string.Format("File Embeded Successfully!!\n{0}", ElapsedTime), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    cleanLocalAppFile();
                                    requireAPasswordToolStripMenuItem.Enabled = true;
                                    if (deleteAfterEmbeddingToolStripMenuItem.Checked)
                                    {
                                        if (fileList.Count != 0)
                                        {
                                            for (int i = 0; i < fileList.Count; i++)
                                            {
                                                File.Delete(fileList[i]);
                                            }
                                            fileList.Clear();
                                        }
                                        else
                                        {
                                            File.Delete(txtPath.Text);
                                        }
                                    }
                                }
                                catch (Exception ErrorMessage)
                                {
                                    MessageBox.Show(string.Format("Opps! We encountered an error while embedding. Please check if the payload file or video file exist and not used by another program and restart the operation. \n\n{0}", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                txtPath.Text = "";
                                vidPath.Text = "";
                            }
                        }
                        else
                        {
                            time.Start();
                            byte[] keystream = new byte[2];
                            Random keygen = new Random();
                            keystream[0] = (byte)keygen.Next(0, 255);
                            keystream[1] = (byte)keygen.Next(0, 255);
                            byte repeatcount = (byte)keygen.Next(1, 5);
                            var dataProgressIndicator = new Progress<int>(ProgressReport);
                            var dataProgressIndicator2 = new Progress<int>(initializeProgress);
                            await io.ReadData(txtPath.Text, keystream, repeatcount, dataProgressIndicator2, dataProgressIndicator);
                            var dataProgressIndicator3 = new Progress<long>(reportEmbedProgress);
                            byte[] format = Encoding.ASCII.GetBytes(Path.GetExtension(txtPath.Text));
                            List<byte[]> data = io.getData();
                            bool itsRGB16 = false;
                            try
                            {
                                await vidreader.ReadAndEmbedVideoStream(vidPath.Text, io.getData(), dataProgressIndicator3, io.getSize(), format, keystream, repeatcount, itsRGB16);
                                time.Stop();
                                TimeSpan span = time.Elapsed;
                                string ElapsedTime = "The Time Elapsed is ";
                                if (span.Hours > 0)
                                {
                                    ElapsedTime += span.Hours + " hours ";
                                }
                                if (span.Minutes > 0)
                                {
                                    ElapsedTime += span.Minutes + " minutes ";
                                }
                                if (span.Seconds > 0)
                                {
                                    ElapsedTime += span.Seconds + " seconds ";
                                }
                                ElapsedTime += span.Milliseconds + " milliseconds";
                                MessageBox.Show(string.Format("File Embeded Successfully!!\n{0}", ElapsedTime), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                cleanLocalAppFile();
                                requireAPasswordToolStripMenuItem.Enabled = true;
                                if (deleteAfterEmbeddingToolStripMenuItem.Checked)
                                {
                                    if (fileList.Count != 0)
                                    {
                                        for (int i = 0; i < fileList.Count; i++)
                                        {
                                            File.Delete(fileList[i]);
                                        }
                                        fileList.Clear();
                                    }
                                    else
                                    {
                                        File.Delete(txtPath.Text);
                                    }
                                }
                            }
                            catch (Exception ErrorMessage)
                            {
                                MessageBox.Show(string.Format("Opps! We encountered an error while embedding. Please check if the payload file or video file exist and not used by another program and restart the operation. \n\n{0}", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            txtPath.Text = "";
                            vidPath.Text = "";

                        }

                    }
                    else
                    {
                        if (requireAPasswordToolStripMenuItem.Checked)
                        {
                            if (!fromBackgroundWorker)
                            {
                                SetPasswordPasswordPopUp setPass = new SetPasswordPasswordPopUp("Set Password");
                                setPass.ShowDialog();
                                if (setPass.DialogResult == DialogResult.OK)
                                {
                                    time.Start();
                                    string password = setPass.password;
                                    CryptoRelated.SHA256Hasher hasher = new CryptoRelated.SHA256Hasher();
                                    byte[] hashedpass = hasher.getHash(password);
                                    byte[,] keystream = CryptoRelated.keyGen.generate128bitkey();
                                    var dataProgressIndicator = new Progress<int>(ProgressReport);
                                    var dataProgressIndicator2 = new Progress<int>(initializeProgress);
                                    await io.ReadData(txtPath.Text, keystream, dataProgressIndicator2, dataProgressIndicator, password);
                                    var dataProgressIndicator3 = new Progress<long>(reportEmbedProgress);
                                    byte[] format = Encoding.ASCII.GetBytes(Path.GetExtension(txtPath.Text));
                                    List<byte[]> data = io.getData();
                                    bool itsRGB16 = false;
                                    try
                                    {
                                        await vidreader.ReadAndEmbedVideoStream(vidPath.Text, io.getData(), dataProgressIndicator3, io.getSize(), format, io.getLinearKey(), 0, itsRGB16, hashedpass);
                                        time.Stop();
                                        TimeSpan span = time.Elapsed;
                                        string ElapsedTime = "The Time Elapsed is ";
                                        if (span.Hours > 0)
                                        {
                                            ElapsedTime += span.Hours + " hours ";
                                        }
                                        if (span.Minutes > 0)
                                        {
                                            ElapsedTime += span.Minutes + " minutes ";
                                        }
                                        if (span.Seconds > 0)
                                        {
                                            ElapsedTime += span.Seconds + " seconds ";
                                        }
                                        ElapsedTime += span.Milliseconds + " milliseconds";
                                        MessageBox.Show(string.Format("File Embeded Successfully!!\n{0}", ElapsedTime), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        cleanLocalAppFile();
                                        requireAPasswordToolStripMenuItem.Enabled = true;
                                        if (deleteAfterEmbeddingToolStripMenuItem.Checked)
                                        {
                                            if (fileList.Count != 0)
                                            {
                                                for (int i = 0; i < fileList.Count; i++)
                                                {
                                                    File.Delete(fileList[i]);
                                                }
                                                fileList.Clear();
                                            }
                                            else
                                            {
                                                File.Delete(txtPath.Text);
                                            }
                                        }
                                    }
                                    catch (Exception ErrorMessage)
                                    {
                                        MessageBox.Show(string.Format("Opps! We encountered an error while embedding. Please check if the payload file or video file exist and not used by another program and restart the operation. \n\n{0}", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    txtPath.Text = "";
                                    vidPath.Text = "";
                                }
                            }
                            else
                            {
                                time.Start();
                                string password = appendingPassword;
                                CryptoRelated.SHA256Hasher hasher = new CryptoRelated.SHA256Hasher();
                                byte[] hashedpass = hasher.getHash(password);
                                byte[,] keystream = CryptoRelated.keyGen.generate128bitkey();
                                var dataProgressIndicator = new Progress<int>(ProgressReport);
                                var dataProgressIndicator2 = new Progress<int>(initializeProgress);
                                await io.ReadData(txtPath.Text, keystream, dataProgressIndicator2, dataProgressIndicator, password);
                                var dataProgressIndicator3 = new Progress<long>(reportEmbedProgress);
                                byte[] format = Encoding.ASCII.GetBytes(Path.GetExtension(txtPath.Text));
                                List<byte[]> data = io.getData();
                                bool itsRGB16 = false;
                                try
                                {
                                    await vidreader.ReadAndEmbedVideoStream(vidPath.Text, io.getData(), dataProgressIndicator3, io.getSize(), format, io.getLinearKey(), 0, itsRGB16, hashedpass);
                                    time.Stop();
                                    TimeSpan span = time.Elapsed;
                                    string ElapsedTime = "The Time Elapsed is ";
                                    if (span.Hours > 0)
                                    {
                                        ElapsedTime += span.Hours + " hours ";
                                    }
                                    if (span.Minutes > 0)
                                    {
                                        ElapsedTime += span.Minutes + " minutes ";
                                    }
                                    if (span.Seconds > 0)
                                    {
                                        ElapsedTime += span.Seconds + " seconds ";
                                    }
                                    ElapsedTime += span.Milliseconds + " milliseconds";
                                    MessageBox.Show(string.Format("File Embeded Successfully!!\n{0}", ElapsedTime), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    cleanLocalAppFile();
                                    requireAPasswordToolStripMenuItem.Enabled = true;
                                    if (deleteAfterEmbeddingToolStripMenuItem.Checked)
                                    {
                                        if (fileList.Count != 0)
                                        {
                                            for (int i = 0; i < fileList.Count; i++)
                                            {
                                                File.Delete(fileList[i]);
                                            }
                                            fileList.Clear();
                                        }
                                        else
                                        {
                                            File.Delete(txtPath.Text);
                                        }
                                    }
                                }
                                catch (Exception ErrorMessage)
                                {
                                    MessageBox.Show(string.Format("Opps! We encountered an error while embedding. Please check if the payload file or video file exist and not used by another program and restart the operation. \n\n{0}", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                txtPath.Text = "";
                                vidPath.Text = "";
                            }
                        }
                        else
                        {
                            time.Start();
                            byte[,] keystream = CryptoRelated.keyGen.generate128bitkey();
                            var dataProgressIndicator = new Progress<int>(ProgressReport);
                            var dataProgressIndicator2 = new Progress<int>(initializeProgress);
                            await io.ReadData(txtPath.Text, keystream, dataProgressIndicator2, dataProgressIndicator);
                            var dataProgressIndicator3 = new Progress<long>(reportEmbedProgress);
                            byte[] format = Encoding.ASCII.GetBytes(Path.GetExtension(txtPath.Text));
                            List<byte[]> data = io.getData();
                            bool itsRGB16 = false;
                            try
                            {
                                await vidreader.ReadAndEmbedVideoStream(vidPath.Text, io.getData(), dataProgressIndicator3, io.getSize(), format, io.getLinearKey(), 0, itsRGB16);
                                time.Stop();
                                TimeSpan span = time.Elapsed;
                                string ElapsedTime = "The Time Elapsed is ";
                                if (span.Hours > 0)
                                {
                                    ElapsedTime += span.Hours + " hours ";
                                }
                                if (span.Minutes > 0)
                                {
                                    ElapsedTime += span.Minutes + " minutes ";
                                }
                                if (span.Seconds > 0)
                                {
                                    ElapsedTime += span.Seconds + " seconds ";
                                }
                                ElapsedTime += span.Milliseconds + " milliseconds";
                                MessageBox.Show(string.Format("File Embeded Successfully!!\n{0}", ElapsedTime), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                requireAPasswordToolStripMenuItem.Enabled = true;
                                cleanLocalAppFile();
                                if (deleteAfterEmbeddingToolStripMenuItem.Checked)
                                {
                                    if (fileList.Count != 0)
                                    {
                                        for (int i = 0; i < fileList.Count; i++)
                                        {
                                            File.Delete(fileList[i]);
                                        }
                                        fileList.Clear();
                                    }
                                    else
                                    {
                                        File.Delete(txtPath.Text);
                                    }
                                }
                            }
                            catch (Exception ErrorMessage)
                            {
                                MessageBox.Show(string.Format("Opps! We encountered an error while embedding. Please check if the payload file or video file exist and not used by another program and restart the operation. \n\n{0}", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            txtPath.Text = "";
                            vidPath.Text = "";
                        }

                    }
                }

                else
                {
                    MessageBox.Show("Opps! we cannot embed the size bigger than the capacity of the video cover. Please use payload withe the size of " + vSize.Text + " or less", "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Unsupported Video Codec \nThe Codec used in the video cover file is currently not supported by this system. \nPlease use a supported codec", "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           menuStrip1.Enabled = true;
           sReportLbl.Text = "Ready";
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            requireAPasswordToolStripMenuItem.Enabled = true;
            removeMetadata = false;
            append = false;
            var dataProgressIndicator3 = new Progress<int>(reportEmbededFileSize);
            var dataProgressIndicator4 = new Progress<string>(displayExt);
            OpenFileDialog diag = new OpenFileDialog();
            diag.Filter = "AVI Files (*.avi)| *.avi";
            diag.Title = "Select a Video Cover";
            DialogResult res = diag.ShowDialog();
            if (res == DialogResult.OK)
            {

                button1.Enabled = false;
                btnBrowse.Enabled = false;
                BeginEmbedding.Enabled = false;
                vFilename.Text = diag.SafeFileName;
                vidPath.Text = diag.FileName;
                vFrame.Text = "Loading.......";
                vCodec.Text = "Loading.......";
                vSize.Text = "Loading.......";
                Supportedlbl.Text = "Checking.........";
                var dataProgressIndicator = new Progress<int>(reportFramesExtracted);
                await vidreader.readInfo(diag.FileName, dataProgressIndicator);
                if (!vidreader.ErrorEncountered)
                {
                    if (vidreader.checkForEmbeddedFile(diag.FileName, dataProgressIndicator3, dataProgressIndicator4))
                    {
                        if (MessageBox.Show("The system found an existing embedded file inside the selected video cover. Do you wish combine it with the selected payload file on time we embed your files?\n\nNOTE: Choosing No will overwrite the payload inside when the video cover is used for embedding instead. Please extract it or use a video cover with no payload inside if you wish not to combine it and keep the payload inside the selected video cover", "Cuttlefish", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            if (vidreader.passwordProtected)
                            {
                                bool reEnterPassword = true;
                                sha256 = vidreader.Sha256Stream;
                                while (reEnterPassword)
                                {
                                    SetPasswordPasswordPopUp pass = new SetPasswordPasswordPopUp("Enter Password....");
                                    pass.ShowDialog();
                                    if (pass.DialogResult == DialogResult.OK)
                                    {
                                        string password2 = pass.password;
                                        CryptoRelated.SHA256Hasher hasher = new CryptoRelated.SHA256Hasher();
                                        byte[] sha256comp = hasher.getHash(password2);
                                        if (hasher.compareHashes(sha256, sha256comp))
                                        {
                                            appendingPassword = password2;
                                            vFrame.Text = vidreader.getFrames();
                                            vCodec.Text = vidreader.getVCodec();
                                            displayVideoFileSize(vidreader.getVideoStreamSize());
                                            validateIfSupported();
                                            removeMetadata = true;
                                            append = true;
                                            MessageBox.Show("The system will extract and combine the selected payload file when embedding starts.", "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            if (!requireAPasswordToolStripMenuItem.Checked)
                                            {
                                                requireAPasswordToolStripMenuItem.Checked = true;
                                                requireAPasswordToolStripMenuItem.Enabled = false;
                                                lblSetPaswordProtection.Text = "Yes";
                                                append = true;
                                                
                                            }
                                            reEnterPassword = false;
                                        }
                                        else
                                        {
                                            MessageBox.Show("Incorrect Password", "CuttleFish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                    else
                                    {
                                        vFrame.Text = "";
                                        vCodec.Text = "";
                                        vSize.Text = "";
                                        Supportedlbl.Text = "";
                                        vFilename.Text = "";
                                        vidPath.Text = "";
                                    }
                                }
                            }
                            else
                            {
                                vFrame.Text = vidreader.getFrames();
                                vCodec.Text = vidreader.getVCodec();
                                displayVideoFileSize(vidreader.getVideoStreamSize());
                                validateIfSupported();
                                removeMetadata = true;
                                append = true;
                                MessageBox.Show("The system will extract and combine the selected payload file when embedding starts.", "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            vFrame.Text = vidreader.getFrames();
                            vCodec.Text = vidreader.getVCodec();
                            displayVideoFileSize(vidreader.getVideoStreamSize());
                            validateIfSupported();
                            removeMetadata = true;
                            MessageBox.Show("The system will overwrite the existing payload file when embedding starts.", "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        sReportLbl.Text = "Ready";
                    }
                    else
                    {
                        vFrame.Text = vidreader.getFrames();
                        vCodec.Text = vidreader.getVCodec();
                        displayVideoFileSize(vidreader.getVideoStreamSize());
                        validateIfSupported();
                    }
                }
                else
                {
                    vFrame.Text = "";
                    vCodec.Text = "";
                    vSize.Text = "";
                    Supportedlbl.Text = "";
                    vFilename.Text = "";
                    vidPath.Text = "";
                }
            }
        }
        private void displayVideoFileSize(long size)
        {
            size = size / 4;
            vSize.Text = displayFileSize(size);
        }
        private void reportFramesExtracted(int frames)
        {
            sReportLbl.Text = String.Format("Counting Video Cover Frames. {0} Frames Counted", frames);
            progBar.Visible = false;
            if (vCodec.Text != "Loading.......")
            {
                sReportLbl.Text = "Ready";
            }
        }
        private void validateIfSupported()
        {
            if (vCodec.Text == "RGB 24 bit" /*|| vCodec.Text == "RGB 8 bit"*/)
            {
                Supportedlbl.Text = "Yes";
            }
            else
            {
                Supportedlbl.Text = "No";
            }
            
        }
        private void FileEmbed_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sReportLbl.Text != "Ready")
            {
                if (MessageBox.Show("Operation still ongoing. Are you Sure you want to stop?", "Cuttlefish", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    vidreader.stop();
                    cleanLocalAppFile();
                }
                
            }
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\compressed.zip"))
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\compressed.zip");
            }
            cleanLocalAppFile();
        }

        private async void VideoCoverExtract_Click(object sender, EventArgs e)
        {
            vidreader = new AVIFileREader();
            OpenFileDialog diag = new OpenFileDialog();
            diag.Filter = "AVI Files (*.avi)| *.avi";
            diag.Title = "Select an Embedded Video Cover";
            DialogResult res = diag.ShowDialog();
            if (res == DialogResult.OK)
            {    
                var dataProgressIndicator = new Progress<int>(reportEmbededFileSize);
                var dataProgressIndicator2 = new Progress<string>(displayExt);
                await vidreader.searchForEmbededData(diag.FileName, dataProgressIndicator, dataProgressIndicator2);
                if (vidreader.HasEmbededFile() && !vidreader.ErrorEncountered)
                {
                    groupBox12.Visible = true;
                    SaveLocationBtn.Enabled = true;
                    if (vidreader.passwordProtected)
                    {
                        sha256 = vidreader.Sha256Stream;
                    }
                    label12.Text = string.Format("{0} bit", (vidreader.getKeyStream().Length * 8));
                    label5.Text = "" + vidreader.passwordProtected;
                    vCoverPath.Text = diag.FileName;
                }
                else
                {
                    vCoverPath.Text = "";
                    groupBox12.Visible = false;
                    SaveLocationBtn.Enabled = false;
                    sha256 = null;
                    MessageBox.Show("We found no embedded file on selected Video Cover. Please select a video cover with a payload inside", "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                savePath.Text = "";
            }
        }
        private void displayExt(string ext)
        {
            extension.Text = ext;
        }
        private void reportEmbededFileSize(int size)
        {
            eSize.Text = displayFileSize(size);
        }
        private void SaveLocationBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog diag = new SaveFileDialog();
            diag.DefaultExt = extension.Text;
            if (extension.Text == "")
            {
                diag.Filter = "Raw File (*.*)| *.*";
            }
            else
            {
                diag.Filter = String.Format("{0} files (*{0})| *{0}", extension.Text);
            }
            diag.Title = "Provide a Save Location and Filename";
            if (diag.ShowDialog() == DialogResult.OK)
            {
                savePath.Text = diag.FileName;
            }
        }
        
        private void BeginExtraction_Click(object sender, EventArgs e)
        {
            bool frmNotFilled = false;
            ErrProvide.Clear();
            if (vCoverPath.Text == "")
            {
                ErrProvide.SetError(vCoverPath, "Please select a video cover");
                frmNotFilled = true;
            }
            if (savePath.Text == "")
            {
                ErrProvide.SetError(savePath, "Please provide a save location for the extracted file");
                frmNotFilled = true;
            }
            if (!frmNotFilled)
            {
                if (vidreader.passwordProtected)
                {
                    SetPasswordPasswordPopUp pass = new SetPasswordPasswordPopUp("Enter Password....");
                    pass.ShowDialog();
                    if (pass.DialogResult == DialogResult.OK)
                    {
                        string password2 = pass.password;
                        CryptoRelated.SHA256Hasher hasher = new CryptoRelated.SHA256Hasher();
                        byte[] sha256comp = hasher.getHash(password2);
                        if (hasher.compareHashes(sha256, sha256comp))
                        {
                            BeginExtract(password2, false);
                        }
                        else
                        {
                            MessageBox.Show("Incorrect Password", "CuttleFish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            SaveLocationBtn.Enabled = true;
                        }
                    }
                }
                else
                {
                    BeginExtract(false);
                }

            }
            else
            {
                MessageBox.Show("Please fill up the necessary inputs", "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
            }

        }
        private async void BeginExtract(bool saveToAppData)
        {
            embededSize = vidreader.getESize();
            try
            {
                Stopwatch time = new Stopwatch();
                time.Start();
                var dataProgressIndicator = new Progress<long>(reportExtractedProgress);
                bool itsRGB16 = false;
                if (vidreader.getVCodec().Equals("RGB 16 bit"))
                {
                    itsRGB16 = true;
                }
                if (!saveToAppData)
                {
                    await vidreader.ReadAndExtractVideoStream(vCoverPath.Text, dataProgressIndicator, embededSize, itsRGB16);
                    var dataProgressIndicator2 = new Progress<long>(reportWroteData);
                    byte[] keystream = vidreader.getKeyStream();
                    if (vidreader.flag[0] == 0)
                    {
                        byte repeatcount = vidreader.getRepeatCount();
                        await io.writeData(savePath.Text, embededSize, keystream, repeatcount, dataProgressIndicator2, vidreader.getExtractedData());
                    }
                    else
                    {
                        ConverterClass.recToLinearArray rtl = new ConverterClass.recToLinearArray();
                        byte[,] keystream2 = rtl.convertTo4x4RectArray(keystream);
                        await io.writeData(savePath.Text, embededSize, keystream2, dataProgressIndicator2, vidreader.getExtractedData());
                    }
                    sReportLbl.Text = "Removing Metadata Please Wait.....";
                    progBar.Visible = false;
                    await vidreader.RemoveMetadata(vCoverPath.Text);
                    sReportLbl.Text = "Ready";
                    time.Stop();
                    TimeSpan span = time.Elapsed;
                    string ElapsedTime = "The Time Elapsed is ";
                    if (span.Hours > 0)
                    {
                        ElapsedTime += span.Hours + " hours ";
                    }
                    if (span.Minutes > 0)
                    {
                        ElapsedTime += span.Minutes + " minutes ";
                    }
                    if (span.Seconds > 0)
                    {
                        ElapsedTime += span.Seconds + " seconds ";
                    }
                    ElapsedTime += span.Milliseconds + " milliseconds";
                    MessageBox.Show("Exctracted File save at " + savePath.Text + "\n" + ElapsedTime, "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    await vidreader.ReadAndExtractVideoStream(vidPath.Text, dataProgressIndicator, embededSize, itsRGB16);
                    var dataProgressIndicator2 = new Progress<long>(reportWroteData);
                    byte[] keystream = vidreader.getKeyStream();
                    if (vidreader.flag[0] == 0)
                    {
                        byte repeatcount = vidreader.getRepeatCount();
                        await io.writeData(AppDataPath+extension.Text, embededSize, keystream, repeatcount, dataProgressIndicator2, vidreader.getExtractedData());
                    }
                    else
                    {
                        ConverterClass.recToLinearArray rtl = new ConverterClass.recToLinearArray();
                        byte[,] keystream2 = rtl.convertTo4x4RectArray(keystream);
                        await io.writeData(AppDataPath + extension.Text, embededSize, keystream2, dataProgressIndicator2, vidreader.getExtractedData());
                    }
                    if (singleFileToolStripMenuItem.Checked)
                    {
                        fileList.Clear();
                        fileList.Add(txtPath.Text);
                    }
                    if (!bgAppendor.IsBusy)
                    {
                        bgAppendor.RunWorkerAsync();
                    }
                 
                }

            }
            catch (Exception ErrorMessage)
            {
                MessageBox.Show(string.Format("Opps! We encountered an error while extracting. Please check if the video file exist and not used by another program and restart the operation. \n\n{0}", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                appendingError = saveToAppData;
            }
            if (!saveToAppData)
            {
                vCoverPath.Text = "";
                savePath.Text = "";
                SaveLocationBtn.Enabled = false;
                groupBox12.Visible = false;
            }
        }
        private async void BeginExtract(string password, bool saveToAppData)
        {
            embededSize = vidreader.getESize();
            try
            {
                Stopwatch time = new Stopwatch();
                time.Start();
                var dataProgressIndicator = new Progress<long>(reportExtractedProgress);
                bool itsRGB16 = false;
                if (vidreader.getVCodec().Equals("RGB 16 bit"))
                {
                    itsRGB16 = true;
                }
                if (!saveToAppData)
                {
                    await vidreader.ReadAndExtractVideoStream(vCoverPath.Text, dataProgressIndicator, embededSize, itsRGB16);
                    var dataProgressIndicator2 = new Progress<long>(reportWroteData);
                    byte[] keystream = vidreader.getKeyStream();
                    if (vidreader.flag[0] == 0)
                    {
                        byte repeatcount = vidreader.getRepeatCount();
                        await io.writeData(savePath.Text, embededSize, keystream, repeatcount, dataProgressIndicator2, vidreader.getExtractedData(), password);
                    }
                    else
                    {
                        ConverterClass.recToLinearArray rtl = new ConverterClass.recToLinearArray();
                        byte[,] keystream2 = rtl.convertTo4x4RectArray(keystream);
                        await io.writeData(savePath.Text, embededSize, keystream2, dataProgressIndicator2, vidreader.getExtractedData(), password);
                    }
                    sReportLbl.Text = "Removing Metadata Please Wait.....";
                    progBar.Visible = false;
                    await vidreader.RemoveMetadata(vCoverPath.Text);
                    sReportLbl.Text = "Ready";
                    time.Stop();
                    TimeSpan span = time.Elapsed;
                    string ElapsedTime = "The Time Elapsed is ";
                    if (span.Hours > 0)
                    {
                        ElapsedTime += span.Hours + " hours ";
                    }
                    if (span.Minutes > 0)
                    {
                        ElapsedTime += span.Minutes + " minutes ";
                    }
                    if (span.Seconds > 0)
                    {
                        ElapsedTime += span.Seconds + " seconds ";
                    }
                    ElapsedTime += span.Milliseconds + " milliseconds";
                    MessageBox.Show("Exctracted File save at " + savePath.Text + "\n" + ElapsedTime, "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    await vidreader.ReadAndExtractVideoStream(vidPath.Text, dataProgressIndicator, embededSize, itsRGB16);
                    var dataProgressIndicator2 = new Progress<long>(reportWroteData);
                    byte[] keystream = vidreader.getKeyStream();
                    if (vidreader.flag[0] == 0)
                    {
                        byte repeatcount = vidreader.getRepeatCount();
                        await io.writeData(AppDataPath + extension.Text, embededSize, keystream, repeatcount, dataProgressIndicator2, vidreader.getExtractedData(), password);
                    }
                    else
                    {
                        ConverterClass.recToLinearArray rtl = new ConverterClass.recToLinearArray();
                        byte[,] keystream2 = rtl.convertTo4x4RectArray(keystream);
                        await io.writeData(AppDataPath + extension.Text, embededSize, keystream2, dataProgressIndicator2, vidreader.getExtractedData(), password);
                    }
                    if (singleFileToolStripMenuItem.Checked)
                    {
                        fileList.Clear();
                        fileList.Add(txtPath.Text);
                    }
                    if (!bgAppendor.IsBusy)
                    {
                        bgAppendor.RunWorkerAsync();
                    }
                }
            }
            catch (Exception ErrorMessage)
            {
                MessageBox.Show(string.Format("Opps! We encountered an error while extracting. Please check if the video file exist and not used by another program and restart the operation. \n\n{0}", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                appendingError = saveToAppData;
            }
            if (!saveToAppData)
            {
                vCoverPath.Text = "";
                savePath.Text = "";
                SaveLocationBtn.Enabled = false;
                groupBox12.Visible = false;
            }
        }
        private void cleanLocalAppFile()
        {
            string localFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CuttleFish";
            string[] allFiles = Directory.GetFiles(localFilePath);
            foreach(string Datafilename in allFiles)
            {
                File.Delete(Datafilename);
            }
        }
        private void reportExtractedProgress(long extractedDataRepored)
        {
            progBar.Minimum = 0;
            progBar.Maximum = 100;
            long c = extractedDataRepored;
            if (extractedDataRepored < vidreader.embedsize2)
            {
                sReportLbl.Text = "Extracting.... " + extractedDataRepored + " bytes out of " + vidreader.embedsize2 + " bytes";
                double a = (vidreader.embedsize2 / (double)100);
                int b = (int)(c / a);
                progBar.Value = b;
            }
            else
            {
                sReportLbl.Text = "Extracting.... " + vidreader.embedsize2 + " bytes out of " + vidreader.embedsize2 + " bytes";
                double a = (vidreader.embedsize2 / (double)100);
                int b = (int)(vidreader.embedsize2 / a);
                progBar.Value = b;
            }
            if (extractedDataRepored == vidreader.embedsize2)
            {
                progBar.Value = 0;
                sReportLbl.Text = "Decoding and Saving File.... ";
            }
        }
        private void reportWroteData(long wroteDataReported)
        {
            progBar.Minimum = 0;
            progBar.Maximum = 100;
            sReportLbl.Text = "Decoding and Saving File.... " + wroteDataReported + " bytes out of " + vidreader.embedsize2 + " bytes";
            double a = (embededSize / (double)100);
            int b = (int)(wroteDataReported / a);

            progBar.Value = b;
            if (wroteDataReported == embededSize)
            {
                progBar.Value = 0;
                sReportLbl.Text = "Ready";
            }
        }
        private void closeAltF4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void deleteAfterEmbeddingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!deleteAfterEmbeddingToolStripMenuItem.Checked)
            {
                deleteAfterEmbeddingToolStripMenuItem.Checked = true;
                lblSorceFileDelete.Text = "Yes";
            }
            else
            {
                deleteAfterEmbeddingToolStripMenuItem.Checked = false;
                lblSorceFileDelete.Text = "No";
            }
        }

        private void multipleFileModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (singleFileToolStripMenuItem.Checked)
            {
                singleFileToolStripMenuItem.Checked = false;
            }
            multipleFileModeToolStripMenuItem.Checked = true;
            lblFileSelectionMode.Text = "Multiple Files";
            txtPath.Text = ""; 
        }

        private void singleFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (multipleFileModeToolStripMenuItem.Checked)
            {
                multipleFileModeToolStripMenuItem.Checked = false;
            }
            singleFileToolStripMenuItem.Checked = true;
            lblFileSelectionMode.Text = "Single File";
            txtPath.Text = "";
        }
        private void requireAPasswordToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (requireAPasswordToolStripMenuItem.Checked)
            {
                requireAPasswordToolStripMenuItem.Checked = false;
                lblSetPaswordProtection.Text = "No";
            }
            else
            {
                requireAPasswordToolStripMenuItem.Checked = true;
                lblSetPaswordProtection.Text = "Yes";
            }
        }

        private void bitfastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            bitfastToolStripMenuItem.Checked = true;
            bitslowerToolStripMenuItem.Checked = false;
            lblKeySize.Text = "16 bit";
        }
        
        private void bitslowerToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            bitslowerToolStripMenuItem.Checked = true;
            bitfastToolStripMenuItem.Checked = false;
            lblKeySize.Text = "128 bit";
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void vCoverPath_TextChanged(object sender, EventArgs e)
        {
            if (vCoverPath.Text == "")
            {
                SaveLocationBtn.Enabled = false;
                groupBox12.Visible = false;
            }
            if (vCoverPath.Text == txtPath.Text)
            {
                txtPath.Text = "";
            }
            if (vCoverPath.Text == vidPath.Text)
            {
                vidPath.Text = "";
            }
        }

        private void txtPath_TextChanged(object sender, EventArgs e)
        {
            if (txtPath.Text == "")
            {
                lblFilename.Text = "";
                lblSize.Text = "";
            }
            if (txtPath.Text == vCoverPath.Text)
            {
                vCoverPath.Text = "";
            }
            if (txtPath.Text == vidPath.Text)
            {
                vidPath.Text = "";
            }

        }

        private void vidPath_TextChanged(object sender, EventArgs e)
        {
            if (vidPath.Text == "")
            {
                vFilename.Text = "";
                vCodec.Text = "";
                vFrame.Text = "";
                Supportedlbl.Text = "";
                vSize.Text = "";
            }
            if (vidPath.Text == vCoverPath.Text)
            {
                vCoverPath.Text = "";
            }
            if (txtPath.Text == vidPath.Text)
            {
                txtPath.Text = "";
            }
        }

        private void bgAppendor_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if(File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CuttleFish\\NewZipPayload.zip"))
            {
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CuttleFish\\NewZipPayload.zip");
            }
            if (extension.Text.Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                string zipPath = AppDataPath + extension.Text;
                using (ZipArchive zipper = ZipFile.Open(zipPath, ZipArchiveMode.Update))
                {
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                            break;
                        }
                        else
                        {
                            ZipArchiveEntry entry = zipper.GetEntry(Path.GetFileName(fileList[i]));
                            if (entry != null)
                            {
                                entry.Delete();
                            }
                            zipper.CreateEntryFromFile(fileList[i], Path.GetFileName(fileList[i]), CompressionLevel.Optimal);
                            System.Threading.Thread.Sleep(100);
                            worker.ReportProgress(i + 2);
                        }
                    }

                    zipper.Dispose();
                }

            }
            else
            {
                string zipPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CuttleFish\\NewZipPayload.zip";
                using (ZipArchive zipper = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                            break;
                        }
                        else
                        {
                            zipper.CreateEntryFromFile(fileList[i], Path.GetFileName(fileList[i]), CompressionLevel.Optimal);
                            zipper.CreateEntryFromFile(AppDataPath + extension.Text, "Extracted From " + Path.GetFileName(vidPath.Text) + Path.GetExtension(AppDataPath + extension.Text), CompressionLevel.Optimal);
                            System.Threading.Thread.Sleep(100);
                            worker.ReportProgress(i + 2);
                        }
                    }

                    zipper.Dispose();
                }

            }
            
        }

        private void bgAppendor_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            sReportLbl.Text = string.Format("Combining File..... {0} of {1} files", e.ProgressPercentage, fileList.Count + 1);
        }

        private async void bgAppendor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("User cancelled the operation", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else if (e.Error != null)
            {
                MessageBox.Show("Opps, Encountered an error when compressing file. Please check if the file selected is used by another program or the computer hard drive space. \n\n" + e.Error.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                sReportLbl.Text = "Ready";
            }
            else
            {
                if (extension.Text.Equals(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    txtPath.Text = AppDataPath + extension.Text;
                   
                }
                else
                {
                    txtPath.Text = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CuttleFish\\NewZipPayload.zip";
                   
                }
                io.ReadFileSize(txtPath.Text);
                if (vidreader.getVideoStreamSize() >= (io.getSize() * 4))
                {
                    try
                    {
                        await vidreader.RemoveMetadata(vidPath.Text);
                        sReportLbl.Text = string.Format("Combining File..... 1 of {0} files", fileList.Count + 1);

                    }
                    catch (Exception ErrorMessage)
                    {
                        MessageBox.Show(string.Format("Opps, Unable to access the video cover. Please check if the video cover file exist or not in use by another program \n\n{0} ", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    BeginEmbed(true);
                }
                else
                {
                    lblFilename.Text = Path.GetFileName(txtPath.Text);
                    lblSize.Text = displayFileSize(io.getSize());
                    MessageBox.Show("We finished merging the files but unfortunately the file size exceeded the capacity of the video cover. We automatically set the merged file as payload. please select a new video cover that can hold " + lblSize.Text + " or more", "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    append = false;
                    sReportLbl.Text = "Ready";
                    requireAPasswordToolStripMenuItem.Enabled = true;
                    
                }
            }
        }

    }

}
