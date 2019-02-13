using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuttleFish
{
    class newEnchiperProto
    {
        byte[,] keyStream = null, keyStreamIn = null;
        int sCounter = 0;
        ConverterClass.recToLinearArray rtl = new ConverterClass.recToLinearArray();
        bool firstRun = true;
        byte[,] previousBlock = new byte[4, 4];
        public newEnchiperProto(byte[,] key)
        {
            keyStream = key;
            keyStreamIn = key;
        }
        public byte[] getLinearArrayFormatKey()
        {
            return rtl.convert4x4RegArrayToLinearArray(keyStreamIn);
        }
        public byte[] decrypt(byte[] data)
        {
            int counter = 0;
            while (counter != data.Length)
            {
                if ((data.Length - counter) < 16)
                {
                    byte[] temp = new byte[data.Length - counter];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        temp[i] = data[counter + i];
                    }
                    temp = alternatePhase2Rev(temp);
                    temp = alternatePhase1Rev(temp);
                    for (int i = 0; i < temp.Length; i++)
                    {
                        data[counter + i] = temp[i];
                    }
                    counter += (data.Length - counter);
                }
                else
                {
                    byte[,] nextPreviousBlock = new byte[4, 4];
                    byte[] temp = new byte[16];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        temp[i] = data[counter + i];
                    }
                    byte[,] dataTable = rtl.convertTo4x4RectArray(temp);
                    if (firstRun)
                    {
                        previousBlock = rtl.convertTo4x4RectArray(temp);
                    }
                    else
                    {
                        nextPreviousBlock = rtl.convertTo4x4RectArray(temp);
                    }
                    dataTable = Phase2Rev(dataTable);
                    dataTable = verticalShiftRev(dataTable);
                    dataTable = horizontalShiftRev(dataTable);
                    dataTable = gearLikeShiftRev(dataTable);
                    if (!firstRun)
                    {
                        dataTable = XORBlocks(dataTable, previousBlock);
                        previousBlock = nextPreviousBlock;
                    }
                    if (firstRun)
                    {
                        firstRun = false;
                    }
                    temp = rtl.convert4x4RegArrayToLinearArray(dataTable);
                    for (int i = 0; i < temp.Length; i++)
                    {
                        data[counter + i] = temp[i];
                    }
                    counter += 16;
                }
                if (sCounter == 0)
                {
                    keyStream = gearLikeShift(keyStream);
                    sCounter++;
                }
                else if (sCounter == 1)
                {
                    keyStream = horizontalShift(keyStream);
                    sCounter++;
                }
                else if (sCounter == 2)
                {
                    keyStream = verticalShiftRev(keyStream);
                    sCounter = 0;
                }
            }
            return data;
        }
        public byte[] decrypt(byte[] data, string password)
        {
            int counter = 0;
            byte[] passStream = Encoding.ASCII.GetBytes(password);
            int maxPointer = passStream.Length;
            int cPointer = 0;
            while (counter != data.Length)
            {
                if ((data.Length - counter) < 16)
                {
                    byte[] temp = new byte[data.Length - counter];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        temp[i] = (byte)(data[counter + i] ^ passStream[cPointer]);
                        cPointer++;
                        if (cPointer == maxPointer)
                        {
                            cPointer = 0;
                        }
                    }
                    temp = alternatePhase2Rev(temp);
                    temp = alternatePhase1Rev(temp);
                    for (int i = 0; i < temp.Length; i++)
                    {
                        data[counter + i] = temp[i];
                    }
                    counter += (data.Length - counter);
                }
                else
                {
                    byte[,] nextPreviousBlock = new byte[4, 4];
                    byte[] temp = new byte[16];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        temp[i] = (byte)(data[counter + i] ^ passStream[cPointer]);
                        cPointer++;
                        if (cPointer == maxPointer)
                        {
                            cPointer = 0;
                        }
                    }
                    byte[,] dataTable = rtl.convertTo4x4RectArray(temp);
                    if (firstRun)
                    {
                        previousBlock = rtl.convertTo4x4RectArray(temp);
                    }
                    else
                    {
                        nextPreviousBlock = rtl.convertTo4x4RectArray(temp);
                    }
                    dataTable = Phase2Rev(dataTable);
                    dataTable = verticalShiftRev(dataTable);
                    dataTable = horizontalShiftRev(dataTable);
                    dataTable = gearLikeShiftRev(dataTable);
                    if (!firstRun)
                    {
                        dataTable = XORBlocks(dataTable, previousBlock);
                        previousBlock = nextPreviousBlock;
                    }
                    if (firstRun)
                    {
                        firstRun = false;
                    }
                    temp = rtl.convert4x4RegArrayToLinearArray(dataTable);
                    for (int i = 0; i < temp.Length; i++)
                    {
                        data[counter + i] = temp[i];
                    }
                    counter += 16;
                }
                if (sCounter == 0)
                {
                    keyStream = gearLikeShift(keyStream);
                    sCounter++;
                }
                else if (sCounter == 1)
                {
                    keyStream = horizontalShift(keyStream);
                    sCounter++;
                }
                else if (sCounter == 2)
                {
                    keyStream = verticalShiftRev(keyStream);
                    sCounter = 0;
                }
            }
            return data;
        }
        public byte[] encrypt(byte[] data)
        {
            int counter = 0;
            while (counter != data.Length)
            {
                if ((data.Length - counter) < 16)
                {
                    byte[] temp = new byte[data.Length - counter];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        temp[i] = data[counter + i];
                    }
                    temp = alternatePhase1(temp);
                    temp = alternatePhase2(temp);
                    for (int i = 0; i < temp.Length; i++)
                    {
                        data[counter + i] = temp[i];
                    }
                    counter += (data.Length - counter);
                }
                else
                {
                    byte[] temp = new byte[16];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        temp[i] = data[counter + i];
                    }
                    byte[,] dataTable = rtl.convertTo4x4RectArray(temp);
                    if (!firstRun)
                    {
                        dataTable = XORBlocks(previousBlock, dataTable);
                    }
                    dataTable = gearLikeShift(dataTable);
                    dataTable = horizontalShift(dataTable);
                    dataTable = verticalShift(dataTable);
                    dataTable = Phase2(dataTable);
                    previousBlock = dataTable;
                    if (firstRun)
                    {
                        firstRun = false;
                    }
                    temp = rtl.convert4x4RegArrayToLinearArray(dataTable);
                    for (int i = 0; i < temp.Length; i++)
                    {
                        data[counter + i] = temp[i];
                    }
                    counter += 16;
                }
                if (sCounter == 0)
                {
                    keyStream = gearLikeShift(keyStream);
                    sCounter++;
                }
                else if (sCounter == 1)
                {
                    keyStream = horizontalShift(keyStream);
                    sCounter++;
                }
                else if (sCounter == 2)
                {
                    keyStream = verticalShiftRev(keyStream);
                    sCounter = 0;
                }
            }
            return data;
        }
        public byte[] encrypt(byte[] data, string password)
        {
            int counter = 0;
            byte[] passStream = Encoding.ASCII.GetBytes(password);
            int maxPointer = passStream.Length;
            int cPointer = 0;
            while (counter != data.Length)
            {
                if ((data.Length - counter) < 16)
                {
                    byte[] temp = new byte[data.Length - counter];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        temp[i] = data[counter + i];
                    }
                    temp = alternatePhase1(temp);
                    temp = alternatePhase2(temp);
                    for (int i = 0; i < temp.Length; i++)
                    {
                        data[counter + i] = (byte)(temp[i] ^ passStream[cPointer]);
                        cPointer++;
                        if (cPointer == maxPointer)
                        {
                            cPointer = 0;
                        }
                    }
                    counter += (data.Length - counter);
                }
                else
                {
                    byte[] temp = new byte[16];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        temp[i] = data[counter + i];
                    }
                    byte[,] dataTable = rtl.convertTo4x4RectArray(temp);
                    if (!firstRun)
                    {
                        dataTable = XORBlocks(previousBlock, dataTable);
                    }
                    dataTable = gearLikeShift(dataTable);
                    dataTable = horizontalShift(dataTable);
                    dataTable = verticalShift(dataTable);
                    dataTable = Phase2(dataTable);
                    previousBlock = dataTable;
                    if (firstRun)
                    {
                        firstRun = false;
                    }
                    temp = rtl.convert4x4RegArrayToLinearArray(dataTable);
                    for (int i = 0; i < temp.Length; i++)
                    {
                        data[counter + i] = (byte)(temp[i] ^ passStream[cPointer]);
                        cPointer++;
                        if (cPointer == maxPointer)
                        {
                            cPointer = 0;
                        }
                    }
                    counter += 16;
                }
                if (sCounter == 0)
                {
                    keyStream = gearLikeShift(keyStream);
                    sCounter++;
                }
                else if (sCounter == 1)
                {
                    keyStream = horizontalShift(keyStream);
                    sCounter++;
                }
                else if (sCounter == 2)
                {
                    keyStream = verticalShiftRev(keyStream);
                    sCounter = 0;
                }
            }
            return data;
        }
        private byte[] alternatePhase1(byte[] data)
        {
            for (int i = 0; i < data.Length - 1; i++)
            {
                byte temp = data[i];
                data[i] = data[i + 1];
                data[i + 1] = temp;
            }
            return data;
        }
        private byte[] alternatePhase1Rev(byte[] data)
        {
            for (int i = data.Length - 1; i > 0; i--)
            {
                byte temp = data[i];
                data[i] = data[i - 1];
                data[i - 1] = temp;
            }
            return data;
        }
        private byte[,] XORBlocks(byte[,] cipher, byte[,] plainText)
        {
            for (int l = 0; l < 4; l++)
            {
                for (int m = 0; m < 4; m++)
                {
                    plainText[l, m] = (byte)(plainText[l, m] ^ cipher[l, m]);
                }
            }
            return plainText;
        }
        private byte[] alternatePhase2(byte[] data)
        {
            byte[] linearKey = rtl.convert4x4RegArrayToLinearArray(keyStream);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] -= linearKey[i];
                data[i] = (byte)(data[i] ^ linearKey[i]);
            }
            return data;
        }
        private byte[] alternatePhase2Rev(byte[] data)
        {
            byte[] linearKey = rtl.convert4x4RegArrayToLinearArray(keyStream);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] ^ linearKey[i]);
                data[i] += linearKey[i];
                
            }
            return data;
        }
        private byte[,] Phase2(byte[,] data)
        {
            for (int l = 0; l < 4; l++)
            {
                for (int m = 0; m < 4; m++)
                {
                    data[l, m] -= keyStream[l, m];
                    data[l, m] = (byte)(data[l, m] ^ (keyStream[l, m]));
                }
            }
            return data;
        }
        private byte[,] Phase2Rev(byte[,] data)
        {
            for (int l = 0; l < 4; l++)
            {
                for (int m = 0; m < 4; m++)
                {
                    data[l, m] = (byte)(data[l, m] ^ (keyStream[l, m]));
                    data[l, m] += keyStream[l, m];
                }
            }
            return data;
        }
        private byte[,] gearLikeShift(byte[,] data)
        {
            byte[,] tempTable = new byte[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (i == 0)
                    {
                        if(j == 0)
                        {
                            tempTable[i + 1, j] = data[i, j];
                        }
                        else
                        {
                            tempTable[i, j - 1] = data[i, j];
                        }
                    }
                    else if (i == 1)
                    {
                        if (j  == 0 || j == 2)
                        {
                            tempTable[i + 1, j] = data[i, j];
                        }
                        else if (j == 3)
                        {
                            tempTable[i - 1, j] = data[i, j];
                        }
                        else
                        {
                            tempTable[i, j + 1] = data[i, j];
                        }
                    }
                    else if (i == 2)
                    {
                        if (j == 0)
                        {
                            tempTable[i + 1, j] = data[i, j];
                        }
                        else if (j == 3 || j == 1)
                        {
                            tempTable[i - 1, j] = data[i, j];
                        }
                        else
                        {
                            tempTable[i, j - 1] = data[i, j];
                        }
                    }
                    else
                    {
                        if (j == 3)
                        {
                            tempTable[i - 1, j] = data[i, j];
                        }
                        else
                        {
                            tempTable[i, j + 1] = data[i, j];
                        }
                    }
                }
            }
            return tempTable;
        }
        private byte[,] gearLikeShiftRev(byte[,] data)
        {
            byte[,] tempTable = new byte[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (i == 0)
                    {
                        if (j == 0)
                        {
                            tempTable[i, j] = data[(i + 1), j];
                        }
                        else
                        {
                            tempTable[i, j] = data[i, j - 1];
                        }
                    }
                    else if (i == 1)
                    {
                        if (j == 0 || j == 2)
                        {
                            tempTable[i, j] = data[i + 1, j];
                        }
                        else if (j == 3)
                        {
                            tempTable[i, j] = data[i - 1, j];
                        }
                        else
                        {
                            tempTable[i, j] = data[i, j + 1];
                        }
                    }
                    else if (i == 2)
                    {
                        if (j == 0)
                        {
                            tempTable[i, j] = data[i + 1, j];
                        }
                        else if (j == 3 || j == 1)
                        {
                            tempTable[i, j] = data[i - 1, j];
                        }
                        else
                        {
                            tempTable[i, j] = data[i, j - 1];
                        }
                    }
                    else
                    {
                        if (j == 3)
                        {
                            tempTable[i, j] = data[i - 1, j];
                        }
                        else
                        {
                            tempTable[i, j] = data[i, j + 1];
                        }
                    }
                }
            }
            return tempTable;
        }
        private byte[,] horizontalShift(byte[,] data)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    byte temp = data[i, j];
                    data[i, j] = data[i, j + 1];
                    data[i, j + 1] = temp;
                }
            }
            return data;
        }
        private byte[,] horizontalShiftRev(byte[,] data)
        {
            for (int i = 3; i >= 0; i--)
            {
                for (int j = 3; j > 0; j--)
                {
                    byte temp = data[i, j];
                    data[i, j] = data[i, j -1];
                    data[i, j - 1] = temp;
                }
            }
            return data;
        }
        private byte[,] verticalShift(byte[,] data)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    byte temp = data[j, i];
                    data[j, i] = data[j + 1, i];
                    data[j + 1, i] = temp;
                }
            }
            return data;
        }
        private byte[,] verticalShiftRev(byte[,] data)
        {
            for (int i = 3; i >= 0; i--)
            {
                for (int j = 3; j > 0; j--)
                {
                    byte temp = data[j, i];
                    data[j, i] = data[j - 1, i];
                    data[j - 1, i] = temp;
                }
            }
            return data;
        }
    }
}
