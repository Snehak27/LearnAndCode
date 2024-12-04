using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
using System;

namespace CafeteriaServer.Service
{
    public interface IAdminService
    {
        Task<bool> AddMenu(MenuItemRequest menuItemRequest);
        Task<bool> UpdateMenu(MenuItem menuItem);
        Task<bool> DeleteMenu(int id);
    }
}
