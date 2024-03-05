﻿using CasaDespaDraft.Data;
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

            if (user.Email == "admin@example.com")
            {
                // Handle user not found
                return RedirectToAction("Index", "Home"); // Redirect to another action or handle appropriately
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Booking(Booking newBooking)
        {

            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var toAdd = newBooking;
                toAdd.BStatus = "Pending";

                newBooking.Status = ProfileStatus.Requests;

                // Set the UserId of the new recipe
                newBooking.userId = userId;

                if (newBooking.accomodation == "REGULAR")
                {
                    if (newBooking.pax >= 21)
                    {
                        ModelState.AddModelError("answer", "The number of Pax for regular accomodations must range from 1-20.");
                        return View(newBooking);
                    }
                }

                if (newBooking.accomodation == "PARTY")
                {
                    if (newBooking.package != Package.Twenty_Two_Hours)
                    {
                        ModelState.AddModelError("answer", "Party Accomodations are only available for TWENTY-TWO HOURS booking packages.");
                        return View(newBooking);
                    }
                    if (newBooking.pax >= 51)
                    {
                        ModelState.AddModelError("answer", "The number of Pax for party accomodations must range from 21-50.");
                        return View(newBooking);
                    }
                    else if (newBooking.pax <= 20)
                    {
                        ModelState.AddModelError("answer", "The number of Pax for party accomodations must range from 21-50.");
                        return View(newBooking);
                    }
                }

                var user = await _userManager.FindByIdAsync(userId);
                var email = user.Email;

                // Add the new recipe to the context
                _dbData.Bookings.Add(toAdd);

                var receiver = "granadaluisss@gmail.com";
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
            return View(newBooking);
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

            var receiver = "alyssamarierromen@gmail.com";
            var subject = $"SUBMITTED PROOF OF DOWN PAYMENT";
            var message = $"{booking.fullName} has submitted their proof of down payment.";

            await _emailSender.SendEmailAsync(receiver, subject, message);

            var user = await _userManager.FindByIdAsync(booking.userId);
            var email = user.Email;

            var customer = email;
            var subjects = $"CONFIRMATION EMAIL FOR UPLOADED PROOF OF PAYMENT";
            var messages = $"You have submitted your proof of down payment to Casa Despa. Please wait for the Casa Despa administrators to check your payment.";

            await _emailSender.SendEmailAsync(customer, subjects, messages);

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