using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeteriaServer.DTO
{
    public class MenuItemRequest
    {
        public string ItemName { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public bool AvailabilityStatus { get; set; }
    }
}
