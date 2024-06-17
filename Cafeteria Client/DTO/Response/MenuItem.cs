using System;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaClient.DTO
{
    public class MenuItem
    {
        [Key]
        public int MenuItemId { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public bool AvailabilityStatus { get; set; }
    }
}
