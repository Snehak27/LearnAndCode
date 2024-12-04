using System;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaServer.DAL.Models
{
    public class EmployeeResponse
    {
        [Key]
        public int EmployeeResponseId { get; set; }
        public int UserId { get; set; }
        public DateTime ResponseDate { get; set; }

        public virtual User User { get; set; }
    }
}
