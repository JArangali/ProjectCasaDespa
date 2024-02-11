using CasaDespaDraft.Data;
using CasaDespaDraft.Models;
using CasaDespaDraft.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace CasaDespaDraft.Controllers
{
    public class HomeController : Controller
    {

        private readonly AppDbContext _dbData;
        private readonly UserManager<User> _userManager;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, AppDbContext dbData, UserManager<User> userManager)
        {
            _logger = logger;
            _dbData = dbData;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult IndexEdit()
        {
            return View();
        }

        public IActionResult EditHome() { 
            return View();
        }

        public IActionResult Gallery()
        {
            return View(_dbData.Gallery);
        }

        [HttpGet]
        public IActionResult GalleryEdit()
        {
            var galleryViewModel = new GalleryViewModel
            {
                Galleries = _dbData.Gallery
            };
            return View(galleryViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GalleryEdit(Gallery newImage, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return View();

            if (image != null && image.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);
                newImage.image = memoryStream.ToArray();
            }

            /*newImage.imageName = image.FileName;*/

            // Apply the selected filter to the uploaded image
            switch (newImage.filter)
            {
                case Filter.Pool:
                    newImage.filterChoice = "filter-web";
                    break;
                case Filter.Amenities:
                    newImage.filterChoice = "filter-card";
                    break;
                case Filter.Entertainment:
                    newImage.filterChoice = "filter-app";
                    break;
            }

            // Add the new recipe to the context
            _dbData.Gallery.Add(newImage);

            // Save changes to the database
            await _dbData.SaveChangesAsync();
            return RedirectToAction("GalleryEdit");
        }

        [HttpDelete]
        public IActionResult Gallery_Delete(int id)
        {
            Gallery? gallery = _dbData.Gallery.Find(id);

            if (gallery != null)
            {
                _dbData.Gallery.Remove(gallery);
                _dbData.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        public IActionResult FAQs()
        {
            return View(_dbData.FAQs);
        }

        public IActionResult FAQsEdit()
        {
            return View(_dbData.FAQs);
        }

        [HttpGet]
        public IActionResult FAQsADD_Page()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FAQsADD_Page(FAQs newFAQ)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            // Add the new FAQ to the context
            _dbData.FAQs.Add(newFAQ);

            // Save changes to the database
            await _dbData.SaveChangesAsync();
            return RedirectToAction("FAQs");
        }

        [HttpGet]
        public IActionResult FAQsEDIT_Page(int id)
        {
            //Search for recipe whose id matches the given id
            FAQs? faqs = _dbData.FAQs.FirstOrDefault(rec => rec.faqsId == id);

            if (faqs != null)//was a recipe found?
                return View(faqs);

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> FAQsEDIT_Page(FAQs faqsChanges)
        {
            if (ModelState.IsValid)
            {
                FAQs? faqs = _dbData.FAQs.FirstOrDefault(rec => rec.faqsId == faqsChanges.faqsId);

                if (faqs != null)
                {
                    faqs.FAQuestion = faqsChanges.FAQuestion;
                    faqs.FAQAnswer = faqsChanges.FAQAnswer;
                   

                    _dbData.Entry(faqs).State = EntityState.Modified;
                    await _dbData.SaveChangesAsync();

                    return RedirectToAction("FAQsEdit");
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return View("FAQsEDIT_Page", faqsChanges);
            }
        }

        [HttpDelete]
        public IActionResult FAQsEDIT_Delete(int id)
        {
            FAQs? faqs = _dbData.FAQs.Find(id);

            if (faqs != null)
            {
                _dbData.FAQs.Remove(faqs);
                _dbData.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        public IActionResult Dashboard_BR()
        {
            return View();
        }

        public IActionResult Dashboard_BT()
        {
            return View();
        }

        public IActionResult Dashboard_AR()
        {
            return View();
        }

        public IActionResult Dashboard_AB()
        {
            return View();
        }

        public IActionResult Profile()
        {

            return View();
        }

        public IActionResult ProfileP()
        {
            return View();
        }

        public IActionResult ProfileA()
        {
            return View();
        }

        public IActionResult Receipt()
        {
            return View();
        }

        public IActionResult Booking()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}