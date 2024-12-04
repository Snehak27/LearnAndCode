using System;

namespace CafeteriaClient.DTO.Response
{
    public class NotificationResponse
    {
        public int NotificationId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
