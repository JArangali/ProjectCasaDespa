using CasaDespaDraft.Data;
using CasaDespaDraft.Models;
using CasaDespaDraft.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Xml.Linq;

namespace CasaDespaDraft.Controllers
{
    public class BookingController : Controller
    {
        private readonly ILogger<BookingController> _logger;
        private readonly AppDbContext _dbData;
        private readonly UserManager<User> _userManager;

        public BookingController(ILogger<BookingController> logger, AppDbContext dbData, UserManager<User> userManager)
        {
            _logger = logger;
            _dbData = dbData;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Booking()
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                // Handle user not found
                return RedirectToAction("Login", "Account"); // Redirect to another action or handle appropriately
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Booking(Booking newBooking)
        {
            if (!ModelState.IsValid)
                return View();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var toAdd = newBooking;
            toAdd.BStatus = "Pending";

            newBooking.Status = ProfileStatus.Requests;

            // Set the UserId of the new recipe
            newBooking.userId = userId;

            // Add the new recipe to the context
            _dbData.Bookings.Add(toAdd);

            // Save changes to the database
            await _dbData.SaveChangesAsync();
            return RedirectToAction("Profile", "Home");
        }

        public IActionResult Receipt()
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