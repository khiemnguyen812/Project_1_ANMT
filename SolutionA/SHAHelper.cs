using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SolutionA
{
    public static class SHAHelper
    {
        public static string ComputeHashSHA1(string str) //Returns hex string of hash value of string
        {
            byte[] hash = SHA1.HashData(Encoding.UTF8.GetBytes(str));
            return Convert.ToHexString(hash);
        }

        public static string ComputeHashSHA256(string str) //Returns hex string of hash value of string
        {
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(str));
            return Convert.ToHexString(hash);
        }
    }
}
