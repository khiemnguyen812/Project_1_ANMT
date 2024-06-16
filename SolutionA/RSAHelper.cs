using System;
using System.Security.Cryptography;
using System.Text;

namespace SolutionA
{
    public static class RSAHelper
    {
        // Generates a new public and private key pair using the RSA algorithm
        public static (string publicKey, string privateKey) GenerateKeys()
        {
            using (var rsa = new RSACryptoServiceProvider(1024))
            {

                var publicKey = rsa.ToXmlString(false); // Export public key
                var privateKey = rsa.ToXmlString(true); // Export private key
                rsa.PersistKeyInCsp = false; // Ensure keys are not persisted in the key container
                return (publicKey, privateKey);
            }
        }

        public static string ExportPublicKeyToX509PemFormat(string parameters)
        {
            using (var rsa = RSA.Create())
            {
                // Import the RSA parameters from XML
                rsa.FromXmlString(parameters);
                // Ensure the RSA object is not null before passing it
                if (rsa == null)
                {
                    throw new ArgumentNullException(nameof(rsa), "RSA object cannot be null.");
                }

                // Export the public key in X.509 format
                var publicKeyBytes = rsa.ExportSubjectPublicKeyInfo();
                var builder = new StringBuilder();
                builder.AppendLine("-----BEGIN PUBLIC KEY-----");
                builder.AppendLine(Convert.ToBase64String(publicKeyBytes, Base64FormattingOptions.InsertLineBreaks));
                builder.AppendLine("-----END PUBLIC KEY-----");
                return builder.ToString();
            }
        }

        public static string ExportPrivateKeyToPkcs8PemFormat(string parameters)
        {
            using (var rsa = RSA.Create())
            {
                // Import the RSA parameters from XML
                rsa.FromXmlString(parameters);
                // Ensure the RSA object is not null before passing it
                if (rsa == null)
                {
                    throw new ArgumentNullException(nameof(rsa), "RSA object cannot be null.");
                }

                // Export the private key in PKCS#8 format
                var privateKeyBytes = rsa.ExportPkcs8PrivateKey();
                var builder = new StringBuilder();
                builder.AppendLine("-----BEGIN PRIVATE KEY-----");
                builder.AppendLine(Convert.ToBase64String(privateKeyBytes, Base64FormattingOptions.InsertLineBreaks));
                builder.AppendLine("-----END PRIVATE KEY-----");
                return builder.ToString();
            }
        }


        // Encrypts data using the RSA algorithm and a public key
        public static string EncryptData(string data, string publicKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                var dataToEncrypt = Encoding.UTF8.GetBytes(data);
                var encryptedData = rsa.Encrypt(dataToEncrypt, false);
                return Convert.ToBase64String(encryptedData);
            }
        }

        // Decrypts data using the RSA algorithm and a private key
        public static string DecryptData(string encryptedData, string privateKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                var dataToDecrypt = Convert.FromBase64String(encryptedData);
                var decryptedData = rsa.Decrypt(dataToDecrypt, false);
                return Encoding.UTF8.GetString(decryptedData);
            }
        }
    }
}