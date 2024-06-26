﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SolutionA
{
    public class Program
    {
        public static void Main()
        {
            RunProgram();
        }

        private static void RunProgram()
        {
            bool keepRunning = true;

            while (keepRunning)
            {
                Console.WriteLine("\nChoose an option:");
                Console.WriteLine("1. Encrypt and Decrypt a file using AES");
                Console.WriteLine("2. Generate RSA keys and encrypt/decrypt data");
                Console.WriteLine("3. Calculate SHA-1 and SHA-256 hash of a string");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your choice: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AES_GenerateKey();
                        break;
                    case "2":
                        GenerateAndTestRSAKeys();
                        break;
                    case "3":
                        ComputeHashSHA();
                        break;
                    case "4":
                        Console.WriteLine("Exiting program...");
                        keepRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }

        private static void AES_GenerateKey()
        {
            string keyStr = AESHelper.GenerateSecretKeyBase64(AESHelper.Type.AES128);

            Console.WriteLine("Secret key (Base64): " + keyStr);

            string origin = "C:\\Users\\ntd12\\OneDrive\\Máy tính\\TestAES\\origin.pdf";
            string encrypted = "C:\\Users\\ntd12\\OneDrive\\Máy tính\\TestAES\\encrypted.pdf";
            string decrypted = "C:\\Users\\ntd12\\OneDrive\\Máy tính\\TestAES\\decrypted.pdf";

            AESHelper.Encrypt(origin, encrypted, keyStr);
            AESHelper.Decrypt(encrypted, decrypted, keyStr);
        }

        private static void GenerateAndTestRSAKeys()
        {
            var (publicKey, privateKey) = RSAHelper.GenerateKeys(RSAHelper.Type.RSA1024);
            Console.WriteLine("Public Key:");
            Console.WriteLine(publicKey);
            Console.WriteLine();

            Console.WriteLine("Private Key:");
            Console.WriteLine(privateKey);
            Console.WriteLine();

            string dataToEncrypt = "Hello, RSA!";
            string encryptedData = RSAHelper.EncryptData(dataToEncrypt, publicKey);

            Console.WriteLine("Encrypted Data:");
            Console.WriteLine(encryptedData);
            Console.WriteLine();

            string decryptedData = RSAHelper.DecryptData(encryptedData, privateKey);

            Console.WriteLine("Decrypted Data:");
            Console.WriteLine(decryptedData);
        }

        private static void ComputeHashSHA()
        {
            Console.Write("Enter string: ");
            var str = Console.ReadLine();

            if (str == null)
            {
                Console.WriteLine("No input provided.");
                return;
            }

            Console.WriteLine("SHA-1: " + SHAHelper.ComputeHashSHA1(str));
            Console.WriteLine("SHA-256: " + SHAHelper.ComputeHashSHA256(str));
        }
    }
}
