using System;

namespace CafeteriaServer.DTO
{
    public class PastOrderResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<PastOrderDTO> PastOrders { get; set; }
    }
}
