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

        /*public async Task<IActionResult> Notification()
        {
            var user = await _userManager.GetUserAsync(User);
            string adminEmail = "admin@example.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (user.Email == adminEmail)
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

        /*public IActionResult Notification()
        {
            IEnumerable<Notification> notifications = _dbData.Notifications;
            NotificationViewModel viewModel = new NotificationViewModel
            {
                Notifications = notifications
            };
            return View(viewModel);
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
            var subjects = $"Booking Request Declined - Booking ID: {Archived.bookingId}";
            var messages = $"Dear {Archived.fullName}\n\nWe regret to inform you that your booking request with Booking ID: {Archived.bookingId} has been declined. If you have any questions or concerns, please feel free to contact us.\n\nThank you\nCasa Despa";

            await _emailSender.SendEmailAsync(customer, subjects, messages);

            // Create a new Notification instance
            var notification = new Notification
            {
                User = await _userManager.FindByIdAsync(Archived.userId),
                RecipientEmail = customer,
                Subject = subjects,
                Message = messages
            };

            // Send the notification
            await _emailSender.SendNotificationAsync(notification);

            _dbData.Bookings.Update(toDecline);
            _dbData.SaveChanges();
            return RedirectToAction("Dashboard", "Admin", Notification);
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
            var subjects = $"Booking Canceled - Booking ID: {Archived.bookingId}";
            var messages = $"Dear {Archived.fullName}\n\nWe regret to inform you that your booking with Booking ID: {Archived.bookingId} has been canceled. If you have any questions or concerns, please contact us.\n\nThank you\nCasa Despa";

            await _emailSender.SendEmailAsync(customer, subjects, messages);

            // Create a new Notification instance
            var notification = new Notification
            {
                User = await _userManager.FindByIdAsync(Archived.userId),
                RecipientEmail = customer,
                Subject = subjects,
                Message = messages
            };
            /*var users = await _userManager.FindByIdAsync(notification.UserId);*/

            // Send the notification
            await _emailSender.SendNotificationAsync(notification);

            _dbData.Bookings.Update(toCancel);
            _dbData.SaveChanges();
            return RedirectToAction("Dashboard", "Admin", Notification);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DashboardAcceptedBooking(int id, DayDates newDayDate, NightInDates newNightInDate, NightOutDates newNightOutDate, TTInDates newTTInDate, TTOutDates newTTOutDate)
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
            var subjects = $"Booking Approved - Booking ID: {Accepted.bookingId}";
            var messages = $"Dear {Accepted.fullName}\n\nWe have received and verified your proof of down payment for Booking ID: {Accepted.bookingId}. Your booking is now confirmed.\n\nThank you\nCasa Despa";

            await _emailSender.SendEmailAsync(customer, subjects, messages);

            // Create a new Notification instance
            var notification = new Notification
            {
                User = await _userManager.FindByIdAsync(Accepted.userId),
                RecipientEmail = customer,
                Subject = subjects,
                Message = messages
            };

            if (Accepted.package == Package.Day_Tour) 
            {
                newDayDate = new DayDates
                {
                    dayDate = Accepted.date
                };
                _dbData.DayDates.Add(newDayDate);
            }

            if (Accepted.package == Package.Night_Tour)
            {
                DateTime bookingDate = DateTime.Parse(Accepted.date);

                newNightInDate = new NightInDates
                {
                    nightInDate = Accepted.date
                };
                _dbData.NightInDates.Add(newNightInDate);

                var nextDayDate = bookingDate.AddDays(1).ToString("yyyy-MM-dd");
                newNightOutDate = new NightOutDates
                {
                    nightOutDate = nextDayDate
                };
                _dbData.NightOutDates.Add(newNightOutDate);
            }

            if (Accepted.package == Package.Twenty_Two_Hours)
            {
                DateTime bookingDate = DateTime.Parse(Accepted.date);

                newTTInDate = new TTInDates
                {
                    ttInDate = Accepted.date
                };
                _dbData.TTInDates.Add(newTTInDate);

                var nextDayDate = bookingDate.AddDays(1).ToString("yyyy-MM-dd");
                newTTOutDate = new TTOutDates
                {
                    ttOutDate = nextDayDate
                };
                _dbData.TTOutDates.Add(newTTOutDate);
            }

            /*var users = await _userManager.FindByIdAsync(notification.UserId);*/

            // Send the notification
            await _emailSender.SendNotificationAsync(notification);

            _dbData.Bookings.Update(toAccept);
            _dbData.SaveChanges();
            return RedirectToAction("Dashboard", "Admin", Notification);
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
            var subjects = $"Booking Done - Booking ID: {Archived.bookingId}";
            var messages = $"Dear {Archived.fullName}\n\nThank you for choosing Casa Despa! Your booking with Booking ID: {Archived.bookingId} has been completed. We look forward for your next visit.\n\nThank you\nCasa Despa";


            await _emailSender.SendEmailAsync(customer, subjects, messages);

            // Create a new Notification instance
            var notification = new Notification
            {
                User = await _userManager.FindByIdAsync(Archived.userId),
                RecipientEmail = customer,
                Subject = subjects,
                Message = messages
            };
            /*var users = await _userManager.FindByIdAsync(notification.UserId);*/

            // Send the notification
            await _emailSender.SendNotificationAsync(notification);

            _dbData.Bookings.Update(toArchive);
            _dbData.SaveChanges();
            return RedirectToAction("Dashboard", "Admin", Notification);
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
                BStatus = booking.BStatus,
                accomodation = booking.accomodation
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

            booking.Remarks = "Please Pay the Downpayment Shown Below";

            var toUpdate = booking;
            toUpdate.BStatus = "Requested";

            _dbData.Bookings.Update(toUpdate);

            booking.Status = ProfileStatus.Pending_Payment;

            var user = await _userManager.FindByIdAsync(booking.userId);
            var email = user.Email;

            var customer = email;
            var subjects = $"Down Payment Request for Booking - Booking ID : {booking.bookingId}";
            var messages = $"Dear {booking.fullName}\n\nOur team is requesting the down payment for your booking with Booking ID: {booking.bookingId}. Kindly proceed with the payment at your earliest convenience to secure your booking. Failure to do so may result in the cancellation of your booking.\n\nThank you\nCasa Despa";

            await _emailSender.SendEmailAsync(customer, subjects, messages);

            // Create a new Notification instance
            var notification = new Notification
            {
                User = await _userManager.FindByIdAsync(booking.userId),
                RecipientEmail = customer,
                Subject = subjects,
                Message = messages
            };
            /*var users = await _userManager.FindByIdAsync(notification.UserId);*/

            // Send the notification
            await _emailSender.SendNotificationAsync(notification);

            _dbData.Entry(booking).State = EntityState.Modified;
            _dbData.SaveChanges();

            return RedirectToAction("Dashboard", "Admin", Notification);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Rerequest(int id)
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
                accomodation = booking.accomodation
            };


            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Rerequest(Booking model, IFormFile QRCode)
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

            booking.Remarks = model.Remarks;

            var toUpdate = booking;
            toUpdate.BStatus = "Requested";

            _dbData.Bookings.Update(toUpdate);

            booking.Status = ProfileStatus.Pending_Payment;

            var user = await _userManager.FindByIdAsync(booking.userId);
            var email = user.Email;

            var customer = email;
            var subjects = $"Rerequest for Down Payment";
            var messages = $"Casa Despa is Rerequesting for the down payment for booking request under the name of {booking.fullName}";

            await _emailSender.SendEmailAsync(customer, subjects, messages);

            // Create a new Notification instance
            var notification = new Notification
            {
                User = await _userManager.FindByIdAsync(booking.userId),
                RecipientEmail = customer,
                Subject = subjects,
                Message = messages
            };
            /*var users = await _userManager.FindByIdAsync(notification.UserId);*/

            // Send the notification
            await _emailSender.SendNotificationAsync(notification);

            _dbData.Entry(booking).State = EntityState.Modified;
            _dbData.SaveChanges();

            return RedirectToAction("Dashboard", "Admin", Notification);
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
            var subjects = $"Booking Details Updated - Booking ID:{booking.bookingId}";
            var messages = $"Dear {booking.fullName}\n\nYour request changes to the booking details for Booking ID: {booking.bookingId} has been processed. Please review the updated information, and let us know if you have any further requests.\n\nThank you\nCasa Despa";

            await _emailSender.SendEmailAsync(customer, subjects, messages);

            // Create a new Notification instance
            var notification = new Notification
            {
                User = await _userManager.FindByIdAsync(booking.userId),
                RecipientEmail = customer,
                Subject = subjects,
                Message = messages
            };
            /*var users = await _userManager.FindByIdAsync(notification.UserId);*/

            // Send the notification
            await _emailSender.SendNotificationAsync(notification);

            _dbData.Bookings.Update(booking);
            _dbData.SaveChanges();
            return RedirectToAction("Dashboard", "Admin", Notification);
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