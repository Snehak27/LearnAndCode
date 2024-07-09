using System;

namespace CafeteriaServer.DTO.ResponseModel
{
    public class DetailedFeedbacksResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<DetailedFeedbackResponse> DetailedFeedbacks { get; set; }
    }

    public class DetailedFeedbackResponse
    {
        public string MenuItemName { get; set; }
        public string FeedbackDate { get; set; }
        public string DislikeReason { get; set; }
        public string PreferredTaste { get; set; }
        public string Recipe { get; set; }
    }
}
