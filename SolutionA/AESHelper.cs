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

        public static string Encrypt(string plainText, Encoding encoding, byte[] key, byte[]? IV = null)
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
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt, encoding))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string Encrypt(string plainText, Encoding encoding, string keyBase64, byte[]? IV = null)
        {
            return Encrypt(plainText, encoding, Convert.FromBase64String(keyBase64), IV);
        }

        public static string Decrypt(string cipherText, Encoding encoding, byte[] key, byte[]? IV = null)
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
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt, encoding))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, Encoding encoding, string keyBase64, byte[]? IV = null)
        {
            return Decrypt(cipherText, encoding, Convert.FromBase64String(keyBase64), IV);
        }

        public static void Encrypt(string inputFile, string outputFile, string keyBase64)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Convert.FromBase64String(keyBase64);
                    aesAlg.IV = new byte[16];
                    Array.Clear(aesAlg.IV, 0, aesAlg.IV.Length);

                    using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
                    using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create))
                    using (ICryptoTransform encryptor = aesAlg.CreateEncryptor())
                    using (CryptoStream cryptoStream = new CryptoStream(fsOutput, encryptor, CryptoStreamMode.Write))
                    {
                        int bytesRead;
                        byte[] buffer = new byte[4096];
                        while ((bytesRead = fsInput.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            cryptoStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error encrypting PDF: {ex.Message}");
            }
        }

        public static void Decrypt(string inputFile, string outputFile, string keyBase64)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Convert.FromBase64String(keyBase64);
                    aesAlg.IV = new byte[16];
                    Array.Clear(aesAlg.IV, 0, aesAlg.IV.Length);

                    using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
                    using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create))
                    using (ICryptoTransform decryptor = aesAlg.CreateDecryptor())
                    using (CryptoStream cryptoStream = new CryptoStream(fsInput, decryptor, CryptoStreamMode.Read))
                    {
                        int bytesRead;
                        byte[] buffer = new byte[4096];
                        while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fsOutput.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decrypting PDF: {ex.Message}");
            }
        }
    }
}
