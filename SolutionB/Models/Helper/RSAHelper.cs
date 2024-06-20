using System;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.OpenSsl;
using System.IO;
using Org.BouncyCastle.Crypto;

namespace SolutionB.Models.Helper
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
                return (ExportPublicKeyToX509PemFormat(publicKey), ExportPrivateKeyToPkcs8PemFormat(privateKey));
            }
        }

        public static string ExportPublicKeyToX509PemFormat(string parameters)
        {
            using (var rsa = RSA.Create())
            {
                rsa.FromXmlString(parameters);
                if (rsa == null)
                {
                    throw new ArgumentNullException(nameof(rsa), "RSA object cannot be null.");
                }

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


        // Encrypts data using the RSA algorithm and a public key in PEM format
        public static string EncryptData(string data, string publicKeyPem)
        {
            using (var rsa = RSA.Create())
            {
                // Convert PEM format to RSAParameters
                rsa.ImportParameters(ImportPublicKey(publicKeyPem));

                var dataToEncrypt = Encoding.UTF8.GetBytes(data);
                var encryptedData = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.Pkcs1);
                return Convert.ToBase64String(encryptedData);
            }
        }

        // Decrypts data using the RSA algorithm and a private key in PEM format
        public static string DecryptData(string encryptedData, string privateKeyPem)
        {
            using (var rsa = RSA.Create())
            {
                // Convert PEM format to RSAParameters
                rsa.ImportParameters(ImportPrivateKey(privateKeyPem));

                var dataToDecrypt = Convert.FromBase64String(encryptedData);
                var decryptedData = rsa.Decrypt(dataToDecrypt, RSAEncryptionPadding.Pkcs1);
                return Encoding.UTF8.GetString(decryptedData);
            }
        }

        private static RSAParameters ImportPublicKey(string pem)
        {
            using (var reader = new StringReader(pem))
            {
                var pemReader = new PemReader(reader);
                var publicKeyParameters = (RsaKeyParameters)pemReader.ReadObject();
                return DotNetUtilities.ToRSAParameters(publicKeyParameters);
            }
        }

        private static RSAParameters ImportPrivateKey(string pem)
        {
            using (var reader = new StringReader(pem))
            {
                var pemReader = new PemReader(reader);
                // Directly cast to RsaPrivateCrtKeyParameters instead of AsymmetricCipherKeyPair
                var privateKeyParameters = (RsaPrivateCrtKeyParameters)pemReader.ReadObject();
                return DotNetUtilities.ToRSAParameters(privateKeyParameters);
            }
        }
    }
}