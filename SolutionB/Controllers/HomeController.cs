using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index1()
        {
            return View();
        }



        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var uploadPath = Server.MapPath("~/App_Data/uploads");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var path = Path.Combine(uploadPath, fileName);
                    file.SaveAs(path);
                    string fileContent;

                    using (var reader = new StreamReader(path))
                    {
                        fileContent = reader.ReadToEnd();
                    }

/*
                    var aesKeys = AESHelper.GenerateKeyAndIV();
                    byte[] key = aesKeys.Key;
                    byte[] iv = aesKeys.IV;

                    string ksBase64 = Convert.ToBase64String(key);

                    var rsaKeys = RSAHelper.GenerateKeys();
                    string publicKey = rsaKeys.publicKey;
                    string privateKey = rsaKeys.privateKey;

                    string encryptedKs = RSAHelper.EncryptData(ksBase64, publicKey);

                    string publicKeyX509 = RSAHelper.ExportPublicKeyToX509PemFormat(publicKey);
                    string privateKeyPkcs8 = RSAHelper.ExportPrivateKeyToPkcs8PemFormat(privateKey);

                    string privateKeyHash = HashHelper.ComputeSha1Hash(privateKey);


                    return Json(new
                    {
                        success = true,
                        message = "File uploaded successfully",
                        ks = ksBase64,
                        encryptedKs = encryptedKs,
                        publicKey = publicKey,
                        privateKey = privateKey,
                        publicKeyX509 = publicKeyX509,
                        privateKeyPkcs8 = privateKeyPkcs8,
                        privateKeyHash = privateKeyHash
                    }, JsonRequestBehavior.AllowGet);
*/
                }
                return Json(new { success = false, message = "No file selected" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occurred. Error details: " + ex.Message });
            }
        }

        public ActionResult Index2()
        {
            return View();
        }
    }
}