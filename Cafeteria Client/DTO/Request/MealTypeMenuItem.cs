using System;

namespace CafeteriaClient.DTO
{
    public class MealTypeMenuItem
    {
        public int MealTypeId { get; set; }
        public List<int> MenuItemIds { get; set; }
    }
    public class SaveFinalMenuRequest
    {
        public List<MealTypeMenuItem> MealTypeMenuItems { get; set; }
    }
}
