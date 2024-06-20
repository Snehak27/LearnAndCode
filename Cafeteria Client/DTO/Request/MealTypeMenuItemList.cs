using System;

namespace CafeteriaClient.DTO
{
    public class MealTypeMenuItemList
    {
        public int MealTypeId { get; set; }
        public List<int> MenuItemIds { get; set; }
    }
}
