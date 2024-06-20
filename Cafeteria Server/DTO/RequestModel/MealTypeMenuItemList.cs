using System;

namespace CafeteriaServer.DTO
{
    public class MealTypeMenuItemList
    {
        public int MealTypeId { get; set; }
        public List<int> MenuItemIds { get; set; }
    }
}
