using System;
using System.Collections.Generic;

namespace CafeteriaServer.DTO.ResponseModel
{
    public class ViewFeedbackResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<FeedbackDTO> Feedbacks { get; set; }
    }
}
