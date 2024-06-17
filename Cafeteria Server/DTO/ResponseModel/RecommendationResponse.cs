using System;

namespace CafeteriaServer.DTO
{
    public class RecommendationResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<MealTypeRecommendations> MealTypeRecommendations { get; set; }
    }

    public class MealTypeRecommendations
    {
        public int MealTypeId { get; set; }
        public List<RecommendedItemDTO> Recommendations { get; set; }
    }

    public class RecommendedItemDTO
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public double PredictedRating { get; set; }
        public List<string> Comments { get; set; }
    }
}
