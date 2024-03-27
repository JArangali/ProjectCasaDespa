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

            if (user.Email == "admin@example.com")
            {
                // Handle user not found
                return RedirectToAction("Index", "Home"); // Redirect to another action or handle appropriately
            }

            return View();
        }


        public async Task<IActionResult> Notification()
        {
            var user = await _userManager.GetUserAsync(User);
            string adminEmail = "admin@example.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (user.Email == adminEmail)
            {
                return View("AdminNotification");
            }
            else
            {
                return View("Notification");
            }
        }

        /*public IActionResult Notification()
        {
            var user = _userManager.GetUserAsync(User);
            string adminEmail = "admin@example.com";
            var adminUser = _userManager.FindByEmailAsync(adminEmail).Result;

            if (user.Result.Email == adminEmail)
            {
                IEnumerable<AdminNotification> notification = _dbData.AdminNotification;
                NotificationViewModel viewModels = new NotificationViewModel
                {
                    AdminNotifications = notification
                };

                return View(viewModels);
            }
            else
            {
                // Show the regular notifications
                IEnumerable<Notification> notifications = _dbData.Notifications;
                NotificationViewModel viewModel = new NotificationViewModel
                {
                    Notifications = notifications
                };

                return View(viewModel);
            }
        }*/

        public IActionResult GetUnreadNotificationCount()
        {
            var userId = _userManager.GetUserId(User);
            var unreadNotifications = _dbData.Notifications.Where(n => n.userId == userId && !n.IsRead).Count();
            return Json(new { unreadNotifications });
        }

        public IActionResult MarkNotificationsAsRead()
        {
            var userId = _userManager.GetUserId(User);
            var notifications = _dbData.Notifications.Where(n => n.userId == userId).ToList();
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }
            _dbData.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Booking(Booking newBooking)
        {
            bool IsDayDateBooked(List<DayDates> bookings)
            {
                return bookings.Any(b => b.dayDate == newBooking.date);
            }

            bool IsNightInDateBooked(List<NightInDates> bookings)
            {
                return bookings.Any(b => b.nightInDate == newBooking.date);
            }

            bool IsNightOutDateBooked(List<NightOutDates> bookings)
            {
                return bookings.Any(b => b.nightOutDate == newBooking.date);
            }

            bool IsTTInDateBooked(List<TTInDates> bookings)
            {
                return bookings.Any(b => b.ttInDate == newBooking.date);
            }

            bool IsTTOutDateBooked(List<TTOutDates> bookings)
            {
                return bookings.Any(b => b.ttOutDate == newBooking.date);
            }

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

                if (newBooking.package == Package.Day_Tour)
                {
                    if (IsDayDateBooked(_dbData.DayDates.ToList()))
                    {
                        ModelState.AddModelError("Date", "Sorry, the chosen date is already booked for a Day Tour.");
                        return View(newBooking);
                    }
                    else if (IsTTInDateBooked(_dbData.TTInDates.ToList()))
                    {
                        ModelState.AddModelError("Date", "Sorry, the chosen date is already booked for a Twenty-Two Hour Tour.");
                        return View(newBooking);
                    }
                    else if (IsTTOutDateBooked(_dbData.TTOutDates.ToList()))
                    {
                        ModelState.AddModelError("Date", "Sorry, booked customers are set to check out at 2pm on the chosen date.");
                        return View(newBooking);
                    }
                }

                if (newBooking.package == Package.Night_Tour)
                {
                    if (IsNightInDateBooked(_dbData.NightInDates.ToList()))
                    {
                        ModelState.AddModelError("Date", "Sorry, the chosen date is already booked for a Night Tour.");
                        return View(newBooking);
                    }
                    else if (IsTTInDateBooked(_dbData.TTInDates.ToList()))
                    {
                        ModelState.AddModelError("Date", "Sorry, the chosen date is already booked for a Twenty-Two Hour Tour.");
                        return View(newBooking);
                    }
                }

                if (newBooking.package == Package.Twenty_Two_Hours)
                {
                    if (IsDayDateBooked(_dbData.DayDates.ToList()))
                    { 
                        ModelState.AddModelError("Date", "Sorry, the chosen date is already booked for a Day Tour.");
                        return View(newBooking);
                    }
                    else if (IsTTInDateBooked(_dbData.TTInDates.ToList()))
                    {
                        
                        ModelState.AddModelError("Date", "Sorry, the chosen date is already booked for a Twenty-Two Hour Tour.");
                        return View(newBooking);
                    }
                    else if (IsNightInDateBooked(_dbData.NightInDates.ToList()))
                    {
                        ModelState.AddModelError("Date", "Sorry, the chosen date is already booked for a Night Tour.");
                        return View(newBooking);
                    }
                }

                var user = await _userManager.FindByIdAsync(userId);
                var email = user.Email;

                newBooking.allowed = true;

                // Add the new recipe to the context
                _dbData.Bookings.Add(toAdd);

                var receiver = "granadaluisss@gmail.com";
                var subject = $"New Booking Request Received - {newBooking.fullName} - Booking Date: {newBooking.date}";
                var message = $"Dear Admin\n\nA new booking request has been submitted by {newBooking.fullName} for the date {newBooking.date}. Please view the details.\n\nThank you. \n\nCasa Despa";

                await _emailSender.SendEmailAsync(receiver, subject, message);

                var customer = email;
                var subjects = $"Booking Request to Casa Despa";
                var messages = $"Dear {newBooking.fullName}\n\nYou have successfully submitted your booking request to Casa Despa!\nBooking Package : {newBooking.package}\nRequested Date: {newBooking.date}\n\nThank you\nCasa Despa";

/*                await _emailSender.SendEmailAsync(customer, subjects, messages);
*/
                //NOTIFICATION
                //ADMIN
                var notification = new AdminNotification
                {
                    User = await _userManager.FindByEmailAsync("admin@example.com"),
                    RecipientEmail = receiver,
                    Subject = subject,
                    Message = message
                };
                await _emailSender.SendNotificationAsync(notification);

                //CUSTOMER
                // Create a new Notification instance
                var notifications = new Notification
                {
                    User = await _userManager.FindByIdAsync(userId),
                    RecipientEmail = customer,
                    Subject = subjects,
                    Message = messages
                };
                // Send the notification
                await _emailSender.SendNotificationAsync(notifications);


                // Save changes to the database
                await _dbData.SaveChangesAsync();
                return RedirectToAction("Profile", "Home", Notification);
            }
            return View(newBooking);
        }

        [HttpGet]
        public IActionResult AllBooking(string bStatus)
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                // Handle user not found
                return RedirectToAction("Login", "Account"); // Redirect to another action or handle appropriately
            }

            // Assuming _dbData is your database context with a DbSet<Bookings>
            var allBookings = _dbData.Bookings
                .Where(b => b.Status == ProfileStatus.Approved && b.BStatus == bStatus)
                .ToList();

            // Pass the list of bookings to the partial view
            return PartialView("_AllBookingsPartial", allBookings);
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
                accomodation = booking.accomodation,
                Remarks = booking.Remarks,
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

            var receiver = "granadaluisss@gmail.com";
            var subject = $"Proof of payment received - {booking.fullName} - Booking ID: {booking.bookingId}";
            var message = $"Dear Admin\n\n{booking.fullName} has submitted their proof of payment for Booking ID: {booking.bookingId}. Please verify and proceed with the next steps accordingly.\n\nThank you\n\tCasa Despa";

            await _emailSender.SendEmailAsync(receiver, subject, message);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var users = await _userManager.FindByIdAsync(userId);

            var user = await _userManager.FindByIdAsync(booking.userId);
            var email = user.Email;

            var customer = email;
            var subjects = $"Proof of Down Payment Received - Booking ID: {booking.bookingId}";
            var messages = $"Dear {booking.fullName}\n\nYour proof of payment for Booking ID : {booking.bookingId} has been successfully received. Our team will review and process it accordingly.\n\nThank you\nCasa Despa";

            /*await _emailSender.SendEmailAsync(customer, subjects, messages);*/

            //NOTIFICATION
            /*var currentUser = await _userManager.GetUserAsync(User);
            var adminUser = await _userManager.FindByEmailAsync("admin@example.com");
*/
           
                var notifications = new AdminNotification
                {
                    User = await _userManager.FindByEmailAsync("admin@example.com"),
                    RecipientEmail = receiver,
                    Subject = subject,
                    Message = message
                };
                await _emailSender.SendNotificationAsync(notifications);
            
                // Create a new Notification instance
                var notification = new Notification
                {
                    User = await _userManager.FindByIdAsync(booking.userId),
                    RecipientEmail = customer,
                    Subject = subjects,
                    Message = messages
                };
                /*var userss = await _userManager.FindByIdAsync(notification.UserId);*/

                // Send the notification
                await _emailSender.SendNotificationAsync(notification);
            

            _dbData.Entry(booking).State = EntityState.Modified;
            _dbData.SaveChanges();

            return RedirectToAction("Profile", "Home", Notification);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}