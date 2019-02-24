using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace CuttleFish
{
    /* FOURCC indexing table
     * 1 = AVI (main header)
     * 2 = LIST 
     * 3 = movi
     * 4 = xxwb
     * 5 = ##db / ##dc
     * 6 = ##ix / ix##
     * 7 = idx1
     * 8 = RIFF
     * 9 = hdlr / strl / odml / INFO
     * 10 = JUNK
     * where # is equivalent to any number 
     */
    
    class AVIFileREader
    {
        int eSize = 0;
        int width = 0, height = 0;
        bool stopOperationFlag = false;
        string vCodec = "";
        int frameCount = 0;
        long vSize = 0;
        public long embedsize2 { set; get; }
        LSBEmbedderClass embeder = null;
        LSBExtractorClass extractor = null;
        byte[] junk = Encoding.ASCII.GetBytes("JUNK");
        byte[] cuttle = Encoding.ASCII.GetBytes("CUTTLE");
        byte[] hsps = Encoding.ASCII.GetBytes("HSPS");
        long junkoffset = 0;
        bool okCapJunkChunk = false;
        bool hasEmbededFile = false;
        byte[] keystreamdata = new byte[2];
        byte repeatCount = 0;
        public byte[] flag = new byte[1];
        List<long> vOffsetList = new List<long>();
        List<long> jOffsetList = new List<long>();
        public bool passwordProtected = false;
        public byte[] Sha256Stream = new byte[32];
        byte zero0x0 = 0;
        long metaLocation = 0;
        public bool ErrorEncountered = false;
        public AVIFileREader()
        {

        }
        public async Task ReadAndEmbedVideoStream(string path, List<byte[]> data, IProgress<long> reportEmbededData, long embedSize, byte[] format, byte[] keystream, byte repeatcount, bool isRGB16)
        {
            FileStream readVid = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            using (readVid)
            {
                
                int fID = 4;
                long seekOffset = 0;
                byte[] fourcc = new byte[fID];
                readVid.Read(fourcc, 0, 4);
                int chunkSize = 0;
                int hops = 0;
                embedsize2 = 0;
                if (embedSize % 2 != 0 && repeatcount != 0)
                {
                    embedsize2 = embedSize + 1;
                }
                else
                {
                    embedsize2 = embedSize;
                }
                embeder = new LSBEmbedderClass(data, isRGB16);
                int metaSize = 0;
                if (repeatcount == 0)
                {
                    metaSize = (cuttle.Length + 1 + 4 + format.Length + 4 + 16);
                }
                else
                {
                    metaSize = (cuttle.Length + 1 + 4 + format.Length + 4 + 2 + 1);
                }
                for (int i = 0; i < jOffsetList.Count; i++)
                {
                    readVid.Seek(jOffsetList[i], SeekOrigin.Begin);
                    readVid.Read(fourcc, 0, 4);
                    if (getEquivalentDecimal(fourcc) >= metaSize && !okCapJunkChunk)
                    {
                        okCapJunkChunk = true;
                        junkoffset = jOffsetList[i]  + 4;
                        break;
                    }
                }
                while (hops < vOffsetList.Count)
                {
                    seekOffset = vOffsetList[hops];
                    if (stopOperationFlag)
                    {
                        break;
                    }
                    if (embeder.getEmbeded() != embedsize2)
                    {
                        readVid.Seek(seekOffset, SeekOrigin.Begin);
                        readVid.Read(fourcc, 0, 4);
                        seekOffset += 4;
                        chunkSize = getEquivalentDecimal(fourcc);
                        byte[] container = new byte[chunkSize];
                        readVid.Read(container, 0, chunkSize);
                        container = processChunk(container);
                        readVid.Seek(seekOffset, SeekOrigin.Begin);
                        readVid.Write(container, 0, chunkSize);
                        seekOffset += chunkSize;
                        reportEmbededData.Report(embeder.getEmbeded());
                    }
                    else
                    {
                        if (okCapJunkChunk)
                        {
                            byte flags = 0;
                            if (keystream.Length == 16)
                            {
                                flags = 1;
                            }
                            
                            seekOffset = junkoffset - 4;
                            int pointer = 0;
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            chunkSize = getEquivalentDecimal(fourcc);
                            readVid.Write(cuttle, 0, cuttle.Length);
                            pointer += cuttle.Length;
                            readVid.WriteByte(flags);
                            pointer += 1;
                            readVid.Write(getEquivalentLittleEndian((long)format.Length), 0, 4);
                            pointer += 4;
                            readVid.Write(format, 0, format.Length);
                            pointer += format.Length;
                            readVid.Write(getEquivalentLittleEndian(embedSize), 0, 4);
                            pointer += 4;
                            readVid.Write(keystream, 0, keystream.Length);
                            pointer += keystream.Length;
                            if (flags == 0)
                            {
                                readVid.WriteByte(repeatcount);
                                pointer += 1;
                            }
                            while (pointer != chunkSize)
                            {
                                readVid.WriteByte(zero0x0);
                                pointer++;
                            }
                        }
                        else
                        {
                            byte flags = 0;
                            if (keystream.Length == 16)
                            {
                                flags = 1;
                            }
                            seekOffset = readVid.Length;
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Write(cuttle, 0, cuttle.Length);
                            readVid.WriteByte(flags);
                            readVid.Write(getEquivalentLittleEndian((long)format.Length), 0, 4);
                            readVid.Write(format, 0, format.Length);
                            readVid.Write(getEquivalentLittleEndian(embedSize), 0, 4);
                            readVid.Write(keystream, 0, keystream.Length);
                        }
                        break;
                    }
                    hops++;
                    if (hops % 2 == 0)
                    {
                        await Task.Delay(100);
                    }
                }
            }
            readVid.Dispose();
            resetClasses();
        }
        private void resetClasses()
        {
            embeder = null;
            extractor = null;
            GC.Collect(0, GCCollectionMode.Forced);
            GC.Collect(1, GCCollectionMode.Forced);
            GC.Collect(2, GCCollectionMode.Forced);
        }
        public async Task ReadAndEmbedVideoStream(string path, List<byte[]> data, IProgress<long> reportEmbededData, long embedSize, byte[] format, byte[] keystream, byte repeatcount, bool isRGB16, byte[] passHash)
        {
            FileStream readVid = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            using (readVid)
            {
                int fID = 4;
                long seekOffset = 0;
                byte[] fourcc = new byte[fID];
                readVid.Read(fourcc, 0, 4);
                int chunkSize = 0;
                int hops = 0;
                embedsize2 = 0;
                if (embedSize % 2 != 0 && repeatcount != 0)
                {
                    embedsize2 = embedSize + 1;
                }
                else
                {
                    embedsize2 = embedSize;
                }
                embeder = new LSBEmbedderClass(data, isRGB16);
                int metaSize = 0;
                if (repeatcount == 0)
                {
                    metaSize = (cuttle.Length + 1 + 4 + format.Length + 4 + 16 + 4 + passHash.Length);
                }
                else
                {
                    metaSize = (cuttle.Length + 1 + 4 + format.Length + 4 + 2 + 1 + 4 + passHash.Length);
                }
                for (int i = 0; i < jOffsetList.Count; i++)
                {
                    readVid.Seek(jOffsetList[i], SeekOrigin.Begin);
                    readVid.Read(fourcc, 0, 4);
                    if (getEquivalentDecimal(fourcc) >= metaSize && !okCapJunkChunk)
                    {
                        okCapJunkChunk = true;
                        junkoffset = jOffsetList[i] + 4;
                        break;
                    }
                }
                while (hops < vOffsetList.Count)
                {
                    seekOffset = vOffsetList[hops];
                    if (stopOperationFlag)
                    {
                        break;
                    }
                    if (embeder.getEmbeded() != embedsize2)
                    {
                        readVid.Seek(seekOffset, SeekOrigin.Begin);
                        readVid.Read(fourcc, 0, 4);
                        seekOffset += 4;
                        chunkSize = getEquivalentDecimal(fourcc);
                        byte[] container = new byte[chunkSize];
                        readVid.Read(container, 0, chunkSize);
                        container = processChunk(container);
                        readVid.Seek(seekOffset, SeekOrigin.Begin);
                        readVid.Write(container, 0, chunkSize);
                        seekOffset += chunkSize;
                        reportEmbededData.Report(embeder.getEmbeded());
                    }
                    else
                    {
                        if (okCapJunkChunk)
                        {
                            byte flags = 0;
                            if (keystream.Length == 16)
                            {
                                flags= 1;
                            }
                            seekOffset = junkoffset - 4;
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            chunkSize = getEquivalentDecimal(fourcc);
                            readVid.Write(cuttle, 0, cuttle.Length);
                            readVid.WriteByte(flags);
                            readVid.Write(getEquivalentLittleEndian((long)format.Length), 0, 4);
                            readVid.Write(format, 0, format.Length);
                            readVid.Write(getEquivalentLittleEndian(embedSize), 0, 4);
                            readVid.Write(keystream, 0, keystream.Length);
                            if (flags == 0)
                            {
                                readVid.WriteByte(repeatcount);
                            }
                            readVid.Write(hsps, 0, 4);
                            readVid.Write(passHash, 0, passHash.Length);
                            while (metaSize != chunkSize)
                            {
                                readVid.WriteByte(zero0x0);
                                metaSize++;
                            }
                        }
                        else
                        {
                            byte flags = 0;
                            if (keystream.Length == 16)
                            {
                                flags = 1;
                            }
                            seekOffset = readVid.Length;
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Write(cuttle, 0, cuttle.Length);
                            readVid.WriteByte(flags);
                            readVid.Write(getEquivalentLittleEndian((long)format.Length), 0, 4);
                            readVid.Write(format, 0, format.Length);
                            readVid.Write(getEquivalentLittleEndian(embedSize), 0, 4);
                            readVid.Write(keystream, 0, keystream.Length);
                            if (flags == 0)
                            {
                                readVid.WriteByte(repeatcount);
                            }
                            readVid.Write(hsps, 0, 4);
                            readVid.Write(passHash, 0, passHash.Length);
                        }
                        break;
                    }
                    hops++;
                    if (hops % 2 == 0)
                    {
                        await Task.Delay(100);
                    }
                }
            }
            readVid.Dispose();
            resetClasses();
        }
        private byte[] processChunk(byte[] data)
        {
            return embeder.embedChunkData(data);
        }
        public async Task ReadAndExtractVideoStream(string path, IProgress<long> reportExtractedData, long SizeToExtract,bool isRGB16)
        {
            ErrorEncountered = false;
            FileStream readVid = new FileStream(path, FileMode.Open, FileAccess.Read);
            using (readVid)
            {
                int fID = 4;
                long seekOffset = 8;
                readVid.Seek(seekOffset, SeekOrigin.Begin);
                byte[] fourcc = new byte[fID];
                readVid.Read(fourcc, 0, 4);
                seekOffset += 4;
                int chunkSize = 0;
                int id = 0;
                int hops = 0;
                embedsize2 = 0;
                if (SizeToExtract % 2 != 0 && flag[0] == 0)
                {
                    embedsize2 = SizeToExtract + 1;
                }
                else
                {
                    embedsize2 = SizeToExtract;
                }
                if (FouCC(fourcc) == 1)
                {
                    extractor = null;
                    extractor = new LSBExtractorClass(embedsize2, isRGB16);
                    while (seekOffset != readVid.Length)
                    {
                        hops++;
                        if (stopOperationFlag)
                        {
                            break;
                        }
                        readVid.Seek(seekOffset, SeekOrigin.Begin);
                        readVid.Read(fourcc, 0, 4);
                        seekOffset += 4;
                        id = FouCC(fourcc);
                        if (id == 2)
                        {
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            seekOffset += 4;
                            chunkSize = getEquivalentDecimal(fourcc);
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            id = FouCC(fourcc);
                            if (id == 9)
                            {
                                seekOffset += chunkSize;
                            }
                            else if (id == 3)
                            {
                                seekOffset += 4;
                                readVid.Seek(seekOffset, SeekOrigin.Begin);
                                readVid.Read(fourcc, 0, 4);
                                seekOffset += 4;
                                id = FouCC(fourcc);
                                if (id == 4)
                                {
                                    readVid.Seek(seekOffset, SeekOrigin.Begin);
                                    readVid.Read(fourcc, 0, 4);
                                    seekOffset += 4 + getEquivalentDecimal(fourcc);
                                }
                                else if (id == 5)
                                {

                                    if (extractor.getExtracted() < embedsize2)
                                    {
                                        readVid.Seek(seekOffset, SeekOrigin.Begin);
                                        readVid.Read(fourcc, 0, 4);
                                        seekOffset += 4;
                                        chunkSize = getEquivalentDecimal(fourcc);
                                        byte[] container = new byte[chunkSize];
                                        readVid.Read(container, 0, chunkSize);
                                        processChunkExtract(container);
                                        reportExtractedData.Report(getExtracted());
                                        seekOffset += chunkSize;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else if (id == 4 || id == 7 || id == 6)
                        {
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            seekOffset += 4 + getEquivalentDecimal(fourcc);
                        }
                        else if (id == 5)
                        {

                            if (extractor.getExtracted() < embedsize2)
                            {
                                
                                readVid.Seek(seekOffset, SeekOrigin.Begin);
                                readVid.Read(fourcc, 0, 4);
                                seekOffset += 4;
                                chunkSize = getEquivalentDecimal(fourcc);
                                byte[] container = new byte[chunkSize];
                                readVid.Read(container, 0, chunkSize);
                                processChunkExtract(container);
                                reportExtractedData.Report(getExtracted());
                                seekOffset += chunkSize;
                                
                            }
                            else
                            {
                                break;
                            }

                        }
                        else if (id == 8)
                        {
                            seekOffset += 8;
                        }
                        else if (id == 10)
                        {
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            chunkSize = getEquivalentDecimal(fourcc);
                            seekOffset += 4 + chunkSize;

                        }
                        else
                        {
                            break;
                        }
                        if (hops % 2 == 0)
                        {
                            await Task.Delay(100);
                        }
                    }
                }
                else
                {
                    ErrorEncountered = true;
                    MessageBox.Show("Unable to read the video file. File maybe corrupted", "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                  
            }
            readVid.Dispose();
        }
        public long getExtracted()
        {
            return extractor.getExtracted();
        }
        private void processChunkExtract(byte[] data)
        {
            extractor.extractDataFromVideoStream(data);
        }
        public List<byte[]> getExtractedData()
        {
            return extractor.getExtractedStream();
        }
        public async Task readInfo(string path, IProgress<int> Readframes)
        {
            ErrorEncountered = false;
            FileStream readVid = new FileStream(path, FileMode.Open, FileAccess.Read);
            using (readVid)
            {
                resetInfo();
                int fID = 4;
                long seekOffset = 8;
                readVid.Seek(seekOffset, SeekOrigin.Begin);
                byte[] fourcc = new byte[fID];
                readVid.Read(fourcc, 0, 4);
                seekOffset += 4;
                int chunkSize = 0;
                int id = 0;
                int hops = 0;
                stopOperationFlag = false;
                okCapJunkChunk = false;
                junkoffset = 0;
                if (FouCC(fourcc) == 1)
                {
                    byte[] Dword = new byte[fID];
                    byte[] word = new byte[2];
                    readVid.Seek(64, SeekOrigin.Begin);
                    readVid.Read(Dword, 0, 4);
                    width = getEquivalentDecimal(Dword);
                    readVid.Seek(68, SeekOrigin.Begin);
                    readVid.Read(Dword, 0, 4);
                    height = getEquivalentDecimal(Dword);
                    readVid.Seek(188, SeekOrigin.Begin);
                    readVid.Read(Dword, 0, 4);
                    if (getEquivalentDecimal(Dword) == 0)
                    {
                        readVid.Seek(186, SeekOrigin.Begin);
                        readVid.Read(word, 0, 2);
                        int bpp = getEquivalentDecimalInt16(word);
                        vCodec = String.Format("RGB {0} bit", bpp);
                    }
                    else
                    {
                        vCodec = Encoding.ASCII.GetString(Dword);
                    }
                    while (seekOffset != readVid.Length)
                    {
                        hops++;
                        if (stopOperationFlag)
                        {
                            break;
                        }
                        readVid.Seek(seekOffset, SeekOrigin.Begin);
                        readVid.Read(fourcc, 0, 4);
                        seekOffset += 4;
                        id = FouCC(fourcc);
                        if (id == 2)
                        {
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            seekOffset += 4;
                            chunkSize = getEquivalentDecimal(fourcc);
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            id = FouCC(fourcc);
                            if (id == 9)
                            {
                                seekOffset += chunkSize;
                            }
                            else if (id == 3)
                            {
                                seekOffset += 4;
                                readVid.Seek(seekOffset, SeekOrigin.Begin);
                                readVid.Read(fourcc, 0, 4);
                                seekOffset += 4;
                                id = FouCC(fourcc);
                                if (id == 4)
                                {
                                    readVid.Seek(seekOffset, SeekOrigin.Begin);
                                    readVid.Read(fourcc, 0, 4);
                                    seekOffset += 4 + getEquivalentDecimal(fourcc);
                                }
                                else if (id == 5)
                                {
                                    readVid.Seek(seekOffset, SeekOrigin.Begin);
                                    readVid.Read(fourcc, 0, 4);
                                    chunkSize = getEquivalentDecimal(fourcc);
                                    vOffsetList.Add(seekOffset);
                                    seekOffset += 4 + chunkSize;
                                    vSize += chunkSize;
                                    frameCount++;
                                }
                            }
                        }
                        else if (id == 4 || id == 7 || id == 6)
                        {
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            seekOffset += 4 + getEquivalentDecimal(fourcc);
                        }
                        else if (id == 5)
                        {
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            chunkSize = getEquivalentDecimal(fourcc);
                            vOffsetList.Add(seekOffset);
                            seekOffset += 4 + chunkSize;
                            vSize += chunkSize;
                            frameCount++;
                            
                        }
                        else if (id == 8)
                        {
                            seekOffset += 8;
                        }
                        else if (id == 10)
                        {
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            jOffsetList.Add(seekOffset);
                            seekOffset += 4 + getEquivalentDecimal(fourcc);

                        }
                        else
                        {
                            break;
                        }
                        if (hops % 500 == 0)
                        {
                            await Task.Delay(100);
                            Readframes.Report(frameCount);
                        }
                    }
                }
                else
                {
                    ErrorEncountered = true;
                    MessageBox.Show("Unable to read the video file. File maybe corrupted", "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Readframes.Report(frameCount);
                  
            }
            readVid.Dispose();
        }
        public async Task searchForEmbededData(string path, IProgress<int> EmbededFileSize, IProgress<string> extenstion)
        {
            ErrorEncountered = false;
            FileStream readVid = new FileStream(path, FileMode.Open, FileAccess.Read);
            using (readVid)
            {
                resetInfo();
                int fID = 4;
                long seekOffset = 8;
                readVid.Seek(seekOffset, SeekOrigin.Begin);
                byte[] fourcc = new byte[fID];
                readVid.Read(fourcc, 0, 4);
                seekOffset += 4;
                int chunkSize = 0;
                int id = 0;
                int hops = 0;
                stopOperationFlag = false;
                bool EmbededFileFound = false;
                if (FouCC(fourcc) == 1)
                {
                    while (seekOffset != readVid.Length)
                    {
                        hops++;
                        if (stopOperationFlag)
                        {
                            break;
                        }
                        byte[] Dword = new byte[fID];
                        byte[] word = new byte[2];
                        readVid.Seek(188, SeekOrigin.Begin);
                        readVid.Read(Dword, 0, 4);
                        if (getEquivalentDecimal(Dword) == 0)
                        {
                            readVid.Seek(186, SeekOrigin.Begin);
                            readVid.Read(word, 0, 2);
                            int bpp = getEquivalentDecimalInt16(word);
                            vCodec = String.Format("RGB {0} bit", bpp);
                        }
                        else
                        {
                            vCodec = Encoding.ASCII.GetString(Dword);
                        }
                        readVid.Seek(seekOffset, SeekOrigin.Begin);
                        readVid.Read(fourcc, 0, 4);
                        seekOffset += 4;
                        id = FouCC(fourcc);
                        if (id == 2)
                        {
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            seekOffset += 4;
                            chunkSize = getEquivalentDecimal(fourcc);
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            id = FouCC(fourcc);
                            if (id == 9)
                            {
                                seekOffset += chunkSize;
                            }
                            else if (id == 3)
                            {
                                seekOffset += 4;
                                readVid.Seek(seekOffset, SeekOrigin.Begin);
                                readVid.Read(fourcc, 0, 4);
                                seekOffset += 4;
                                id = FouCC(fourcc);
                                if (id == 4)
                                {
                                    readVid.Seek(seekOffset, SeekOrigin.Begin);
                                    readVid.Read(fourcc, 0, 4);
                                    seekOffset += 4 + getEquivalentDecimal(fourcc);
                                }
                                else if (id == 5)
                                {
                                    readVid.Seek(seekOffset, SeekOrigin.Begin);
                                    readVid.Read(fourcc, 0, 4);
                                    chunkSize = getEquivalentDecimal(fourcc);
                                    seekOffset += 4 + chunkSize;
                                    
                                }
                            }
                        }
                        else if (id == 4 || id == 7 || id == 6)
                        {
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            seekOffset += 4 + getEquivalentDecimal(fourcc);
                        }
                        else if (id == 5)
                        {

                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            chunkSize = getEquivalentDecimal(fourcc);
                            seekOffset += 4 + chunkSize;
                        }
                        else if (id == 8)
                        {
                            seekOffset += 8;
                        }
                        else if (id == 10)
                        {
                            metaLocation = seekOffset;
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            if (getEquivalentDecimal(fourcc) > 20)
                            {
                                byte[] confirmSign = new byte[cuttle.Length];
                                readVid.Read(confirmSign, 0, confirmSign.Length);
                                seekOffset += confirmSign.Length;
                                if (ASCIIEncoding.ASCII.GetString(confirmSign) == ASCIIEncoding.ASCII.GetString(cuttle))
                                {
                                    readVid.Read(flag, 0, 1);
                                    seekOffset++;
                                    if (flag[0] == 0)
                                    {
                                        keystreamdata = new byte[2];
                                    }
                                    else
                                    {
                                        keystreamdata = new byte[16];
                                    }
                                    readVid.Read(fourcc, 0, 4);
                                    seekOffset += 4;
                                    int extSize = getEquivalentDecimalNoAdd1(fourcc);
                                    byte[] ext = new byte[extSize];
                                    readVid.Read(ext, 0, extSize);
                                    seekOffset += extSize;
                                    extenstion.Report(ASCIIEncoding.ASCII.GetString(ext));
                                    readVid.Read(fourcc, 0, 4);
                                    EmbededFileSize.Report(getEquivalentDecimalNoAdd1(fourcc));
                                    eSize = getEquivalentDecimalNoAdd1(fourcc);
                                    seekOffset += 4;
                                    readVid.Read(keystreamdata, 0, keystreamdata.Length);
                                    seekOffset += keystreamdata.Length;
                                    if (flag[0] == 0)
                                    {
                                        repeatCount = (byte)readVid.ReadByte();
                                    }
                                    readVid.Read(fourcc, 0, 4);
                                    if (ASCIIEncoding.ASCII.GetString(fourcc) == ASCIIEncoding.ASCII.GetString(hsps))
                                    {
                                        passwordProtected = true;
                                        readVid.Read(Sha256Stream, 0, 32);
                                    }
                                    else
                                    {
                                        passwordProtected = false;
                                    }
                                    EmbededFileFound = true;

                                    break;
                                }
                            }
                            else
                            {
                                seekOffset += 4 + getEquivalentDecimal(fourcc);
                            }
                        }
                        else
                        {
                            break;
                        }
                        if (hops % 500 == 0)
                        {
                            await Task.Delay(100);
                        }
                    }
                }
                else
                {
                    ErrorEncountered = true;
                    MessageBox.Show("Unable to read the video file. File maybe corrupted", "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (EmbededFileFound)
                {
                    hasEmbededFile = true;
                }
                else
                {
                    hasEmbededFile = false;
                }
                
            }
            readVid.Dispose(); 
        }
        public bool checkForEmbeddedFile(string path)
        {
            ErrorEncountered = false;
            FileStream readVid = new FileStream(path, FileMode.Open, FileAccess.Read);
            using (readVid)
            {
                resetInfoExceptvSize();
                int fID = 4;
                long seekOffset = 8;
                readVid.Seek(seekOffset, SeekOrigin.Begin);
                byte[] fourcc = new byte[fID];
                readVid.Read(fourcc, 0, 4);
                seekOffset += 4;
                int chunkSize = 0;
                int id = 0;
                int hops = 0;
                stopOperationFlag = false;
                bool EmbededFileFound = false;
                if (FouCC(fourcc) == 1)
                {
                    while (seekOffset != readVid.Length)
                    {
                        hops++;
                        if (stopOperationFlag)
                        {
                            break;
                        }
                        byte[] Dword = new byte[fID];
                        byte[] word = new byte[2];
                        readVid.Seek(188, SeekOrigin.Begin);
                        readVid.Read(Dword, 0, 4);
                        if (getEquivalentDecimal(Dword) == 0)
                        {
                            readVid.Seek(186, SeekOrigin.Begin);
                            readVid.Read(word, 0, 2);
                            int bpp = getEquivalentDecimalInt16(word);
                            vCodec = String.Format("RGB {0} bit", bpp);
                        }
                        else
                        {
                            vCodec = Encoding.ASCII.GetString(Dword);
                        }
                        readVid.Seek(seekOffset, SeekOrigin.Begin);
                        readVid.Read(fourcc, 0, 4);
                        seekOffset += 4;
                        id = FouCC(fourcc);
                        if (id == 2)
                        {
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            seekOffset += 4;
                            chunkSize = getEquivalentDecimal(fourcc);
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            id = FouCC(fourcc);
                            if (id == 9)
                            {
                                seekOffset += chunkSize;
                            }
                            else if (id == 3)
                            {
                                seekOffset += 4;
                                readVid.Seek(seekOffset, SeekOrigin.Begin);
                                readVid.Read(fourcc, 0, 4);
                                seekOffset += 4;
                                id = FouCC(fourcc);
                                if (id == 4)
                                {
                                    readVid.Seek(seekOffset, SeekOrigin.Begin);
                                    readVid.Read(fourcc, 0, 4);
                                    seekOffset += 4 + getEquivalentDecimal(fourcc);
                                }
                                else if (id == 5)
                                {
                                    readVid.Seek(seekOffset, SeekOrigin.Begin);
                                    readVid.Read(fourcc, 0, 4);
                                    chunkSize = getEquivalentDecimal(fourcc);
                                    seekOffset += 4 + chunkSize;

                                }
                            }
                        }
                        else if (id == 4 || id == 7 || id == 6)
                        {
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            seekOffset += 4 + getEquivalentDecimal(fourcc);
                        }
                        else if (id == 5)
                        {

                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            chunkSize = getEquivalentDecimal(fourcc);
                            seekOffset += 4 + chunkSize;
                        }
                        else if (id == 8)
                        {
                            seekOffset += 8;
                        }
                        else if (id == 10)
                        {
                            metaLocation = seekOffset;
                            readVid.Seek(seekOffset, SeekOrigin.Begin);
                            readVid.Read(fourcc, 0, 4);
                            if (getEquivalentDecimal(fourcc) > 20)
                            {
                                byte[] confirmSign = new byte[cuttle.Length];
                                readVid.Read(confirmSign, 0, confirmSign.Length);
                                seekOffset += confirmSign.Length;
                                if (ASCIIEncoding.ASCII.GetString(confirmSign) == ASCIIEncoding.ASCII.GetString(cuttle))
                                {
                                    readVid.Read(flag, 0, 1);
                                    seekOffset++;
                                    if (flag[0] == 0)
                                    {
                                        keystreamdata = new byte[2];
                                    }
                                    else
                                    {
                                        keystreamdata = new byte[16];
                                    }
                                    readVid.Read(fourcc, 0, 4);
                                    seekOffset += 4;
                                    int extSize = getEquivalentDecimalNoAdd1(fourcc);
                                    byte[] ext = new byte[extSize];
                                    readVid.Read(ext, 0, extSize);
                                    seekOffset += extSize;
                                    readVid.Read(fourcc, 0, 4);
                                    eSize = getEquivalentDecimalNoAdd1(fourcc);
                                    seekOffset += 4;
                                    readVid.Read(keystreamdata, 0, keystreamdata.Length);
                                    seekOffset += keystreamdata.Length;
                                    if (flag[0] == 0)
                                    {
                                        repeatCount = (byte)readVid.ReadByte();
                                    }
                                    readVid.Read(fourcc, 0, 4);
                                    if (ASCIIEncoding.ASCII.GetString(fourcc) == ASCIIEncoding.ASCII.GetString(hsps))
                                    {
                                        passwordProtected = true;
                                        readVid.Read(Sha256Stream, 0, 32);
                                    }
                                    else
                                    {
                                        passwordProtected = false;
                                    }
                                    EmbededFileFound = true;

                                    break;
                                }
                            }
                            else
                            {
                                seekOffset += 4 + getEquivalentDecimal(fourcc);
                            }
                        }
                        else
                        {
                            break;
                        }
                        if (hops % 500 == 0)
                        {

                        }
                    }
                }
                else
                {
                    ErrorEncountered = true;
                    MessageBox.Show("Unable to read the video file. File maybe corrupted", "Cuttlefish", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (EmbededFileFound)
                {
                    readVid.Dispose(); 
                    return true;
                }
                else
                {
                    readVid.Dispose(); 
                    return false;
                }

            }
            
        }
        public async Task RemoveMetadata(string path)
        {
            FileStream readVid = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            using (readVid)
            {
                int fID = 4;
                long seekOffset = metaLocation;
                readVid.Seek(seekOffset, SeekOrigin.Begin);
                byte[] fourcc = new byte[fID];
                readVid.Read(fourcc, 0, 4);
                seekOffset += 4;
                int chunkSize = getEquivalentDecimal(fourcc);
                for (int i = 0; i < chunkSize; i++)
                {
                    readVid.WriteByte(zero0x0);
                    if (i % 4 == 0)
                    {
                        await Task.Delay(10);
                    }
                    if (i == 30)
                    {
                        break;
                    }
                }
            }
                
        }
        private int FouCC(byte[] data)
        {
            string fourccTag = System.Text.Encoding.UTF8.GetString(data);
            if (fourccTag == "AVI ")
            {
                
                return 1;
                
            }
            else if (fourccTag == "LIST")
            {
                
                return 2;
            }
            else if (fourccTag == "movi")
            {
               
                return 3;
            }
            else if (fourccTag.Contains("wb"))
            {
                
                return 4;
            }
            else if (fourccTag.Contains("db") || fourccTag.Contains("dc"))
            {
               
                return 5;
            }
            else if (fourccTag.Contains("ix"))
            {
                return 6;
            }
            else if (fourccTag == "idx1")
            {
                return 7;
            }
            else if (fourccTag == "RIFF")
            {
                return 8;
            }
            else if (fourccTag == "hdrl" || fourccTag == "strl" || fourccTag == "odml" || fourccTag == "INFO")
            {
                return 9;
            }
            else if (fourccTag == "JUNK")
            {
                return 10;
            }
            else
            {
                return 0;
            }
            
        }
        private int getEquivalentDecimal(byte[] data)
        {
            int size = 0;
            for(int i = 0; i < data.Length; i++)
            {
                size += data[i] * (int)Math.Pow(256, i);
            }
            if (size % 2 != 0)
            {
                size += 1;
            }
            return size;
        }
        private int getEquivalentDecimalNoAdd1(byte[] data)
        {
            int size = 0;
            for (int i = 0; i < data.Length; i++)
            {
                size += data[i] * (int)Math.Pow(256, i);
            }
            return size;
        }
        private int getEquivalentDecimalInt16(byte[] data)
        {
            int size = 0;
            for (int i = 0; i < data.Length; i++)
            {
                size += data[i] * (int)Math.Pow(256, i);
            }
            return size;
        }
        private byte[] getEquivalentLittleEndian(long size)
        {
            byte[] dataSize = BitConverter.GetBytes(size);
            if (BitConverter.IsLittleEndian)
            {
                return dataSize;
            }
            else
            {
                byte[] temp = new byte[4];
                for (int i = 0; i < dataSize.Length; i++)
                {
                    temp[i] = dataSize[3 - i];
                }
                return temp;
            }
        }
        public void stop()
        {
            stopOperationFlag = true;
        }
        public string getVCodec()
        {
            return vCodec;
        }
        public string dimension()
        {
            return String.Format("{0}x{1}", width, height);
        }
        public string getFrames()
        {
            return "" + frameCount;
        }
        private void resetInfo()
        {
            vCodec = "";
            width = 0;
            height = 0;
            frameCount = 0;
            vSize = 0;
            vOffsetList = new List<long>();
            jOffsetList = new List<long>();
        }
        private void resetInfoExceptvSize()
        {
            vCodec = "";
            width = 0;
            height = 0;
        }
        public long getVideoStreamSize()
        {
            return vSize;
        }
        public bool HasEmbededFile()
        {
            return hasEmbededFile;
        }
        public byte[] getKeyStream()
        {
            return keystreamdata;
        }
        public byte getRepeatCount()
        {
            return repeatCount;
        }
        public int getESize()
        {
            return eSize;
        }
    }
}
