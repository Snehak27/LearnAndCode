using System;

namespace CafeteriaClient.DTO
{
    public class ViewFeedbackResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<FeedbackDTO> Feedbacks { get; set; }
    }

    public class FeedbackDTO
    {
        public string Comment { get; set; }
        public int Rating { get; set; }
        public string MenuItemName { get; set; }
    }
}
