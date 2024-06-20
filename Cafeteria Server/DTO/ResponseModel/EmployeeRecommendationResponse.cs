using System;

namespace CafeteriaServer.DTO
{
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
        public List<RecommendedItemResponse> RecommendedItems { get; set; }
    }
}
