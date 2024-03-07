using CasaDespaDraft.Models;

namespace CasaDespaDraft
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);

        Task SendNotificationAsync(Notification notification);

        Task SendNotificationAsync(AdminNotification notification);
    }
}