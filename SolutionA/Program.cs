using System;
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
                        //cEncryptDecryptFile();
                        break;
                    case "2":
                        GenerateAndTestRSAKeys();
                        break;
                    case "3":
                        //cCalculateHashes();
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
        /*
                private static void EncryptDecryptFile()
                {
                    Console.Write("Enter the path to the file: ");
                    var filePath = Console.ReadLine();

                    if (string.IsNullOrEmpty(filePath))
                    {
                        Console.WriteLine("File path is not provided or is empty.");
                        return;
                    }

                    var (key, iv) = AESHelper.GenerateKeyAndIV();

                    Console.WriteLine("\nKey:" + Convert.ToBase64String(key));
                    Console.WriteLine("IV:" + Convert.ToBase64String(iv));
                    var encryptedFilePath = filePath + ".enc";
                    var decryptedFilePath = filePath + ".dec";

                    Console.WriteLine("\nOriginal File Content:");
                    try
                    {
                        Console.WriteLine(File.ReadAllText(filePath));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading file: {ex.Message}");
                        return;
                    }

                    AESHelper.EncryptFile(filePath, encryptedFilePath, key, iv);
                    Console.WriteLine("\nEncrypted File Content:");
                    Console.WriteLine(File.ReadAllText(encryptedFilePath));

                    AESHelper.DecryptFile(encryptedFilePath, decryptedFilePath, key, iv);
                    Console.WriteLine("\nDecrypted File Content:");
                    Console.WriteLine(File.ReadAllText(decryptedFilePath));
                }
        */
        private static void GenerateAndTestRSAKeys()
        {
            var (publicKey, privateKey) = RSAHelper.GenerateKeys();
            Console.WriteLine("Public Key:");
            Console.WriteLine(RSAHelper.ExportPublicKeyToX509PemFormat(publicKey));
            Console.WriteLine();

            Console.WriteLine("Private Key:");
            Console.WriteLine(RSAHelper.ExportPrivateKeyToPkcs8PemFormat(privateKey));
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
        /*
                private static void CalculateHashes()
                {
                    Console.Write("Enter the string to calculate hash: ");
                    string? input = Console.ReadLine();
                    if (input == null)
                    {
                        Console.WriteLine("No input provided.");
                        return;
                    }
                    Console.WriteLine("SHA-1 Hash:");
                    Console.WriteLine(HashHelper.ComputeSha1Hash(input));
                    Console.WriteLine("SHA-256 Hash:");
                    Console.WriteLine(HashHelper.ComputeSha256Hash(input));
                }
        */
    }
}
