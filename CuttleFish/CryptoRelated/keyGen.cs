using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuttleFish.CryptoRelated
{
    class keyGen
    {
        public static byte[,] generate128bitkey()
        {
            byte[,] key = new byte[4, 4];
            Random rng = new Random();
            for (int i = 0; i < key.GetLength(0); i++)
            {
                for (int j = 0; j < key.GetLength(1); j++)
                {
                    key[i, j] = (byte)rng.Next(0, 255);
                }
            }
            return key;
        }
    }
}
