using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuttleFish
{
    class UOMConverter
    {
        public UOMConverter()
        {

        }
        public double byteToKilo(long bytes)
        {
            double res = (double)bytes / (double)1024;
            return res;
        }
        public double byteToMega(long bytes)
        {
            double res = (double)bytes / (double)(1024 * 1024);
            return res;
        }
        public double byteToGiga(long bytes)
        {
            double res = (double)bytes / (double)(1024 * 1024 * 1024);
            return res;
        }
    }
}
