using CasaDespaDraft.Models;
using CasaDespaDraft.Data;

namespace CasaDespaDraft.ViewModels
{
    public class NotificationViewModel
    {
        public IEnumerable<Notification> Notifications { get; set; }

        public IEnumerable<AdminNotification> AdminNotifications { get; set; }

        public string userId { get; set; }

        public List<Notification> Notification { get; set; }
        
    }
}