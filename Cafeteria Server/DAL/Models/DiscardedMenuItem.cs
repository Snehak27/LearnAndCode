using System;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaServer.DAL.Models
{
    public class DiscardedMenuItem
    {
        [Key]
        public int DiscardedMenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public DateTime DiscardDate { get; set; }
    }
}
