using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuttleFish
{
    class HexConverterClass
    {
        public HexConverterClass()
        {

        }
        public int hexToDecimal(string Data)
        {
            char[] input = Data.ToCharArray();
            int deci = 0;
            for (int i = 0;  i < input.Length; i++)
            {
                if (input[i].Equals('A') || input[i].Equals('a'))
                {
                    deci += 10 * (int)Math.Pow(16.0, input.Length - (i + 1));
                }
                else if (input[i].Equals('B') || input[i].Equals('b'))
                {
                    deci += 11 * (int)Math.Pow(16.0, input.Length - (i + 1));
                }
                else if (input[i].Equals('C') || input[i].Equals('c'))
                {
                    deci += 12 * (int)Math.Pow(16.0, input.Length - (i + 1));

                }
                else if (input[i].Equals('D') || input[i].Equals('d'))
                {
                    deci += 13 * (int)Math.Pow(16.0, input.Length - (i + 1));
                }
                else if (input[i].Equals('E') || input[i].Equals('e'))
                {
                    deci += 14 * (int)Math.Pow(16.0, input.Length - (i + 1));
                }
                else if (input[i].Equals('F') || input[i].Equals('f'))
                {
                    deci += 15 * (int)Math.Pow(16.0, input.Length - (i + 1));
                    
                }
                else
                {
                    deci += int.Parse(input[i] + "") * (int)Math.Pow(16, input.Length - (i + 1)); 
                }
            }
            return deci;
        }
        public string decimalToHex(int data)
        {
            string hex = "";
            Stack<string> stk = new Stack<string>();
            int modulo = 0;
            while (data > 16)
            {
                modulo = data % 16;
                if (modulo > 9)
                {
                    if (modulo == 10)
                    {
                        stk.Push("A");
                    }
                    else if (modulo == 11)
                    {
                        stk.Push("B");
                    }
                    else if (modulo == 12)
                    {
                        stk.Push("C");
                    }
                    else if (modulo == 13)
                    {
                        stk.Push("D");
                    }
                    else if (modulo == 14)
                    {
                        stk.Push("E");
                    }
                    else if (modulo == 15)
                    {
                        stk.Push("F");
                    }

                }
                else
                {
                    stk.Push(modulo + "");
                }
                data = data / 16;
            }
            modulo = data % 16;
            if (modulo > 9)
            {
                if (modulo == 10)
                {
                    stk.Push("A");
                }
                else if (modulo == 11)
                {
                    stk.Push("B");
                }
                else if (modulo == 12)
                {
                    stk.Push("C");
                }
                else if (modulo == 13)
                {
                    stk.Push("D");
                }
                else if (modulo == 14)
                {
                    stk.Push("E");
                }
                else if (modulo == 15)
                {
                    stk.Push("F");
                }
            }
            else if (data == 16)
            {
                stk.Push("10");
            }
            else
            {
                stk.Push(modulo + "");
            }
            while (stk.Count != 0)
            {
                hex += stk.Pop();
            }
            return hex;
        }
    }
}
