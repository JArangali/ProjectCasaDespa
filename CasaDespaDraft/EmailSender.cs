using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using CasaDespaDraft.Data;
using CasaDespaDraft.Models;

namespace CasaDespaDraft
{
    public class EmailSender : IEmailSender
    {
        private readonly AppDbContext _dbContext;

        public EmailSender(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "thecasadespa@outlook.com";
            var pw = "casadespaofficial789";

            var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw),
                Timeout = 50000 // Set timeout to 20 seconds
            };

            return client.SendMailAsync(
                new MailMessage(from: mail,
                                to: email,
                                subject,
                                message));
        }

        public async Task SendNotificationAsync(Notification notification)
        {
            // Your email sending logic here
            var user = notification.User;
            var email = notification.RecipientEmail;
            var subject = notification.Subject;
            var message = notification.Message;

            // Send the email
            await SendEmailAsync(email, subject, message);

            // Create a new notification instance
            var newNotification = new Notification
            {
                User = user,
                Subject = notification.Subject,
                Message = notification.Message,
                SentDate = DateTime.UtcNow,
                RecipientEmail = notification.RecipientEmail
            };

            // Save the notification to the database
            _dbContext.Notifications.Add(newNotification);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SendNotificationAsync(AdminNotification notification)
        {
            // Your email sending logic here
            var user = notification.User;
            var email = notification.RecipientEmail;
            var subject = notification.Subject;
            var message = notification.Message;

            // Send the email
            await SendEmailAsync(email, subject, message);

            // Create a new notification instance
            var newNotification = new Notification
            {
                User = user,
                Subject = notification.Subject,
                Message = notification.Message,
                SentDate = DateTime.UtcNow,
                RecipientEmail = notification.RecipientEmail
            };

            // Save the notification to the database
            _dbContext.Notifications.Add(newNotification);
            await _dbContext.SaveChangesAsync();
        }
    }
}