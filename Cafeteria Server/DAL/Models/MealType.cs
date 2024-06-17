using System;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaServer.DAL.Models
{
    public class MealType
    {
        [Key]
        public int MealTypeId { get; set; }
        public string MealTypeName { get; set; }
    }
}
