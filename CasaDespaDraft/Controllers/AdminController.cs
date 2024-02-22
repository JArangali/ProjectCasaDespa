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

            var bookings = _dbData.Bookings.Where(B => B.BStatus == "Pending").ToList();
            var requested = _dbData.Bookings.Where(B => B.BStatus == "Requested").ToList();
            var accepted = _dbData.Bookings.Where(B => B.BStatus == "Accepted").ToList();
            var archive = _dbData.Bookings.Where(B => (B.BStatus == "Completed"||B.BStatus == "Cancelled" || B.BStatus == "Declined")).ToList();

            var viewModel = new AccountViewModel
            {
                Bookings = bookings,
                Requested = requested,
                Accepted = accepted,
                Archive = archive
            };

            return View(viewModel);

            return View(_dbData.Bookings);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ShowDetail(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            Booking? bookings = _dbData.Bookings.FirstOrDefault(st => st.bookingId == id);

            if (bookings != null)
                return View(bookings);

            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult DashboardRequestPayment(int id)
        {
            Booking? Requested = _dbData.Bookings.FirstOrDefault(st => st.bookingId == id);

            if (Requested == null)
            {
                // Handle the case where the booking is not found
                return NotFound();
            }

            var toUpdate = Requested;
            toUpdate.BStatus = "Requested";

            _dbData.Bookings.Update(toUpdate);
            _dbData.SaveChanges();
            return RedirectToAction("Dashboard", "Admin");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult DashboardAcceptedBooking(int id)
        {
            Booking? Accepted = _dbData.Bookings.FirstOrDefault(st => st.bookingId == id);

            if (Accepted == null)
            {
                // Handle the case where the booking is not found
                return NotFound();
            }

            var toAccept = Accepted;
            toAccept.BStatus = "Accepted";

            _dbData.Bookings.Update(toAccept);
            _dbData.SaveChanges();
            return RedirectToAction("Dashboard", "Admin");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult DashboardArchive(int id)
        {
            Booking? Archive = _dbData.Bookings.FirstOrDefault(st => st.bookingId == id);

            if (Archive == null)
            {
                // Handle the case where the booking is not found
                return NotFound();
            }

            var toArchive = Archive;
            toArchive.BStatus = "Completed";

            _dbData.Bookings.Update(toArchive);
            _dbData.SaveChanges();
            return RedirectToAction("Dashboard", "Admin");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}