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
                    // Check the MIME type to determine the file format
                    string mimeType = System.IO.Path.GetExtension(file.FileName);

                    string fileContent_P;

                    using (var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
                    {
                        fileContent_P = reader.ReadToEnd();
                    }

                    string AESKey_Ks = AESHelper.GenerateSecretKeyBase64(aesType);

                    var encryptedContent_C = AESHelper.Encrypt(fileContent_P, AESKey_Ks);

                    var (Kpublic, Kprivate) = RSAHelper.GenerateKeys(rsaType);

                    var encryptedAESKeybyRSA = RSAHelper.EncryptData(AESKey_Ks, Kpublic);

                    var HKprivate = SHAHelper.ComputeHashSHA1(Kprivate);

                    return Json(new { success = true, message = "File uploaded successfully", fileContent = fileContent_P, AESKey_Ks = AESKey_Ks, encryptedContent_C = encryptedContent_C, Kpublic = Kpublic, Kprivate = Kprivate, encryptedAESKeybyRSA = encryptedAESKeybyRSA, HKprivate = HKprivate, typeFile = mimeType , AESSize = AESSize , RSASize = RSASize });
            }
                return Json(new { success = false, message = "No file selected" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occurred. Error details: " + ex.Message });
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

                using (var reader = new StreamReader(cipher.OpenReadStream()))
                {
                    fileContent = reader.ReadToEnd();
                }

                //Decrypt Kx -> Ks
                string Ks = RSAHelper.DecryptData(Kx, KPrivate);
                string origin = AESHelper.Decrypt(fileContent, Ks);

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
