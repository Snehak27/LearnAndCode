using System;

namespace CafeteriaServer.DTO.ResponseModel
{
    public class DiscardMenuResponse
    {
        public bool IsSuccess { get; set; }
        public List<DiscardMenuItem> DiscardItems { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class DiscardMenuItem
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public double AverageRating { get; set; }
        public List<string> Sentiments { get; set; }
    }
}
