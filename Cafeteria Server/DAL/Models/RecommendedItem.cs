using System;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaServer.DAL.Models
{
    public class RecommendedItem
    {
        [Key]
        public int RecommendedItemId { get; set; }
        public int RecommendationId { get; set; }
        public int MenuItemId { get; set; }

        public virtual Recommendation Recommendation { get; set; }
        public virtual MenuItem MenuItem { get; set; }
    }
}
