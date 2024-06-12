using System.ComponentModel.DataAnnotations;

namespace CafeteriaServer.DAL.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }
        public string UserId { get; set; }
        public int MenuItemId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime FeedbackDate { get; set; }
    }
}
