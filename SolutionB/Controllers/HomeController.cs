using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Http; 
using SolutionB.Models;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography.X509Certificates;
using SolutionA;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json.Nodes;
using System.Text;
using Org.BouncyCastle.Crypto.Paddings;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SolutionB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment; 
        }

        public IActionResult Index1()
        {
            return View();
        }

		[HttpPost]
		public ActionResult EncryptFile(IFormFile file, string? AESSize, string? RSASize)
		{
			try
			{
				if (file != null && file.Length > 0)
				{
					AESSize = AESSize ?? Request.Form["AESSize"];
					RSASize = RSASize ?? Request.Form["RSASize"];

					AESSize = string.IsNullOrEmpty(AESSize) ? "AES128" : AESSize;
					RSASize = string.IsNullOrEmpty(RSASize) ? "RSA2048" : RSASize;

					AESHelper.Type aesType = (AESHelper.Type)Enum.Parse(typeof(AESHelper.Type), AESSize);
					RSAHelper.Type rsaType = (RSAHelper.Type)Enum.Parse(typeof(RSAHelper.Type), RSASize);

					// Get file extension
					string fileExtension = Path.GetExtension(file.FileName);

					// Fixed file names with extension
					string originalFileName = $"origin{fileExtension}";
					string encryptedFileName = $"encrypted{fileExtension}";
					string uploadsFolder = Path.Combine(_environment.WebRootPath, "encrypt");
					string originalFilePath = Path.Combine(uploadsFolder, originalFileName);
					string encryptedFilePath = Path.Combine(uploadsFolder, encryptedFileName);

					// Delete existing files if they exist
					if (System.IO.File.Exists(originalFilePath))
					{
						System.IO.File.Delete(originalFilePath);
					}
					if (System.IO.File.Exists(encryptedFilePath))
					{
						System.IO.File.Delete(encryptedFilePath);
					}

					using (var fileStream = new FileStream(originalFilePath, FileMode.Create))
					{
						file.CopyTo(fileStream);
					}

					string AESKey_Ks = AESHelper.GenerateSecretKeyBase64(aesType);
					var (Kpublic, Kprivate) = RSAHelper.GenerateKeys(rsaType);
					var encryptedAESKeybyRSA = RSAHelper.EncryptData(AESKey_Ks, Kpublic);
					var HKprivate = SHAHelper.ComputeHashSHA1(Kprivate);

					AESHelper.Encrypt(originalFilePath, encryptedFilePath, AESKey_Ks);

					string encryptedContent_C;
					using (var reader = new StreamReader(encryptedFilePath))
					{
						encryptedContent_C = reader.ReadToEnd();
					}

					return Json(new { success = true, message = "File uploaded and encrypted successfully", originalFileName, encryptedFileName, AESKey_Ks, encryptedContent_C, Kpublic, Kprivate, encryptedAESKeybyRSA, HKprivate , fileExtension });
				}
				return Json(new { success = false, message = "No file selected" });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = "Error occurred. Error details: " + ex.Message });
			}
		}

		public static Encoding GetFileEncoding(Stream stream)
        {
            // Read the BOM
            using (var reader = new StreamReader(stream))
            {
                reader.Peek(); // you need this!
                return reader.CurrentEncoding;
            }
        }

        public IActionResult Index2()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DecryptFile(IFormFile cipher, string KPrivate, string Kx, string HKprivate)
        {
            try
            {
                if (cipher == null || cipher.Length == 0)  return Json(new { success = false, message = "No file selected" });

                //Check if Hash of kPrivate matches HKprivate
                if (SHAHelper.ComputeHashSHA1(KPrivate) != HKprivate) return Json(new { success = false, message = "Hash of KPrivate doesn\'t match HKprivate" });
                string fileContent;
                string fileContent_P;
                Encoding fileEncoding = GetFileEncoding(cipher.OpenReadStream());

                using (var reader = new StreamReader(cipher.OpenReadStream()))
                {
                    fileContent = reader.ReadToEnd();
                }

                //Decrypt Kx -> Ks
                string Ks = RSAHelper.DecryptData(Kx, KPrivate);
                string origin = AESHelper.Decrypt(fileContent, fileEncoding, Ks);

                string fileType = System.IO.Path.GetExtension(cipher.FileName);

                return Json(new { success = true, message = "Success", fileType = fileType, origin = origin, Ks = Ks});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occurred. Error details: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult InsertKxHKprivateFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0) return Json(new { success = false, message = "No file selected" });

                if (System.IO.Path.GetExtension(file.FileName) != ".json") return Json(new { success = false, message = "Json file is required" }); ;

                string fileContent;

                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    fileContent = reader.ReadToEnd();
                }

                JsonObject result = JsonValue.Parse(fileContent) as JsonObject;

                return Json(new { success = true, message = "Success", Kx = (string)result["Kx"], HKprivate = (string)result["HKprivate"] });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occurred. Error details: " + ex.Message });
            }
        }
    }
}
