using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO.ResponseModel;
using CafeteriaServer.DTO;
using CafeteriaServer.UnitofWork;
using System;

namespace CafeteriaServer.Service
{
    internal class SharedMenuService : ISharedMenuService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRecommendationService _recommendationService;
        private readonly INotificationService _notificationService;

        public SharedMenuService(IUnitOfWork unitOfWork, IRecommendationService recommendationService, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _recommendationService = recommendationService;
            _notificationService = notificationService;
        }

        public async Task<List<DiscardMenuItem>> GetDiscardMenuItems()
        {
            var allRecommendations = await _recommendationService.GetRecommendations();

            var filteredRecommendations = allRecommendations
                .SelectMany(r => r.Recommendations)
                .GroupBy(r => r.MenuItemId)
                .Select(g => new
                {
                    MenuItemId = g.Key,
                    MenuItemName = g.First().MenuItemName,
                    AverageRating = g.Average(r => r.AverageRating),
                    Comments = g.SelectMany(r => r.Comments).ToList()
                })
                .ToList();

            var discardItems = filteredRecommendations
                .Where(item => item.AverageRating < 2)
                .Select(item => new DiscardMenuItem
                {
                    MenuItemId = item.MenuItemId,
                    MenuItemName = item.MenuItemName,
                    AverageRating = item.AverageRating,
                    Sentiments = item.Comments
                })
                .ToList();

            return discardItems;
        }

        public async Task<bool> RemoveMenuItems(List<int> menuItemIds)
        {
            foreach (var menuItemId in menuItemIds)
            {
                var menuItem = await _unitOfWork.MenuItems.GetById(menuItemId);
                if (menuItem != null)
                {
                    //menuItem.IsDeleted = true;
                    //_unitOfWork.MenuItems.Update(menuItem);
                    await _notificationService.RemoveNotifications(menuItemId);
                    _unitOfWork.MenuItems.Delete(menuItem);

                    await AddDiscardedMenuItem(menuItem.ItemName, DateTime.Now);
                }
            }
            _unitOfWork.Save();
            return true;
        }

        private async Task AddDiscardedMenuItem(string menuItemName, DateTime discardDate)
        {
            var discardedMenuItem = new DiscardedMenuItem
            {
                MenuItemName = menuItemName,
                DiscardDate = discardDate
            };
            await _unitOfWork.DiscardedMenuItems.Add(discardedMenuItem);
            _unitOfWork.Save();
        }

        public async Task<bool> RequestDetailedFeedback(List<int> menuItemIds)
        {
            foreach (var menuItemId in menuItemIds)
            {
                var users = await _unitOfWork.Users.FindAll(u => u.RoleId == 3);
                var menuItem = await _unitOfWork.MenuItems.GetById(menuItemId);

                if (menuItem == null)
                {
                    return false;
                }

                await _notificationService.NotifyEmployees(notificationTypeId: 4, menuItemId);
            }
            return true;
        }

        public async Task<DateTime?> GetLastDiscardDate()
        {
            var logs = await _unitOfWork.DiscardedMenuItems.GetAll();
            var log = logs.OrderByDescending(l => l.DiscardDate).FirstOrDefault();
            return log?.DiscardDate;
        }

        public async Task<List<DetailedFeedback>> GetAllDetailedFeedbacks()
        {
            return (await _unitOfWork.DetailedFeedbacks.GetAll()).ToList();
        }
    }
}
