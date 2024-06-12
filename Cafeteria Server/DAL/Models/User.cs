using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CafeteriaServer.DAL.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int RoleId {  get; set; }

        public Role Role { get; set; }
    }
}
