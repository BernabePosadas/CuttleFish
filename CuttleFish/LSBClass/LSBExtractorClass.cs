using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuttleFish
{
    class LSBExtractorClass
    {
        List<byte[]> stream = new List<byte[]>();
        GlobalVar gVars = null;
        bool RGB16Flag = false;
        public LSBExtractorClass(long Size, bool isRGB16)
        {
            gVars = new GlobalVar();
            RGB16Flag = isRGB16;
            byte[] Chunk = null; 
            int chunk100MB = 1024 * 1024 * 10;
            if(Size < chunk100MB)
            {
                Chunk = new byte[Size];
                stream.Add(Chunk);
            }
            else
            {
                while(Size > 0)
                {
                    Chunk = null;
                    if (Size < chunk100MB)
                    {
                        Chunk = new byte[Size];
                        stream.Add(Chunk);
                        Size = 0;
                    }
                    else
                    {
                        Chunk = new byte[chunk100MB];
                        stream.Add(Chunk);
                        Size -= chunk100MB;
                    }
                }
            }
        }
        public void extractDataFromVideoStream(byte[] chunkStream)
        {
            int i = 0;
            byte[] streamData = stream[gVars.dataIndexPos];
            byte[] tempContainer = new byte[4];
            while (i < chunkStream.Length)
            {
                if (gVars.partial)
                {
                    int x = 0;
                    for (int j = 0; j < gVars.remainderBytes.Length; j++)
                    {
                        tempContainer[j] = gVars.remainderBytes[j];
                        x++;
                    }
                    for (int j = 0; j < x; j++)
                    {
                        tempContainer[j+ x] = chunkStream[i + j];
                    }
                    streamData[gVars.datapos] = extractData(tempContainer);
                    gVars.partial = false;
                    gVars.datapos += 1;
                    gVars.extracted += 1;
                    i += (4 - x);
                }
                if ((chunkStream.Length - i) < 4 && (chunkStream.Length - i) != 0)
                {
                    byte[] temp2 = new byte[(chunkStream.Length - i)];
                    for (int j = 0; j < temp2.Length; j++)
                    {
                        temp2[j] = chunkStream[i + j];
                    }
                    gVars.remainderBytes = temp2;
                    gVars.partial = true;
                    i += (chunkStream.Length - i);
                }
                else
                {
                    for (int j = 0; j < 4; j++)
                    {
                        tempContainer[j] = chunkStream[i + j];
                    }
                    streamData[gVars.datapos] = extractData(tempContainer);
                    gVars.datapos += 1;
                    gVars.extracted += 1;
                    i += 4;
                }
                if (streamData.Length == gVars.datapos)
                {
                    stream[gVars.dataIndexPos] = streamData;
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
        private byte extractData(byte[] streamData)
        {
            byte a = 0;
            byte b = 0;
            byte c = 0;
            byte d = 0;
            if (RGB16Flag)
            {
                byte temp = 0;
                a = (byte)(streamData[0] >> 7);
                a = (byte)(a << 7);
                temp = (byte)(streamData[0] >> 6);
                temp = (byte)(temp << 7);
                a += (byte)(temp >> 1);
                temp = (byte)(streamData[0] >> 1);
                temp = (byte)(temp << 7);
                a += (byte)(temp >> 2);

                b = (byte)(streamData[1] << 7);

                c = (byte)(streamData[2] >> 7);
                c = (byte)(c << 7);
                temp = (byte)(streamData[2] >> 6);
                temp = (byte)(temp << 7);
                c += (byte)(temp >> 1);
                temp = (byte)(streamData[2] >> 1);
                temp = (byte)(temp << 7);
                c += (byte)(temp >> 2);

                d = (byte)(streamData[3] << 7);
                return (byte)(a + (b >> 3) + (c >> 4) + (d >> 7));
            }
            else
            {
                a = (byte)(streamData[0] << 6);
                b = (byte)(streamData[1] << 6);
                c = (byte)(streamData[2] << 6);
                d = (byte)(streamData[3] << 6);
                return (byte)(a + (b >> 2) + (c >> 4) + (d >> 6));
            }

        }
        public long getExtracted()
        {
            return gVars.extracted;
        }
        public List<byte[]> getExtractedStream()
        {
            return stream;
        }
    }
}
