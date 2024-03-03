using Azure.Core;
using CasaDespaDraft.Data;
using CasaDespaDraft.Models;
using CasaDespaDraft.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;

namespace CasaDespaDraft.Controllers
{
    public class AdminController : Controller
    {

        private readonly AppDbContext _dbData;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        private readonly ILogger<HomeController> _logger;

        public AdminController(ILogger<HomeController> logger, AppDbContext dbData, UserManager<User> userManager, IEmailSender emailSender)
        {
            _logger = logger;
            _dbData = dbData;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {

            var bookings = _dbData.Bookings.Where(B => B.BStatus == "Pending").ToList();
            var requested = _dbData.Bookings.Where(B => B.BStatus == "Requested").ToList();
            var accepted = _dbData.Bookings.Where(B => B.BStatus == "Accepted").ToList();
            var archive = _dbData.Bookings.Where(B => (B.BStatus == "Completed" || B.BStatus == "Cancelled" || B.BStatus == "Declined")).ToList();

            var viewModel = new AccountViewModel
            {
                Bookings = bookings,
                Requested = requested,
                Accepted = accepted,
                Archive = archive
            };

            return View(viewModel);
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
        public async Task<IActionResult> DashboardBookingDecline(int id)
        {
            Booking? Archived = _dbData.Bookings.FirstOrDefault(st => st.bookingId == id);

            if (Archived == null)
            {
                // Handle the case where the booking is not found
                return NotFound();
            }

            var toDecline = Archived;
            toDecline.BStatus = "Declined";

            Archived.Status = ProfileStatus.Archive;

            var user = await _userManager.FindByIdAsync(Archived.userId);
            var email = user.Email;

            var customer = email;
            var subjects = $"DECLINED BOOKING REQUEST";
            var messages = $"Casa Despa has declined your booking request under the name of {Archived.fullName}";

            await _emailSender.SendEmailAsync(customer, subjects, messages);

            _dbData.Bookings.Update(toDecline);
            _dbData.SaveChanges();
            return RedirectToAction("Dashboard", "Admin");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DashboardBookingCancelled(int id)
        {
            Booking? Archived = _dbData.Bookings.FirstOrDefault(st => st.bookingId == id);

            if (Archived == null)
            {
                // Handle the case where the booking is not found
                return NotFound();
            }

            if (Archived == null)
            {
                // Handle the case where the booking is not found
                return NotFound();
            }

            var toCancel = Archived;
            toCancel.BStatus = "Cancelled";

            Archived.Status = ProfileStatus.Archive;

            var user = await _userManager.FindByIdAsync(Archived.userId);
            var email = user.Email;

            var customer = email;
            var subjects = $"CANCELLED BOOKING TRANSACTION";
            var messages = $"Casa Despa has cancelled your booking under the name of {Archived.fullName}";

            await _emailSender.SendEmailAsync(customer, subjects, messages);

            _dbData.Bookings.Update(toCancel);
            _dbData.SaveChanges();
            return RedirectToAction("Dashboard", "Admin");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DashboardAcceptedBooking(int id)
        {
            Booking? Accepted = _dbData.Bookings.FirstOrDefault(st => st.bookingId == id);

            if (Accepted == null)
            {
                // Handle the case where the booking is not found
                return NotFound();
            }

            var toAccept = Accepted;
            toAccept.BStatus = "Accepted";

            Accepted.Status = ProfileStatus.Approved;

            var user = await _userManager.FindByIdAsync(Accepted.userId);
            var email = user.Email;

            var customer = email;
            var subjects = $"APPROVED BOOKING";
            var messages = $"Casa Despa has reviewed and approved your booking request under the name of {Accepted.fullName}";

            await _emailSender.SendEmailAsync(customer, subjects, messages);

            _dbData.Bookings.Update(toAccept);
            _dbData.SaveChanges();
            return RedirectToAction("Dashboard", "Admin");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DashboardArchive(int id)
        {
            Booking? Archived = _dbData.Bookings.FirstOrDefault(st => st.bookingId == id);

            if (Archived == null)
            {
                // Handle the case where the booking is not found
                return NotFound();
            }

            var toArchive = Archived;
            toArchive.BStatus = "Completed";

            Archived.Status = ProfileStatus.Archive;

            var user = await _userManager.FindByIdAsync(Archived.userId);
            var email = user.Email;

            var customer = email;
            var subjects = $"BOOKING DONE";
            var messages = $"Thank you for visiting CASA DESPA!";

            await _emailSender.SendEmailAsync(customer, subjects, messages);

            _dbData.Bookings.Update(toArchive);
            _dbData.SaveChanges();
            return RedirectToAction("Dashboard", "Admin");
        }

        [HttpGet]
        public IActionResult DashboardDeleteBooking(int id)
        {
            Booking? bookingToDelete = _dbData.Bookings.FirstOrDefault(st => st.bookingId == id);

            if (bookingToDelete == null)
            {
                // Handle the case where the booking is not found
                return NotFound();
            }

            _dbData.Bookings.Remove(bookingToDelete);
            _dbData.SaveChanges();

            return RedirectToAction("Dashboard", "Admin");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AddQr(int id)
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
                BStatus = booking.BStatus
            };


            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddQR(Booking model, IFormFile QRCode)
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

            if (QRCode != null && QRCode.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await QRCode.CopyToAsync(memoryStream);
                booking.QRCode = memoryStream.ToArray();
            }

            booking.Amount = model.Amount;

            var toUpdate = booking;
            toUpdate.BStatus = "Requested";

            _dbData.Bookings.Update(toUpdate);

            booking.Status = ProfileStatus.Pending_Payment;

            var user = await _userManager.FindByIdAsync(booking.userId);
            var email = user.Email;

            var customer = email;
            var subjects = $"Request for Down Payment";
            var messages = $"Casa Despa is requesting for the down payment for booking request under the name of {booking.fullName}";

            await _emailSender.SendEmailAsync(customer, subjects, messages);

            _dbData.Entry(booking).State = EntityState.Modified;
            _dbData.SaveChanges();

            return RedirectToAction("Dashboard", "Admin");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ShowReceipt(int id)
        {
            Booking? booking = _dbData.Bookings.FirstOrDefault(st => st.bookingId == id);

            var image = booking.image;
            if (image == null)
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            return View(booking);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ShowReceipt_Archive(int id)
        {
            Booking? booking = _dbData.Bookings.FirstOrDefault(st => st.bookingId == id);

            var image = booking.image;
            if (image == null)
            {
                return RedirectToAction("Dasboard", "Admin");
            }

            return View(booking);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EditBooking(int id)
        {
            Booking? booking = _dbData.Bookings.FirstOrDefault(st => st.bookingId == id);

            return View(booking);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditBooking(Booking model, int id)
        {
            Booking? booking = _dbData.Bookings.FirstOrDefault(st => st.bookingId == id);

            booking.fullName = model.fullName;
            booking.contactNumber = model.contactNumber;
            booking.messengerLink = model.messengerLink;
            booking.package = model.package;
            booking.pax = model.pax;
            booking.date = model.date;

            var user = await _userManager.FindByIdAsync(booking.userId);
            var email = user.Email;

            var customer = email;
            var subjects = $"UPDATED BOOKING DETAILS";
            var messages = $"Casa Despa has updated your booking under the name of {booking.fullName} based on your inquiry.";

            await _emailSender.SendEmailAsync(customer, subjects, messages);

            _dbData.Bookings.Update(booking);
            _dbData.SaveChanges();
            return RedirectToAction("Dashboard", "Admin");
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult CancelQR(int id)
        {
            return RedirectToAction("Dashboard", "Admin");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}