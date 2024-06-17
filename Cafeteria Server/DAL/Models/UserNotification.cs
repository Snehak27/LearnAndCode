using System;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaServer.DAL.Models
{
    public class UserNotification
    {
        [Key]
        public int UserNotificationId { get; set; }
        public int UserId { get; set; }
        public int NotificationTypeId { get; set; }
        public bool IsRead { get; set; }

        public virtual User User { get; set; }
        public virtual NotificationType NotificationType { get; set; }
    }
}
