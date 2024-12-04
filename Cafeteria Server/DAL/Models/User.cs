using System;
using System.ComponentModel.DataAnnotations;

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

        public virtual Role Role { get; set; }
    }
}
