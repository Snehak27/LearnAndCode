using System;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaServer.DAL.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }

        public virtual User User { get; set; }
    }
}
