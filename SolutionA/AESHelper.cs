using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace SolutionA
{
    public static class AESHelper
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

        public static string Encrypt(string plainText, byte[] key, byte[]? IV = null)
        {
            if (IV == null)
            {
                IV = new byte[16];
                Array.Clear(IV, 0, IV.Length);
            }

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt, Encoding.UTF8))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string Encrypt(string plainText, string keyBase64, byte[]? IV = null)
        {
            return Encrypt(plainText, Convert.FromBase64String(keyBase64), IV);
        }

        public static string Decrypt(string cipherText, byte[] key, byte[]? IV = null)
        {
            if (IV == null)
            {
                IV = new byte[16];
                Array.Clear(IV, 0, IV.Length);
            }

            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(buffer))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt, Encoding.UTF8))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string keyBase64, byte[]? IV = null)
        {
            return Decrypt(cipherText, Convert.FromBase64String(keyBase64), IV);
        }
    }
}
