using System;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaServer.DAL.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int RecommendedItemId { get; set; }

        public virtual Order Order { get; set; }
        public virtual RecommendedItem RecommendedItem { get; set; }
    }
}
