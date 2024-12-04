using System;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaServer.DAL.Models
{
    public class SpiceLevel
    {
        [Key]
        public int SpiceLevelId { get; set; }
        public string Level { get; set; }
    }
}
