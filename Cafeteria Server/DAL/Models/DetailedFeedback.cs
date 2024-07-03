using System;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaServer.DAL.Models
{
    public class DetailedFeedback
    {
        [Key]
        public int DetailedFeedbackId { get; set; }
        public int UserId { get; set; }
        public int MenuItemId { get; set; }
        public string DislikeReason { get; set; } 
        public string PreferredTaste { get; set; } 
        public string Recipe { get; set; }
        public DateTime FeedbackDate { get; set; }

        public virtual User User { get; set; }
        public virtual MenuItem MenuItem { get; set; }
    }
}
