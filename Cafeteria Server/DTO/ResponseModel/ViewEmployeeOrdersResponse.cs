using System;

namespace CafeteriaServer.DTO
{
    public class ViewEmployeeOrdersResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<EmployeeOrderSummary> EmployeeOrders { get; set; }
    }
    public class EmployeeOrderSummary
    {
        public int MealTypeId { get; set; }
        public string MealTypeName { get; set; }
        public List<MenuItemOrder> MenuItemOrders { get; set; }
    }

    public class MenuItemOrder
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public int OrderCount { get; set; }
    }
}
