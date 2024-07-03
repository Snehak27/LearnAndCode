using System;

namespace CafeteriaServer.DTO.ResponseModel
{
    public class DetailedFeedbackResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<DetailedFeedbackDTO> DetailedFeedbacks { get; set; }
    }

    public class DetailedFeedbackDTO
    {
        public string MenuItemName { get; set; }
        public string FeedbackDate { get; set; }
        public string DislikeReason { get; set; }
        public string PreferredTaste { get; set; }
        public string Recipe { get; set; }
    }
}
