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
        public List<RecommendedItemResponse> Recommendations { get; set; }
    }

    public class RecommendedItemResponse
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public double PredictedRating { get; set; }
        public List<string> Comments { get; set; }
        public int RecommendedItemId { get; set; }
        public int VoteCount { get; set; }
        public double AverageRating { get; set; }
        public string OverallSentiment { get; set; }
    }
}

