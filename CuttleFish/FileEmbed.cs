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
        public FileEmbed()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
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
            }
        }
        private void initializeProgress(int Steps)
        {
            progBar.Minimum = 0;
            progBar.Maximum = Steps;
            sReportLbl.Text = "Loading and Encrypting data.......";
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
            Stopwatch time = new Stopwatch();
            bool formNotFilledUp = false;
            bool metaCleanErrorFlag = false;
            ErrProvide.Clear();
            deleteAfterEmbeddingToolStripMenuItem.Enabled = false;
            if (removeMetadata)
            {
                sReportLbl.Text = "Cleaning Video Cover......";
                try
                {
                    await vidreader.RemoveMetadata(vidPath.Text);
                }
                catch(Exception ErrorMessage)
                {
                    MessageBox.Show(string.Format("Encountered an exception while cleaning existing metadata.\n{0} ", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    metaCleanErrorFlag = true;
                }
                sReportLbl.Text = "Ready";
            }
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
            if (!formNotFilledUp && !metaCleanErrorFlag)
            {
                long gb2 = 1024 * 1024;
                gb2 *= 1024 * 2;
                if (io.getSize() > gb2)
                {
                    MessageBox.Show(string.Format("Cannot embed data, payload must not be greater than 2GB({0:0,000} bytes)", gb2), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (Supportedlbl.Text == "Yes")
                {
                    if (vidreader.getVideoStreamSize() >= (io.getSize() * 4))
                    {
                        if (bitfastToolStripMenuItem.Checked)
                        {
                            if (requireAPasswordToolStripMenuItem.Checked)
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
                                        MessageBox.Show(string.Format("File Embeded Successfully!!\nTime Elapsed: {0}", time.Elapsed), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                                        MessageBox.Show(string.Format("Encountered an exception while embedding file.\n{0} ", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                    MessageBox.Show(string.Format("File Embeded Successfully!!\nTime Elapsed: {0}", time.Elapsed), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                                    MessageBox.Show(string.Format("Encountered an exception while embedding file.\n{0} ", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                txtPath.Text = "";
                                vidPath.Text = "";
                                
                            }
                            
                        }
                        else
                        {
                            if (requireAPasswordToolStripMenuItem.Checked)
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
                                        MessageBox.Show(string.Format("File Embeded Successfully!!\nTime Elapsed: {0}", time.Elapsed), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                                        MessageBox.Show(string.Format("Encountered an exception while embedding file.\n{0} ", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                    MessageBox.Show(string.Format("File Embeded Successfully!!\nTime Elapsed: {0}", time.Elapsed), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                                    MessageBox.Show(string.Format("Encountered an exception while embedding file.\n{0} ", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                txtPath.Text = "";
                                vidPath.Text = "";
                            }
                           
                        }
                    }

                    else
                    {
                        MessageBox.Show("Cannot embed data, selected video cover can only hold " + displayFileSize((vidreader.getVideoStreamSize() / 4)) + "bytes or less", "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Unsupported Video Codec! \nThe Codec used in the video cover file is currently not supported by this system. \nCurrently only codecless RGB 24 bit is supported", "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else if(formNotFilledUp)
            {
                MessageBox.Show("Missing input found. Fill up the neccessary inputs", "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (metaCleanErrorFlag)
            {
                txtPath.Text = "";
                vidPath.Text = "";
            }
            deleteAfterEmbeddingToolStripMenuItem.Enabled = true;
            sReportLbl.Text = "Ready";
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            removeMetadata = false;
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
                    if (vidreader.checkForEmbeddedFile(diag.FileName))
                    {
                        if (MessageBox.Show("The system found an existing embedded file inside the selected video cover. Using the selected video cover for embedding will erase the existing one. Do you still want to use it as a video cover?", "Cuttlefish", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            vFrame.Text = vidreader.getFrames();
                            vCodec.Text = vidreader.getVCodec();
                            displayVideoFileSize(vidreader.getVideoStreamSize());
                            validateIfSupported();
                            removeMetadata = true;
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
                }
                
            }
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\compressed.zip"))
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\compressed.zip");
            }
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
                    MessageBox.Show("Found no embedded file on selected Video Cover", "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
            diag.Filter = String.Format("{0} files (*{0})| *{0}", extension.Text);
            diag.Title = "Provide a Save Location and Filename";
            if (diag.ShowDialog() == DialogResult.OK)
            {
                savePath.Text = diag.FileName;
            }
        }

        private void BeginExtraction_Click(object sender, EventArgs e)
        {
            embededSize = vidreader.getESize();
            bool frmNotFilled = false;
            ErrProvide.Clear();
            SaveLocationBtn.Enabled = false;
            if (vCoverPath.Text == "")
            {
                ErrProvide.SetError(vCoverPath, "Select a Video Cover");
                frmNotFilled = true;
            }
            if (savePath.Text == "")
            {
                ErrProvide.SetError(savePath, "Provide a Save Location for the Extracted File");
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
                            BeginExtract(password2);
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
                    BeginExtract();
                }

            }
            else
            {
                MessageBox.Show("Missing input found. Fill up the neccessary inputs", "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SaveLocationBtn.Enabled = false;
            }

        }
        private async void BeginExtract()
        {
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
                await vidreader.ReadAndExtractVideoStream(vCoverPath.Text, dataProgressIndicator, embededSize, itsRGB16);
                var dataProgressIndicator2 = new Progress<long>(reportWroteData);
                byte[] keystream = vidreader.getKeyStream();
                byte repeatcount = vidreader.getRepeatCount();
                if (vidreader.flag[0] == 0)
                {
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
                MessageBox.Show("Exctracted File Saved. \nPath: " + savePath.Text + "\nTime Elapsed: " + time.Elapsed, "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ErrorMessage)
            {
                MessageBox.Show(string.Format("Encountered an exception while extracting file.\n{0}", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            vCoverPath.Text = "";
            savePath.Text = "";
            SaveLocationBtn.Enabled = false;
            groupBox12.Visible = false;
        }
        private async void BeginExtract(string password)
        {
            Stopwatch time = new Stopwatch();
            time.Start();
            var dataProgressIndicator = new Progress<long>(reportExtractedProgress);
            bool itsRGB16 = false;
            if (vidreader.getVCodec().Equals("RGB 16 bit"))
            {
                itsRGB16 = true;
            }
            await vidreader.ReadAndExtractVideoStream(vCoverPath.Text, dataProgressIndicator, embededSize, itsRGB16);
            var dataProgressIndicator2 = new Progress<long>(reportWroteData);
            byte[] keystream = vidreader.getKeyStream();
            byte repeatcount = vidreader.getRepeatCount();
            if (vidreader.flag[0] == 0)
            {
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
            MessageBox.Show("Exctracted File Saved. \nPath: " + savePath.Text + "\nTime Elapsed: " + time.Elapsed, "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Information);
            vCoverPath.Text = "";
            savePath.Text = "";
            SaveLocationBtn.Enabled = false;
            groupBox12.Visible = false;
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
                sReportLbl.Text = "Decrypting and Saving File.... ";
            }
        }
        private void reportWroteData(long wroteDataReported)
        {
            progBar.Minimum = 0;
            progBar.Maximum = 100;
            sReportLbl.Text = "Decrypting and Saving File.... " + wroteDataReported + " bytes out of " + vidreader.embedsize2 + " bytes";
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
        }

        private void singleFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (multipleFileModeToolStripMenuItem.Checked)
            {
                multipleFileModeToolStripMenuItem.Checked = false;
            }
            singleFileToolStripMenuItem.Checked = true;
            lblFileSelectionMode.Text = "Single File";
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
    }

}
