using System;

namespace CafeteriaServer.DTO
{
    public class PastOrderResponse
    {
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public DateTime OrderDate { get; set; }
        public int MealTypeId { get; set; }
    }

    public class PastOrdersResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<PastOrderResponse> PastOrders { get; set; }
    }
}
