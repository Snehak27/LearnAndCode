using System;

namespace CafeteriaClient.DTO
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
        public int RecommendedItemId { get; set; }
        public int VoteCount { get; set; }
        public double AverageRating { get; set; }
        public string OverallSentiment { get; set; }
    }
    public class EmployeeRecommendationResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<MealTypeRecommendation> MealTypeRecommendations { get; set; }
    }
    public class MealTypeRecommendation
    {
        public int MealTypeId { get; set; }
        public string MealTypeName { get; set; }
        public List<RecommendedItemDTO> RecommendedItems { get; set; }
    }
}
