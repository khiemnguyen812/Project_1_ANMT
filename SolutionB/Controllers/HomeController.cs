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
using SolutionB.Models.Helper;

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
        public ActionResult UploadFile(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    string fileContent_P;

                    // Read the file content directly from the file stream
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    {
                        fileContent_P = reader.ReadToEnd();
                    }

                    string AESKey_Ks = AESHelper.GenerateSecretKeyBase64(AESHelper.Type.AES128);

                    var encryptedContent_C = AESHelper.Encrypt(fileContent_P, AESKey_Ks);

                    var (Kpublic, Kprivate) = RSAHelper.GenerateKeys();

                    var encryptedAESKey = RSAHelper.EncryptData(AESKey_Ks, Kpublic);

                    var Kx_SHA1 = SHAHelper.ComputeHashSHA1(Kprivate);

                    return Json(new { success = true, message = "File uploaded successfully", fileContent = fileContent_P, AESKey_Ks = AESKey_Ks, encryptedContent_C= encryptedContent_C, Kpublic=Kpublic,Kprivate=Kprivate, encryptedAESKey = encryptedAESKey , Kx_SHA1 = Kx_SHA1 });
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
    }
}
