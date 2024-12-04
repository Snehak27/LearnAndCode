using System;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaServer.DAL.Models
{
    public class EmployeePreference
    {
        [Key]
        public int EmployeePreferenceId { get; set; }
        public int UserId { get; set; }
        public int FoodTypeId { get; set; }
        public int SpiceLevelId { get; set; }
        public int CuisineTypeId { get; set; }
        public bool HasSweetTooth { get; set; }

        public virtual User User { get; set; }
        public virtual FoodType FoodType { get; set; }
        public virtual SpiceLevel SpiceLevel { get; set; }
        public virtual CuisineType CuisineType { get; set; }
    }
}
