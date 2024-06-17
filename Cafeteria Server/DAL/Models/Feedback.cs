using System.ComponentModel.DataAnnotations;

namespace CafeteriaServer.DAL.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }
        public int UserId { get; set; }
        public int MenuItemId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime FeedbackDate { get; set; }
        public int MealTypeId { get; set; }

        public virtual MenuItem MenuItem { get; set; }
        public virtual User User { get; set; }
        public virtual MealType MealType { get; set; }
    }
}
