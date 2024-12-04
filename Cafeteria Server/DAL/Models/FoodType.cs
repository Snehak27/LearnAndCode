using System;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaServer.DAL.Models
{
    public class FoodType
    {
        [Key]
        public int FoodTypeId { get; set; }
        public string FoodTypeName { get; set; }
    }
}
