using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO.ResponseModel;
using System;

namespace CafeteriaServer.Service
{
    public interface ISharedMenuService
    {
        Task<List<DiscardMenuItem>> GetDiscardMenuItems();
        Task<bool> RemoveMenuItems(List<int> menuItemIds);
        Task<bool> RequestDetailedFeedback(List<int> menuItemIds);
        Task<DateTime?> GetLastDiscardDate();
        Task<List<DetailedFeedback>> GetAllDetailedFeedbacks();
    }
}
