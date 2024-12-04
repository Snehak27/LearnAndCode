using System;

namespace CafeteriaServer.DTO.RequestModel
{
    public class DetailedFeedbackRequest
    {
        public int UserId { get; set; }
        public int MenuItemId { get; set; }
        public List<string> Answers { get; set; }
    }
}
