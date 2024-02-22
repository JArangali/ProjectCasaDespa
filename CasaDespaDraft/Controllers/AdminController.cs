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
    public class AdminController : Controller
    {

        private readonly AppDbContext _dbData;
        private readonly UserManager<User> _userManager;

        private readonly ILogger<HomeController> _logger;

        public AdminController(ILogger<HomeController> logger, AppDbContext dbData, UserManager<User> userManager)
        {
            _logger = logger;
            _dbData = dbData;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {
            return View(_dbData.Bookings);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeStatus(int bookingId, BookingStatus Status)
        {
            var booking = _dbData.Bookings.FirstOrDefault(r => r.bookingId == bookingId);
            if (booking != null)
            {
                booking.Status = (ProfileStatus)Status;
                await _dbData.SaveChangesAsync();
            }

            return RedirectToAction("Dashboard"); // Or any other appropriate action
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}