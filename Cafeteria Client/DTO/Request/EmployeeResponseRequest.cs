using System;

namespace CafeteriaClient.DTO.Request
{
    public class EmployeeResponseRequest
    {
        public int UserId { get; set; }
        public List<Vote> Votes { get; set; }
    }

    public class Vote
    {
        public int MenuItemId { get; set; }
        public int MealTypeId { get; set; }
        public int RecommendedItemId { get; set; }
    }
}
