using System;

namespace CafeteriaClient.DTO.Request
{
    public class FeedbackRequest
    {
        public int UserId { get; set; }
        public int MenuItemId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public int MealTypeId { get; set; }
    }
    public class PastOrderDTO
    {
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public DateTime OrderDate { get; set; }
        public int MealTypeId { get; set; }
    }

    public class PastOrdersResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<PastOrderDTO> PastOrders { get; set; }
    }
}
