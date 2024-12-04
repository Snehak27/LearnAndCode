using System;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaServer.DAL.Models
{
    public class NotificationType
    {
        [Key]
        public int NotificationTypeId { get; set; }

        public string NotificationMessage { get; set; }
    }
}
