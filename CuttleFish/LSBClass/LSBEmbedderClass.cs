using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuttleFish

{
    class LSBEmbedderClass
    {
        List<byte[]> stream = new List<byte[]>();
        GlobalVar gVars = null;
        bool RGB16Flag = false;
        public LSBEmbedderClass(List<byte[]> datastream, bool isRGB16)
        {
            gVars = new GlobalVar();
            stream = datastream;
            RGB16Flag = isRGB16;
        }
        public byte[] embedChunkData(byte[] chunkStream)
        {
            int i = 0;
            byte[] streamData = stream[gVars.dataIndexPos];
            byte[] tempContainer = null;
            while (i < chunkStream.Length)
            {
                if (!RGB16Flag)
                {
                    if (gVars.partial)
                    {
                        gVars.flag -= 4;
                        tempContainer = new byte[gVars.flag];
                        for (int j = 0; j < gVars.flag; j++)
                        {
                            tempContainer[j] = chunkStream[i + j];
                        }
                        tempContainer = EmbedData(tempContainer, streamData[gVars.datapos], gVars.flag);
                        for (int j = 0; j < gVars.flag; j++)
                        {
                            chunkStream[i + j] = tempContainer[j];
                        }
                        i += gVars.flag;
                        gVars.flag = 0;
                        gVars.partial = false;
                        gVars.datapos += 1;
                        gVars.embeded += 1;

                    }
                    if ((chunkStream.Length - i) < 4 && (chunkStream.Length - i) != 0)
                    {
                        gVars.flag = 8 - (chunkStream.Length - i);
                        tempContainer = new byte[(chunkStream.Length - i)];
                        for (int j = 0; j < (chunkStream.Length - i); j++)
                        {
                            tempContainer[j] = chunkStream[i + j];
                        }
                        tempContainer = EmbedData(tempContainer, streamData[gVars.datapos], gVars.flag);
                        for (int j = 0; j < (chunkStream.Length - i); j++)
                        {
                            chunkStream[i + j] = tempContainer[j];
                        }
                        i += (chunkStream.Length - i);
                        gVars.partial = true;
                    }
                    else
                    {
                        tempContainer = new byte[4];
                        for (int j = 0; j < 4; j++)
                        {
                            tempContainer[j] = chunkStream[i + j];
                        }
                        tempContainer = EmbedData(tempContainer, streamData[gVars.datapos], gVars.flag);
                        for (int j = 0; j < 4; j++)
                        {
                            chunkStream[i + j] = tempContainer[j];
                        }
                        i += 4;
                        gVars.datapos += 1;
                        gVars.embeded += 1;
                    }
                    if (streamData.Length == gVars.datapos)
                    {
                        gVars.datapos = 0;
                        gVars.dataIndexPos += 1;
                        if (gVars.dataIndexPos != stream.Count)
                        {
                            streamData = stream[gVars.dataIndexPos];
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    if (gVars.partial)
                    {
                        gVars.flag -= 16;
                        tempContainer = new byte[gVars.flag];
                        for (int j = 0; j < gVars.flag; j++)
                        {
                            tempContainer[j] = chunkStream[i + j];
                        }
                        tempContainer = EmbedData(tempContainer, streamData[gVars.datapos], gVars.flag);
                        for (int j = 0; j < gVars.flag; j++)
                        {
                            chunkStream[i + j] = tempContainer[j];
                        }
                        i += gVars.flag;
                        gVars.flag = 0;
                        gVars.partial = false;
                        gVars.datapos += 1;
                        gVars.embeded += 1;

                    }
                    if ((chunkStream.Length - i) < 16 && (chunkStream.Length - i) != 0)
                    {
                        gVars.flag = 32 - (chunkStream.Length - i);
                        tempContainer = new byte[(chunkStream.Length - i)];
                        for (int j = 0; j < (chunkStream.Length - i); j++)
                        {
                            tempContainer[j] = chunkStream[i + j];
                        }
                        tempContainer = EmbedData(tempContainer, streamData[gVars.datapos], gVars.flag);
                        for (int j = 0; j < (chunkStream.Length - i); j++)
                        {
                            chunkStream[i + j] = tempContainer[j];
                        }
                        i += (chunkStream.Length - i);
                        gVars.partial = true;
                    }
                    else
                    {
                        tempContainer = new byte[16];
                        for (int j = 0; j < 16; j++)
                        {
                            tempContainer[j] = chunkStream[i + j];
                        }
                        tempContainer = EmbedData(tempContainer, streamData[gVars.datapos], gVars.flag);
                        for (int j = 0; j < 16; j++)
                        {
                            chunkStream[i + j] = tempContainer[j];
                        }
                        i += 16;
                        gVars.datapos += 1;
                        gVars.embeded += 1;
                    }
                    if (streamData.Length == gVars.datapos)
                    {
                        gVars.datapos = 0;
                        gVars.dataIndexPos += 1;
                        if (gVars.dataIndexPos != stream.Count)
                        {
                            streamData = stream[gVars.dataIndexPos];
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return chunkStream;
        }
        public long getEmbeded()
        {
            return gVars.embeded;
        }
        public byte[] EmbedData(byte[] stream, byte data, int mode)
        {
            byte[] embededStream = null;
            if (mode == 3)
            {
                if (RGB16Flag)
                {
                    embededStream = new byte[3];
                    bool[] temp1 = byteTo8Bit(data);
                    bool[] temp2 = byteTo8Bit(stream[0]);
                    bool[] temp3 = byteTo8Bit(stream[1]);
                    bool[] temp4 = byteTo8Bit(stream[2]);
                    temp3[0] = temp1[7];
                    embededStream[0] = bits8ToByte(temp2);
                    embededStream[1] = bits8ToByte(temp3);
                    embededStream[2] = bits8ToByte(temp4);
                }
                else
                {
                    embededStream = new byte[3];
                    bool[] temp1 = byteTo8Bit(data);
                    bool[] temp2 = byteTo8Bit(stream[0]);
                    bool[] temp3 = byteTo8Bit(stream[1]);
                    bool[] temp4 = byteTo8Bit(stream[2]);
                    temp2[6] = temp1[2];
                    temp2[7] = temp1[3];
                    temp3[6] = temp1[4];
                    temp3[7] = temp1[5];
                    temp4[6] = temp1[6];
                    temp4[7] = temp1[7];
                    embededStream[0] = bits8ToByte(temp2);
                    embededStream[1] = bits8ToByte(temp3);
                    embededStream[2] = bits8ToByte(temp4);
                }
            }
            else if (mode == 2)
            {
                if (RGB16Flag)
                {
                        embededStream = new byte[2];
                        bool[] temp1 = byteTo8Bit(data);
                        bool[] temp2 = byteTo8Bit(stream[0]);
                        bool[] temp3 = byteTo8Bit(stream[1]);
                        temp2[0] = temp1[7];
                        embededStream[0] = bits8ToByte(temp2); 
                        embededStream[1] = bits8ToByte(temp3);
                    

                }
                else
                {
                    embededStream = new byte[2];
                    bool[] temp1 = byteTo8Bit(data);
                    bool[] temp2 = byteTo8Bit(stream[0]);
                    bool[] temp3 = byteTo8Bit(stream[1]);
                    temp2[6] = temp1[4];
                    temp2[7] = temp1[5];
                    temp3[6] = temp1[6];
                    temp3[7] = temp1[7];
                    embededStream[0] = bits8ToByte(temp2);
                    embededStream[1] = bits8ToByte(temp3);
                }
            }
            else if (mode == 1)
            {
                if (RGB16Flag)
                {
                    embededStream = new byte[1];
                    embededStream[0] = stream[0];
                }
                else
                {
                    embededStream = new byte[1];
                    bool[] temp1 = byteTo8Bit(data);
                    bool[] temp2 = byteTo8Bit(stream[0]);
                    temp2[6] = temp1[6];
                    temp2[7] = temp1[7];
                    embededStream[0] = bits8ToByte(temp2);
                }
            }
            else if (mode == 5)
            {
                if (RGB16Flag)
                {
                    embededStream = new byte[5];
                    bool[] temp1 = byteTo8Bit(data);
                    bool[] temp2 = byteTo8Bit(stream[1]);
                    bool[] temp3 = byteTo8Bit(stream[3]);
                    temp2[0] = temp1[6];
                    temp3[1] = temp1[7];
                    embededStream[0] = stream[0];
                    embededStream[1] = bits8ToByte(temp2);
                    embededStream[2] = stream[2];
                    embededStream[3] = bits8ToByte(temp3);
                    embededStream[4] = stream[4];
                }
                else
                {
                    embededStream = new byte[3];
                    bool[] temp1 = byteTo8Bit(data);
                    bool[] temp2 = byteTo8Bit(stream[0]);
                    bool[] temp3 = byteTo8Bit(stream[1]);
                    bool[] temp4 = byteTo8Bit(stream[2]);
                    temp2[6] = temp1[0];
                    temp2[7] = temp1[1];
                    temp3[6] = temp1[2];
                    temp3[7] = temp1[3];
                    temp4[6] = temp1[4];
                    temp4[7] = temp1[5];
                    embededStream[0] = bits8ToByte(temp2);
                    embededStream[1] = bits8ToByte(temp3);
                    embededStream[2] = bits8ToByte(temp4);
                }
            }
            else if (mode == 6)
            {
                if (RGB16Flag)
                {
                    embededStream = new byte[6];
                    bool[] temp1 = byteTo8Bit(data);
                    bool[] temp2 = byteTo8Bit(stream[0]);
                    bool[] temp3 = byteTo8Bit(stream[2]);
                    bool[] temp4 = byteTo8Bit(stream[4]);
                    temp2[0] = temp1[5];
                    temp3[0] = temp1[6];
                    temp4[0] = temp1[7];
                    embededStream[0] = bits8ToByte(temp2);
                    embededStream[1] = stream[1];
                    embededStream[2] = bits8ToByte(temp3);
                    embededStream[3] = stream[3];
                    embededStream[2] = bits8ToByte(temp4);
                    embededStream[5] = stream[5];
                }
                else
                {
                    embededStream = new byte[2];
                    bool[] temp1 = byteTo8Bit(data);
                    bool[] temp2 = byteTo8Bit(stream[0]);
                    bool[] temp3 = byteTo8Bit(stream[1]);
                    temp2[6] = temp1[0];
                    temp2[7] = temp1[1];
                    temp3[6] = temp1[2];
                    temp3[7] = temp1[3];
                    embededStream[0] = bits8ToByte(temp2);
                    embededStream[1] = bits8ToByte(temp3);
                }
            }
            else if (mode == 7)
            {
                if (RGB16Flag)
                {
                    embededStream = new byte[7];
                    bool[] temp1 = byteTo8Bit(data);
                    bool[] temp2 = byteTo8Bit(stream[1]);
                    bool[] temp3 = byteTo8Bit(stream[3]);
                    bool[] temp4 = byteTo8Bit(stream[5]);
                    temp2[0] = temp1[5];
                    temp3[0] = temp1[6];
                    temp4[0] = temp1[7];
                    embededStream[0] = stream[0];
                    embededStream[1] = bits8ToByte(temp2);
                    embededStream[2] = stream[2];
                    embededStream[3] = bits8ToByte(temp3);
                    embededStream[4] = stream[4];
                    embededStream[5] = bits8ToByte(temp4);
                    embededStream[6] = stream[6];
                }
                else
                {
                    embededStream = new byte[4];
                    bool[] temp1 = byteTo8Bit(data);
                    bool[] temp2 = byteTo8Bit(stream[0]);
                    temp2[0] = temp1[0];
                    temp2[5] = temp1[1];
                    embededStream[0] = bits8ToByte(temp2);
                }
            }
            else
            {
                if (RGB16Flag)
                {
                    embededStream = new byte[16];
                    bool[] temp1 = byteTo8Bit(data);
                    bool[] temp2 = byteTo8Bit(stream[1]);
                    bool[] temp3 = byteTo8Bit(stream[3]);
                    bool[] temp4 = byteTo8Bit(stream[5]);
                    bool[] temp5 = byteTo8Bit(stream[7]);
                    bool[] temp6 = byteTo8Bit(stream[9]);
                    bool[] temp7 = byteTo8Bit(stream[11]);
                    bool[] temp8 = byteTo8Bit(stream[13]);
                    bool[] temp9 = byteTo8Bit(stream[15]);
                    temp2[7] = temp1[0];
                    temp3[7] = temp1[1];
                    temp4[7] = temp1[2];
                    temp5[7] = temp1[3];
                    temp6[7] = temp1[4];
                    temp7[7] = temp1[5];
                    temp8[7] = temp1[6];
                    temp9[7] = temp1[7];
                    embededStream[1] = bits8ToByte(temp2);
                    embededStream[0] = stream[0];
                    embededStream[1] = bits8ToByte(temp3);
                    embededStream[2] = stream[2];
                    embededStream[5] = bits8ToByte(temp4);
                    embededStream[4] = stream[4];
                    embededStream[7] = bits8ToByte(temp5);
                    embededStream[6] = stream[6];
                    embededStream[9] = bits8ToByte(temp6);
                    embededStream[8] = stream[8];
                    embededStream[11] = bits8ToByte(temp7);
                    embededStream[10] = stream[10];
                    embededStream[12] = bits8ToByte(temp8);
                    embededStream[13] = stream[13];
                    embededStream[15] = bits8ToByte(temp9);
                    embededStream[14] = stream[14];
                }
                else
                {
                    embededStream = new byte[4];
                    bool[] temp1 = byteTo8Bit(data);
                    bool[] temp2 = byteTo8Bit(stream[0]);
                    bool[] temp3 = byteTo8Bit(stream[1]);
                    bool[] temp4 = byteTo8Bit(stream[2]);
                    bool[] temp5 = byteTo8Bit(stream[3]);
                    temp2[6] = temp1[0];
                    temp2[7] = temp1[1];
                    temp3[6] = temp1[2];
                    temp3[7] = temp1[3];
                    temp4[6] = temp1[4];
                    temp4[7] = temp1[5];
                    temp5[6] = temp1[6];
                    temp5[7] = temp1[7];
                    embededStream[0] = bits8ToByte(temp2);
                    embededStream[1] = bits8ToByte(temp3);
                    embededStream[2] = bits8ToByte(temp4);
                    embededStream[3] = bits8ToByte(temp5);
                }
            }
            return embededStream;
        }
        private bool[] byteTo8Bit(byte data)
        {
            bool[] bits = new bool[8];
            int n = 0;
            int pos = 7;
            while (n < 8)
            {
                if ((data & (1 << n)) != 0)
                {
                    bits[pos] = true;
                }
                else
                {
                    bits[pos] = false;
                }
                pos--;
                n++;
            }
            return bits;
        }
        private byte bits8ToByte(bool[] bits)
        {
            byte bytes = 0;
            for (int i = 0; i < 8; i++)
            {
                if (bits[i])
                {
                    bytes += (byte)Math.Pow(2, 7 - i);
                }
            }
            return bytes;
        }
    }
}
