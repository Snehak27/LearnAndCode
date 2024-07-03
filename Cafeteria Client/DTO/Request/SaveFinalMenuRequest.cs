using System;

namespace CafeteriaClient.DTO.Request
{
    public class SaveFinalMenuRequest
    {
        public List<MealTypeMenuItemList> MealTypeMenuItems { get; set; }
    }
}
