using Microsoft.AspNetCore.Mvc;

namespace UploadingFiles.Controllers
{
    public class UploadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SingleFileUpload()
        {
            return View("SingleFile");
        }

        [HttpPost]
        public async Task<IActionResult> SingleFileUpload(IFormFile SingleFile)
        {
            if (SingleFile == null || SingleFile.Length == 0)
            {
                return View("SingleFile");
            }
            if (Path.GetExtension(SingleFile.FileName).ToLower() != ".pdf")
            {
                ModelState.AddModelError("File", "Only PDF files are allowed.");
            }

            else if (SingleFile.Length > 5000000)
            {
                ModelState.AddModelError("", "The file is too large.");
            }

            if (ModelState.IsValid)
            {
                if (SingleFile != null && SingleFile.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UploadedFiles", SingleFile.FileName);

                    //Using Buffering
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        // The file is saved in a buffer before being processed
                        await SingleFile.CopyToAsync(stream);
                    }
                    TempData["UploadSuccess"] = "File uploaded successfully.";
                    return RedirectToAction("SingleFileUpload");
                }
            }

            return View("SingleFile");
        }

        [HttpGet]
        public async Task <IActionResult> MultipleFileUpload()
        {
            return View("MultipleFile");
        }

        [HttpPost]
        public async Task<IActionResult> MultipleFileUpload(IFormFile[] Files)
        {
            if (Files == null || Files.Length == 0)
            {
                ModelState.AddModelError("", "Please select at least one file.");
                return View("Index");
            }

            foreach (var file in Files)
            {
                // Check for file extension (PDF in this case)
                if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
                {
                    ModelState.AddModelError("", "Only PDF files are allowed.");
                    return View("Index");
                }

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UploadedFiles", file.FileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
            }

            // Set success message
            TempData["UploadSuccess"] = "Files uploaded successfully!";
            return RedirectToAction("Index"); // Redirect back to the index or another page
        }


    }
}