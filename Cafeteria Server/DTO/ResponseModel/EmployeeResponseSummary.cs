using System;

namespace CafeteriaServer.DTO
{
    public class EmployeeResponseSummary
    {
        public int MealTypeId { get; set; }
        public string MealTypeName { get; set; }
        public List<MenuItemVote> MenuItemVotes { get; set; }
    }

    public class MenuItemVote
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public int VoteCount { get; set; }
    }

    public class ViewEmployeeResponseResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<EmployeeResponseSummary> EmployeeResponses { get; set; }
    }
}
