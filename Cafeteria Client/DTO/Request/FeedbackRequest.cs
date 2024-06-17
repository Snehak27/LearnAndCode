using System;

namespace CafeteriaClient.DTO.Request
{
    public class FeedbackRequest
    {
        public int UserId { get; set; }
        public int MenuItemId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public int MealTypeId { get; set; }
    }
}
