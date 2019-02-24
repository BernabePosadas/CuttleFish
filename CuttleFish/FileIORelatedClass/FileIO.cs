using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace CuttleFish
{
    class FileIO
    {
        long size = 0;
        static List<byte[]> listByte = new List<byte[]>();
        EBCEA enchipher = null;
        newEnchiperProto newEncipher = null;
        public bool ExceptionEncountered = false;
        public FileIO()
        {

        }
        public void ReadFileSize(string path)
        {
            ExceptionEncountered = false;
            try
            {
                using (FileStream readData = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    size = readData.Length;
                    readData.Dispose();
                }
            }catch(Exception ErrorMessage)
            {
                ExceptionEncountered = true;
                MessageBox.Show(string.Format("Opps, file access denied. Please check if file is in use by another program \n\n{0}", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public async Task ReadData(string path, byte[] keystream, byte keyrepeat, IProgress<int> sizeReport, IProgress<int> PerformStep)
        {
            ExceptionEncountered = false;
            try
            {
                listByte.Clear();
                int chunk = 1024 * 1024 * 10;
                byte[] chunkData = new byte[chunk];
                long readChunk = 0;
                bool less100mb = false;
                enchipher = new EBCEA(keystream, keyrepeat);
                if (size < chunk)
                {
                    chunk = (int)size;
                    less100mb = true;
                }
                int rounds = 0;
                if (size % chunk != 0)
                {
                    rounds = (int)(size / chunk) + 1;
                }
                else
                {
                    rounds = (int)size / chunk;
                }
                sizeReport.Report(rounds);
                int i = 0;
                while (readChunk != size)
                {
                    if (!less100mb)
                    {
                        if ((size - readChunk) < (1024 * 1024 * 10))
                        {
                            chunk = (int)(size - readChunk);
                            chunkData = new byte[chunk];
                        }
                    }
                    chunkData = enchipher.encyptData(ReadChunk(path, readChunk, chunk, true));
                    i++;
                    PerformStep.Report(i);
                    readChunk += chunk;
                    listByte.Add(chunkData);
                    await Task.Delay(100);
                }
            }
            catch (Exception ErrorMessage)
            {
                ExceptionEncountered = true;
                MessageBox.Show(string.Format("Opps, file access denied. Please check if file is in use by another program \n\n{0}", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public async Task ReadData(string path, byte[] keystream, byte keyrepeat, IProgress<int> sizeReport, IProgress<int> PerformStep, string password)
        {
            ExceptionEncountered = false;
            try
            {
                listByte.Clear();
                int chunk = 1024 * 1024 * 10;
                byte[] chunkData = new byte[chunk];
                long readChunk = 0;
                bool less100mb = false;
                enchipher = new EBCEA(keystream, keyrepeat);
                if (size < chunk)
                {
                    chunk = (int)size;
                    less100mb = true;
                }
                int rounds = 0;
                if (size % chunk != 0)
                {
                    rounds = (int)(size / chunk) + 1;
                }
                else
                {
                    rounds = (int)size / chunk;
                }
                sizeReport.Report(rounds);
                int i = 0;
                while (readChunk != size)
                {
                    if (!less100mb)
                    {
                        if ((size - readChunk) < (1024 * 1024 * 10))
                        {
                            chunk = (int)(size - readChunk);
                            chunkData = new byte[chunk];
                        }
                    }
                    chunkData = enchipher.encyptData(ReadChunk(path, readChunk, chunk, true), password);
                    i++;
                    PerformStep.Report(i);
                    readChunk += chunk;
                    listByte.Add(chunkData);
                    await Task.Delay(100);
                }
            }
            catch (Exception ErrorMessage)
            {
                ExceptionEncountered = true;
                MessageBox.Show(string.Format("Encountered an exception while reading file.\n\n{0}", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public byte[] getLinearKey()
        {
            return newEncipher.getLinearArrayFormatKey();
        }
        public async Task ReadData(string path, byte[,] keystream, IProgress<int> sizeReport, IProgress<int> PerformStep)
        {
            ExceptionEncountered = false;
            try
            {
                listByte.Clear();
                int chunk = 1024 * 1024 * 10;
                byte[] chunkData = new byte[chunk];
                long readChunk = 0;
                bool less100mb = false;
                newEncipher = new newEnchiperProto(keystream);
                if (size < chunk)
                {
                    chunk = (int)size;
                    less100mb = true;
                }
                int rounds = 0;
                if (size % chunk != 0)
                {
                    rounds = (int)(size / chunk) + 1;
                }
                else
                {
                    rounds = (int)size / chunk;
                }
                sizeReport.Report(rounds);
                int i = 0;
                while (readChunk != size)
                {
                    if (!less100mb)
                    {
                        if ((size - readChunk) < (1024 * 1024 * 10))
                        {
                            chunk = (int)(size - readChunk);
                            chunkData = new byte[chunk];
                        }
                    }
                    chunkData = newEncipher.encrypt(ReadChunk(path, readChunk, chunk, false));
                    i++;
                    PerformStep.Report(i);
                    readChunk += chunk;
                    listByte.Add(chunkData);
                    await Task.Delay(100);
                }
            }
            catch (Exception ErrorMessage)
            {
                ExceptionEncountered = true;
                MessageBox.Show(string.Format("Opps, file access denied. Please check if file is in use by another program \n{0}", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public async Task ReadData(string path, byte[,] keystream, IProgress<int> sizeReport, IProgress<int> PerformStep, string password)
        {
            ExceptionEncountered = false;
            try
            {
                listByte.Clear();
                int chunk = 1024 * 1024 * 10;
                byte[] chunkData = new byte[chunk];
                long readChunk = 0;
                bool less100mb = false;
                newEncipher = new newEnchiperProto(keystream);
                if (size < chunk)
                {
                    chunk = (int)size;
                    less100mb = true;
                }
                int rounds = 0;
                if (size % chunk != 0)
                {
                    rounds = (int)(size / chunk) + 1;
                }
                else
                {
                    rounds = (int)size / chunk;
                }
                sizeReport.Report(rounds);
                int i = 0;
                while (readChunk != size)
                {
                    if (!less100mb)
                    {
                        if ((size - readChunk) < (1024 * 1024 * 10))
                        {
                            chunk = (int)(size - readChunk);
                            chunkData = new byte[chunk];
                        }
                    }
                    chunkData = newEncipher.encrypt(ReadChunk(path, readChunk, chunk, false), password);
                    i++;
                    PerformStep.Report(i);
                    readChunk += chunk;
                    listByte.Add(chunkData);
                    await Task.Delay(100);
                }
            }
            catch (Exception ErrorMessage)
            {
                ExceptionEncountered = true;
                MessageBox.Show(string.Format("Opps, file access denied. Please check if file is in use by another program \n{0}", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public async Task writeData(string path, long size, byte[] keystream, byte keyrepeat, IProgress<long> reportWroteData, List<byte[]> data)
        {
            ExceptionEncountered = false;
            try
            {
                enchipher = new EBCEA(keystream, keyrepeat);
                long wroteDataLenght = 0;
                reportWroteData.Report(wroteDataLenght);
                FileStream fs = null;
                if (File.Exists(path))
                {
                    fs = new FileStream(path, FileMode.Truncate, FileAccess.Write);
                }
                else
                {
                    fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                }
                using (fs)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        byte[] temp = decryptDataStream(data[i]);
                        if ((size - wroteDataLenght) < temp.Length)
                        {
                            fs.Write(temp, 0, (int)(size - wroteDataLenght));
                            wroteDataLenght += (size - wroteDataLenght);
                            reportWroteData.Report(wroteDataLenght);
                        }
                        else
                        {
                            fs.Write(temp, 0, temp.Length);
                            wroteDataLenght += temp.Length;
                            reportWroteData.Report(wroteDataLenght);
                        }
                        await Task.Delay(100);
                    }
                }
                fs.Dispose();
            }
            catch (Exception ErrorMessage)
            {
                ExceptionEncountered = true;
                MessageBox.Show(string.Format("Opps, file access denied. Please check if file is in use by another program \n{0}", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public async Task writeData(string path, long size, byte[] keystream, byte keyrepeat, IProgress<long> reportWroteData, List<byte[]> data, string password)
        {
            ExceptionEncountered = false;
            try
            {
                enchipher = new EBCEA(keystream, keyrepeat);
                long wroteDataLenght = 0;
                reportWroteData.Report(wroteDataLenght);
                FileStream fs = null;
                if (File.Exists(path))
                {
                    fs = new FileStream(path, FileMode.Truncate, FileAccess.Write);
                }
                else
                {
                    fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                }
                using (fs)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        byte[] temp = decryptDataStream(data[i], password);
                        if ((size - wroteDataLenght) < temp.Length)
                        {
                            fs.Write(temp, 0, (int)(size - wroteDataLenght));
                            wroteDataLenght += (size - wroteDataLenght);
                            reportWroteData.Report(wroteDataLenght);
                        }
                        else
                        {
                            fs.Write(temp, 0, temp.Length);
                            wroteDataLenght += temp.Length;
                            reportWroteData.Report(wroteDataLenght);
                        }
                        await Task.Delay(100);
                    }
                }
                fs.Dispose();
            }
            catch (Exception ErrorMessage)
            {
                ExceptionEncountered = true;
                MessageBox.Show(string.Format("Opps, file access denied. Please check if file is in use by another program \n{0}", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public async Task writeData(string path, long size, byte[,] keystream, IProgress<long> reportWroteData, List<byte[]> data)
        {
            ExceptionEncountered = false;
            try
            {
                newEncipher = new newEnchiperProto(keystream);
                long wroteDataLenght = 0;
                reportWroteData.Report(wroteDataLenght);
                FileStream fs = null;
                if (File.Exists(path))
                {
                    fs = new FileStream(path, FileMode.Truncate, FileAccess.Write);
                }
                else
                {
                    fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                }
                using (fs)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        byte[] temp = decryptDataStream128(data[i]);
                        if ((size - wroteDataLenght) < temp.Length)
                        {
                            fs.Write(temp, 0, (int)(size - wroteDataLenght));
                            wroteDataLenght += (size - wroteDataLenght);
                            reportWroteData.Report(wroteDataLenght);
                        }
                        else
                        {
                            fs.Write(temp, 0, temp.Length);
                            wroteDataLenght += temp.Length;
                            reportWroteData.Report(wroteDataLenght);
                        }
                        await Task.Delay(100);
                    }
                }
                fs.Dispose();
            }
            catch (Exception ErrorMessage)
            {
                ExceptionEncountered = true;
                MessageBox.Show(string.Format("Opps, file access denied. Please check if file is in use by another program \n{0}", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public async Task writeData(string path, long size, byte[,] keystream, IProgress<long> reportWroteData, List<byte[]> data, string password)
        {
            ExceptionEncountered = false;
            try
            {
                newEncipher = new newEnchiperProto(keystream);
                long wroteDataLenght = 0;
                reportWroteData.Report(wroteDataLenght);
                FileStream fs = null;
                if (File.Exists(path))
                {
                    fs = new FileStream(path, FileMode.Truncate, FileAccess.Write);
                }
                else
                {
                    fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                }
                using (fs)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        byte[] temp = decryptDataStream128(data[i], password);
                        if ((size - wroteDataLenght) < temp.Length)
                        {
                            fs.Write(temp, 0, (int)(size - wroteDataLenght));
                            wroteDataLenght += (size - wroteDataLenght);
                            reportWroteData.Report(wroteDataLenght);
                        }
                        else
                        {
                            fs.Write(temp, 0, temp.Length);
                            wroteDataLenght += temp.Length;
                            reportWroteData.Report(wroteDataLenght);
                        }
                        await Task.Delay(100);
                    }
                }
                fs.Dispose();
            }
            catch (Exception ErrorMessage)
            {
                ExceptionEncountered = true;
                MessageBox.Show(string.Format("Opps, file access denied. Please check if file is in use by another program \n{0}", ErrorMessage.Message), "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private byte[] decryptDataStream(byte[] data)
        {
            return enchipher.DecryptData(data);
        }
        private byte[] decryptDataStream(byte[] data, string password)
        {
            return enchipher.DecryptData(data, password);
        }
        private byte[] decryptDataStream128(byte[] data)
        {
            return newEncipher.decrypt(data);
        }
        private byte[] decryptDataStream128(byte[] data, string password)
        {
            return newEncipher.decrypt(data, password);
        }
        private static byte[] ReadChunk(string path, long offset, int bufferSize, bool is16BitBlockChipher)
        {
            int size = bufferSize;
            byte[] buffer = new byte[size];
            using(FileStream fs = new FileStream(path,FileMode.Open, FileAccess.Read))
            {
                fs.Seek(offset,SeekOrigin.Begin);
                fs.Read(buffer, 0, size);
                fs.Dispose();
            }
            if (buffer.Length % 2 != 0 && is16BitBlockChipher)
            {
                byte[] temp = buffer;
                buffer = new byte[temp.Length + 1];
                for (int i = 0; i < temp.Length; i++)
                {
                    buffer[i] = temp[i];
                }
                buffer[temp.Length] = 255;
            }
            return buffer;

        }
        public long getSize()
        {
            return size;
        }
        public List<byte[]> getData()
        {
            return listByte;
        }
    }
}
