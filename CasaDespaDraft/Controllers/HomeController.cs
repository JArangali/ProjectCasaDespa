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
            return View(_dbData.infos);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult IndexEdit(int id)
        {
            //Search for recipe whose id matches the given id
            Homepage? infos = _dbData.infos.FirstOrDefault(rec => rec.Id == id);

            if (infos != null)//was a recipe found?
                return View(infos);

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> IndexEdit(Homepage infosChanges)
        {
            if (ModelState.IsValid)
            {
                Homepage? infos = _dbData.infos.FirstOrDefault(rec => rec.Id == infosChanges.Id);

                if (infos != null)
                {
                    infos.wd22hrs = infosChanges.wd22hrs;
                    infos.wdMornhrs = infosChanges.wdMornhrs;
                    infos.wdNythrs = infosChanges.wdNythrs;

                    infos.we22hrs = infosChanges.we22hrs;
                    infos.weMornhrs = infosChanges.weMornhrs;
                    infos.weNythrs = infosChanges.weNythrs;

                    infos.af1 = infosChanges.af1;
                    infos.af2 = infosChanges.af2;
                    infos.af3 = infosChanges.af3;
                    infos.af4 = infosChanges.af4;
                    infos.af5 = infosChanges.af5;

                    infos.sd1 = infosChanges.sd1;
                    infos.c1 = infosChanges.c1;
                    infos.c2 = infosChanges.c2;
                    infos.oc1 = infosChanges.oc1;

                    infos.cIN = infosChanges.cIN;
                    infos.cOUT = infosChanges.cOUT;
                    infos.payment = infosChanges.payment;

                    infos.mc1 = infosChanges.mc1;
                    infos.mc2 = infosChanges.mc2;
                    infos.clean = infosChanges.clean;
                    infos.sc1 = infosChanges.sc1;
                    infos.noise = infosChanges.noise;
                    infos.penalty = infosChanges.penalty;
                    infos.warning = infosChanges.warning;


                    _dbData.Entry(infos).State = EntityState.Modified;
                    await _dbData.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return View("Index", infosChanges);
            }
        }



        [Authorize(Roles = "Admin")]
        public IActionResult EditHome() {
            return View();
        }

        public IActionResult Gallery()
        {
            return View(_dbData.Gallery);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
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

        [HttpGet]
        public IActionResult Gallery_Delete(int id)
        {
            Gallery? gallery = _dbData.Gallery.FirstOrDefault(rec => rec.imageId == id);
            if (gallery == null)
            {
                // Handle the case where the booking is not found
                return NotFound();
            }

            _dbData.Gallery.Remove(gallery);
            _dbData.SaveChanges();

            return RedirectToAction("GalleryEdit", "Home");
        }

        [HttpGet]
        public IActionResult GalleryDELETE_Page(int id)
        {
            Gallery? gallery = _dbData.Gallery.FirstOrDefault(rec => rec.imageId == id);

            return View(gallery);
        }

        [HttpGet]
        public IActionResult GalleryVideoEdit(int id)
        {
            Gallery? gallery = _dbData.Gallery.FirstOrDefault(rec => rec.imageId == id);

            id = 1;

            return View(gallery);
        }
            
        

        [HttpPost]
        public async Task<IActionResult> GalleryVideoEdit(Gallery model)
        {
            Gallery? gallery = _dbData.Gallery.FirstOrDefault(rec => rec.imageId == model.imageId);

            model.imageId = 1;

            gallery.video = model.video;

            _dbData.Gallery.Update(gallery);
            _dbData.SaveChanges();
            return RedirectToAction("Gallery", "Home");
        }

        public static string ConvertToEmbedUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }

            // Extract the video ID from the URL
            var videoId = new Uri(url).GetComponents(UriComponents.Query, UriFormat.Unescaped).Split('=')[1];

            // Convert the video URL to an embed URL
            var video = $"https://www.youtube.com/embed/{videoId}";
            return video;
        }

        public IActionResult FAQs()
        {
            return View(_dbData.FAQs);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult FAQsEdit()
        {
            return View(_dbData.FAQs);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult FAQsADD_Page()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        public IActionResult Profile()
        {

            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                // Handle user not found
                return RedirectToAction("Index"); // Redirect to another action or handle appropriately
            }

            var createdBookings = _dbData.Bookings.Where(r => r.userId == user.Id).ToList();


            var viewModel = new ProfileViewModel
            {
                CreatedBookings = createdBookings,
            };

            return View(viewModel);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}