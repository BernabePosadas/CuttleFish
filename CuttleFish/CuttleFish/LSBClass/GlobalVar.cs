using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuttleFish
{
    class GlobalVar
    {
        public GlobalVar()
        {
            datapos = 0;
            embeded = 0;
            dataIndexPos = 0;
            partial = false;
            flag = 0;
            remainderBytes = null;
            extracted = 0;
        }
        public int datapos { set; get; }
        public long embeded { set; get; }
        public int dataIndexPos { set; get; }
        public bool partial { set; get; }
        public int flag { set; get; }
        public byte[] remainderBytes { set; get; }
        public long extracted { set; get; }

    }
}
