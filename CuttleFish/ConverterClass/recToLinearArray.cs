using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuttleFish.ConverterClass
{
    class recToLinearArray
    {
        public recToLinearArray()
        {

        }
        public byte[,] convertTo4x4RectArray(byte[] data)
        {
            byte[,] Table = new byte[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Table[i, j] = data[(i * 4) + j];
                }
            }
            return Table;
        }
        public byte[] convert4x4RegArrayToLinearArray(byte[,] data)
        {
            byte[] linear = new byte[16];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    linear[(i * 4) + j] = data[i, j];
                }
            }
            return linear;
        }
    }
}
