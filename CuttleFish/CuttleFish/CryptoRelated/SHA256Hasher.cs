using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CuttleFish.CryptoRelated
{
    class SHA256Hasher
    {
        SHA256Managed hasher = null;
        string Salt = "CUTTLE";
        public SHA256Hasher()
        {
            hasher = new SHA256Managed();
        }
        public byte[] getHash(string password)
        {
            password += Salt;
            byte[] input = Encoding.ASCII.GetBytes(password);
            return hasher.ComputeHash(input);
        }
        public bool compareHashes(byte[] hash1, byte[] hash2)
        {
            return hash1.SequenceEqual(hash2);
        }
    }
}
