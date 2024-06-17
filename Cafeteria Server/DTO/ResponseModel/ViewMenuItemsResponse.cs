using CafeteriaServer.DAL.Models;
using System;

namespace CafeteriaServer.DTO.ResponseModel
{
    public class ViewMenuItemsResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<MenuItem> MenuItems { get; set; }
    }
}
