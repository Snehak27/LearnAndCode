using System;

namespace CafeteriaServer.DAL.Models
{
    public class Recommendation
    {
        public int RecommendationId { get; set; }
        public int MealTypeId { get; set; }
        public DateTime RecommendationDate { get; set; }

        public virtual MealType MealType { get; set; }
    }
}
