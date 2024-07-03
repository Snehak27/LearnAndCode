﻿using System;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaClient.DTO
{
    public class MenuItem
    {
        [Key]
        public int MenuItemId { get; set; }
        public string ItemName { get; set; }
        public double Price { get; set; }
        public bool AvailabilityStatus { get; set; }
        public int FoodTypeId { get; set; }
        public int SpiceLevelId { get; set; }
        public int CuisineTypeId { get; set; }
        public bool IsSweet { get; set; }
    }
}
