using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace SolutionA
{
    class AESHelper
    {
        public enum Type
        {
            AES128, AES192, AES256
        }

        public static byte[] GenerateSecretKey(Type type)
        {
            using (Aes aes = Aes.Create())
            {
                switch (type)
                {
                    case Type.AES128: aes.KeySize = 128; break;
                    case Type.AES192: aes.KeySize = 192; break;
                    case Type.AES256: aes.KeySize = 256; break;
                }
                aes.GenerateKey();
                return aes.Key;
            }
        }

        public static string GenerateSecretKeyBase64(Type type)
        {
            return Convert.ToBase64String(GenerateSecretKey(type));
        }

        public static bool EncryptFile(string inputFile, string outputFile, byte[] key, byte[]? IV = null)
        {
            if (IV == null)
            {
                IV = new byte[16];
                Array.Clear(IV, 0, IV.Length);
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = IV; // Use a fixed or pre-defined IV

                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(fileStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (FileStream inputFileStream = new FileStream(inputFile, FileMode.Open))
                        {
                            inputFileStream.CopyTo(cryptoStream);
                            return true;
                        }
                    }
                }
            }
        }

        public static bool EncryptFile(string inputFile, string outputFile, string keyBase64, byte[]? IV = null)
        {
            return EncryptFile(inputFile, outputFile, Convert.FromBase64String(keyBase64), IV);
        }

        public static bool DecryptFile(string inputFile, string outputFile, byte[] key, byte[]? IV = null)
        {
            if (IV == null)
            {
                IV = new byte[16];
                Array.Clear(IV, 0, IV.Length);
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = IV; // Use the same fixed or pre-defined IV

                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(fileStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        using (FileStream inputFileStream = new FileStream(inputFile, FileMode.Open))
                        {
                            inputFileStream.CopyTo(cryptoStream);
                            return true;
                        }
                    }
                }
            }
        }

        public static bool DecryptFile(string inputFile, string outputFile, string keyBase64, byte[]? IV = null)
        {
            return DecryptFile(inputFile, outputFile, Convert.FromBase64String(keyBase64), IV);
        }
    }
}
