using System;

namespace CafeteriaServer.DTO
{
    public class EmployeeOderRequest
    {
        public int UserId { get; set; }
        public List<OrderRequest> OrderList { get; set; }
    }

    public class OrderRequest
    {
        public int MenuItemId { get; set; }
        public int MealTypeId { get; set; }
        public int RecommendedItemId { get; set; }
    }
}
