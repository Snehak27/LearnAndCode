﻿using System;

namespace CafeteriaClient.DTO.Request
{
    public class DiscardActionRequest
    {
        public string Action { get; set; }
        //public int MenuItemId { get; set; }
        public List<int> MenuItemIds { get; set; }
    }
}
