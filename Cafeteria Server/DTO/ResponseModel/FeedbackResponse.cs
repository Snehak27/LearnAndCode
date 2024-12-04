using System;

namespace CafeteriaServer.DTO
{
    public class FeedbackResponse
    {
        public string Comment { get; set; }
        public int Rating { get; set; }
        public string MenuItemName { get; set; }
    }
}
