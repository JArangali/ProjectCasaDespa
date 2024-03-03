using CasaDespaDraft.Data;
using CasaDespaDraft.Models;
using CasaDespaDraft.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
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
        private readonly IEmailSender _emailSender;

        public BookingController(ILogger<BookingController> logger, AppDbContext dbData, UserManager<User> userManager, IEmailSender emailSender)
        {
            _logger = logger;
            _dbData = dbData;
            _userManager = userManager;
            _emailSender = emailSender;
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

            var user = await _userManager.FindByIdAsync(userId);
            var email = user.Email;

            // Add the new recipe to the context
            _dbData.Bookings.Add(toAdd);

            var receiver = "granadaluis.lg@yahoo.com";
            var subject = $"New Booking Request from {newBooking.fullName}";
            var message = $"{newBooking.fullName} is trying to make a booking request with Casa Despa.";

            await _emailSender.SendEmailAsync(receiver, subject, message);

            var customer = email;
            var subjects = $"You have sucessfully submitted your request to Casa Despa.";
            var messages = $"You just submitted a booking request on Casa Despa under the name of {newBooking.fullName}";

            await _emailSender.SendEmailAsync(customer, subjects, messages);

            // Save changes to the database
            await _dbData.SaveChangesAsync();
            return RedirectToAction("Profile", "Home");
        }

        [HttpGet]
        public IActionResult Receipt(int id)
        {
            Booking? booking = _dbData.Bookings.FirstOrDefault(st => st.bookingId == id);
                
            var model = new Booking
                {
                    bookingId = id,
                    userId = booking.userId,
                    fullName = booking.fullName,
                    contactNumber = booking.contactNumber,
                    messengerLink = booking.messengerLink,
                    package = booking.package,
                    pax = booking.pax,
                    date = booking.date,
                    BStatus = booking.BStatus,
                    Amount = booking.Amount,
                    QRCode = booking.QRCode,
                };


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Receipt(Booking model, IFormFile image)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            Booking? booking = _dbData.Bookings.FirstOrDefault(st => st.bookingId == model.bookingId);

            if (booking == null)
            {
                // Handle the case where the booking is not found
                return NotFound();
            }

            if (image != null && image.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);
                booking.image = memoryStream.ToArray();
            }

            booking.Refnum = model.Refnum;

            var toUpdate = booking;
            toUpdate.BStatus = "Requested";

            _dbData.Bookings.Update(toUpdate);

            booking.Status = ProfileStatus.Pending_Payment;

            _dbData.Entry(booking).State = EntityState.Modified;
            _dbData.SaveChanges();

            return RedirectToAction("Profile", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}