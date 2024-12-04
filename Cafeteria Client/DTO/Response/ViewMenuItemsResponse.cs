using System;

namespace CafeteriaClient.DTO
{
    public class ViewMenuItemsResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<MenuItem> MenuItems { get; set; }
    }
}
