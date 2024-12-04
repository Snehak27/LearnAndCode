using System;

namespace CafeteriaClient.DTO
{
    public class ResponseMessage
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}
