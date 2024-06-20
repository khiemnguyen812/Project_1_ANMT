using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Http; 
using SolutionB.Models;
using Microsoft.Extensions.Hosting;

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
        [HttpPost]
        public ActionResult UploadFile(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var uploadPath = Path.Combine(_environment.WebRootPath, "uploads");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var path = Path.Combine(uploadPath, fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    string fileContent;

                    using (var reader = new StreamReader(path))
                    {
                        fileContent = reader.ReadToEnd();
                    }

                    return Json(new { success = true, message = "File uploaded successfully", fileContent = fileContent });
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
