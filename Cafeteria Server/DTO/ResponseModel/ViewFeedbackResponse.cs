using System;
using System.Collections.Generic;

namespace CafeteriaServer.DTO
{
    public class ViewFeedbackResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<FeedbackResponse> Feedbacks { get; set; }
    }
}
