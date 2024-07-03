using System;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaServer.DAL.Models
{
    public class CuisineType
    {
        [Key]
        public int CuisineTypeId { get; set; }
        public string Cuisine { get; set; }
    }
}
