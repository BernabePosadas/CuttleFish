using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CuttleFish
{
    class EBCEA
    {
        byte key8 = 0, key16 = 0;
        byte repeatCount;
        byte[] previousBlocks = new byte[2];
        bool firstRun = true;
        public EBCEA(byte[] keystream, byte keyRepeatCount)
        {
            key8 = keystream[0];
            key16 = keystream[1];
            repeatCount = keyRepeatCount;
        }
        public byte[] encyptData(byte[] data)
        {
                for (int j = 0; j < data.Length; j += 2)
                {
                    if (!firstRun)
                    {
                        data[j] = (byte)(data[j] ^ previousBlocks[0]);
                        data[j + 1] = (byte)(data[j + 1] ^ previousBlocks[1]);
                    }
                    byte a = 0, b = 0;
                    for (int k = 1; k <= repeatCount; k++)
                    {
                        a = data[j + 1];
                        b = data[j];
                        a -= key8;
                        b -= key16;
                        data[j] = a;
                        data[j + 1] = b;
                        if (repeatCount % 2 == 0)
                        {
                            data[j] ^= (byte)((key8 + k)  ^ (key16 - k));
                            data[j + 1] ^= (byte)((key8 + k) ^ (key16 - k));
                        }
                        else
                        {
                            data[j] ^= (byte)((key8 - k) ^ (key16 + k));
                            data[j + 1] ^= (byte)((key8 - k) ^ (key16 + k));
                        }
                        
                        
                    }
                    previousBlocks[0] = data[j];
                    previousBlocks[1] = data[j + 1];
                    if (firstRun)
                    {
                        firstRun = false;
                    }
                }
                return data;
        }
        public byte[] encyptData(byte[] data, string password)
        {
            byte[] passStream = Encoding.ASCII.GetBytes(password);
            int maxPointer = passStream.Length;
            int cPointer = 0;
            for (int j = 0; j < data.Length; j += 2)
            {
                if (!firstRun)
                {
                    data[j] = (byte)(data[j] ^ previousBlocks[0]);
                    data[j + 1] = (byte)(data[j + 1] ^ previousBlocks[1]);
                }
                byte a = 0, b = 0;
                for (int k = 1; k <= repeatCount; k++)
                {
                    
                    a = data[j + 1];
                    b = data[j];
                    a -= key8;
                    b -= key16;
                    data[j] = a;
                    data[j + 1] = b;
                    if (repeatCount % 2 == 0)
                    {
                        data[j] ^= (byte)((key8 + k) ^ (key16 - k));
                        data[j + 1] ^= (byte)((key8 + k) ^ (key16 - k));
                    }
                    else
                    {
                        data[j] ^= (byte)((key8 - k) ^ (key16 + k));
                        data[j + 1] ^= (byte)((key8 - k) ^ (key16 + k));
                    }
                }
                data[j] ^= (byte)passStream[cPointer];
                cPointer++;
                if (cPointer == maxPointer)
                {
                    cPointer = 0;
                }
                data[j + 1] ^= (byte)passStream[cPointer];
                cPointer++;
                if (cPointer == maxPointer)
                {
                    cPointer = 0;
                }
                previousBlocks[0] = data[j];
                previousBlocks[1] = data[j + 1];
                if (firstRun)
                {
                    firstRun = false;
                }

            }
            return data;
        }
        public byte[] DecryptData(byte[] data)
        {
            for (int i = 0; i < data.Length; i+=2)
            {
                byte[] NextPreviousBlock = new byte[2]; 
                if (firstRun)
                {
                    previousBlocks[0] = data[i];
                    previousBlocks[1] = data[i + 1];
                }
                else
                {
                    NextPreviousBlock[0] = data[i];
                    NextPreviousBlock[1] = data[i + 1];
                }
                  byte a = 0, b = 0;
                  for (int k = repeatCount; k >= 1; k--)
                  {

                      if (repeatCount % 2 == 0)
                      {
                          data[i] ^= (byte)((key8 + k) ^ (key16 - k));
                          data[i + 1] ^= (byte)((key8 + k) ^ (key16 - k));
                      }
                      else
                      {
                          data[i] ^= (byte)((key8 - k) ^ (key16 + k));
                          data[i + 1] ^= (byte)((key8 - k) ^ (key16 + k));
                      }
                      a = data[i];
                      b = data[i + 1];
                      a += key8;
                      b += key16;
                      data[i] = b;
                      data[i + 1] = a;
                      
                }
                if (!firstRun)
                {
                      data[i] = (byte)(data[i] ^ previousBlocks[0]);
                      data[i + 1] = (byte)(data[i + 1] ^ previousBlocks[1]);
                      previousBlocks = NextPreviousBlock;
                }
                if (firstRun)
                {
                    firstRun = false;
                }
  
            }
            return data;
        }
        public byte[] DecryptData(byte[] data, string password)
        {
            byte[] passStream = Encoding.ASCII.GetBytes(password);
            int maxPointer = passStream.Length;
            int cPointer = 0;
            for (int i = 0; i < data.Length; i += 2)
            {
                byte[] NextPreviousBlock = new byte[2];
                if (firstRun)
                {
                    previousBlocks[0] = data[i];
                    previousBlocks[1] = data[i + 1];
                }
                else
                {
                    NextPreviousBlock[0] = data[i];
                    NextPreviousBlock[1] = data[i + 1];
                }
                data[i] ^= (byte)passStream[cPointer];
                cPointer++;
                if (cPointer == maxPointer)
                {
                    cPointer = 0;
                }
                data[i + 1] ^= (byte)passStream[cPointer];
                cPointer++;
                if (cPointer == maxPointer)
                {
                    cPointer = 0;
                }
                byte a = 0, b = 0;
                for (int k = repeatCount; k >= 1; k--)
                {
                    if (repeatCount % 2 == 0)
                    {
                        data[i] ^= (byte)((key8 + k) ^ (key16 - k));
                        data[i + 1] ^= (byte)((key8 + k) ^ (key16 - k));
                    }
                    else
                    {
                        data[i] ^= (byte)((key8 - k) ^ (key16 + k));
                        data[i + 1] ^= (byte)((key8 - k) ^ (key16 + k));
                    }
                    a = data[i];
                    b = data[i + 1];
                    a += key8;
                    b += key16;
                    data[i] = b;
                    data[i + 1] = a;
                }
                if (!firstRun)
                {
                    data[i] = (byte)(data[i] ^ previousBlocks[0]);
                    data[i + 1] = (byte)(data[i + 1] ^ previousBlocks[1]);
                    previousBlocks = NextPreviousBlock;
                }
                if (firstRun)
                {
                    firstRun = false;
                }
            }
            return data;
        }
    
    }
}
