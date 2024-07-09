using System;
using CafeteriaServer.DTO;
using CafeteriaServer.DAL.Models;
using CafeteriaServer.UnitofWork;

namespace CafeteriaServer.Service
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public AdminService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<bool> AddMenu(MenuItemRequest menuItemRequest)
        {
            var item = new MenuItem
            {
                ItemName = menuItemRequest.ItemName,
                Price = menuItemRequest.Price,
                AvailabilityStatus = menuItemRequest.AvailabilityStatus,
                FoodTypeId = menuItemRequest.FoodTypeId,
                SpiceLevelId = menuItemRequest.SpiceLevelId,
                CuisineTypeId = menuItemRequest.CuisineTypeId,
                IsSweet = menuItemRequest.IsSweet,
            };

            await _unitOfWork.MenuItems.Add(item);
            _unitOfWork.Save();

            await _notificationService.NotifyEmployees(notificationTypeId: 1, item.MenuItemId);

            return true;
        }

        public async Task<bool> UpdateMenu(MenuItem menuItem)
        {
            bool notify = false;
            var item = await _unitOfWork.MenuItems.GetById(menuItem.MenuItemId);
            if (item == null)
            {
                throw new KeyNotFoundException("Menu item not found.");
            }

            if(item.AvailabilityStatus == false && menuItem.AvailabilityStatus == true)
            {
                notify = true;
            }

            item.ItemName = menuItem.ItemName;
            item.Price = menuItem.Price;
            item.AvailabilityStatus = menuItem.AvailabilityStatus;
            item.FoodTypeId = menuItem.FoodTypeId;
            item.CuisineTypeId = menuItem.CuisineTypeId;
            item.SpiceLevelId = menuItem.SpiceLevelId;
            item.IsSweet = menuItem.IsSweet;

            _unitOfWork.MenuItems.Update(item);
            _unitOfWork.Save();

            if(notify)
            {
                await _notificationService.NotifyEmployees(notificationTypeId:2, item.MenuItemId);
            }

            return true;
        }

        public async Task<bool> DeleteMenu(int id)
        {
            var item = await _unitOfWork.MenuItems.GetById(id);
            if (item == null)
            {
                throw new KeyNotFoundException("Menu item not found.");
            }

            _unitOfWork.MenuItems.Delete(item);
            _unitOfWork.Save();

            return true;
        }
    }
}
