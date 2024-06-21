using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace SolutionA
{
    public class AESHelper
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

        public static void Encrypt(string inputFile, string outputFile, byte[] key, byte[]? IV = null)
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
                        }
                    }
                }
            }

            // Convert to base64
            byte[] encryptedContent = File.ReadAllBytes(outputFile);
            string base64EncryptedContent = Convert.ToBase64String(encryptedContent);
            File.WriteAllText(outputFile, base64EncryptedContent);
        }

        public static void Encrypt(string inputFile, string outputFile, string keyBase64, byte[]? IV = null)
        {
            Encrypt(inputFile, outputFile, Convert.FromBase64String(keyBase64), IV);
        }

        public static string Encrypt(string plainText, byte[] key, byte[]? IV = null)
        {
            if (IV == null)
            {
                IV = new byte[16];
                Array.Clear(IV, 0, IV.Length);
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = IV;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        public static string Encrypt(string plainText, string keyBase64, byte[]? IV = null)
        {
            return Encrypt(plainText, Convert.FromBase64String(keyBase64), IV);
        }

        public static void Decrypt(string inputFile, string outputFile, byte[] key, byte[]? IV = null)
        {
            if (IV == null)
            {
                IV = new byte[16];
                Array.Clear(IV, 0, IV.Length);
            }

            // Convert from base64 to byte
            string base64EncryptedContent = File.ReadAllText(inputFile);
            byte[] buffer = Convert.FromBase64String(base64EncryptedContent);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = IV;

                using (FileStream fileStream = new FileStream(outputFile, FileMode.Create))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(fileStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        using (MemoryStream memoryStream = new MemoryStream(buffer))
                        {
                            memoryStream.CopyTo(cryptoStream);
                        }
                    }
                }
            }
        }

        public static void Decrypt(string inputFile, string outputFile, string keyBase64, byte[]? IV = null)
        {
            Decrypt(inputFile, outputFile, Convert.FromBase64String(keyBase64), IV);
        }

        public static string Decrypt(string cipherText, byte[] key, byte[]? IV = null)
        {
            if (IV == null)
            {
                IV = new byte[16];
                Array.Clear(IV, 0, IV.Length);
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = IV;

                byte[] buffer = Convert.FromBase64String(cipherText);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
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
