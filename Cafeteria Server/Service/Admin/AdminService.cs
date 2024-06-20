using System;
using CafeteriaServer.DTO;
using CafeteriaServer.DAL.Models;
using CafeteriaServer.UnitofWork;

namespace CafeteriaServer.Service
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddMenu(MenuItemRequest menuItemRequest)
        {
            var item = new MenuItem
            {
                ItemName = menuItemRequest.ItemName,
                Price = menuItemRequest.Price,
                AvailabilityStatus = menuItemRequest.AvailabilityStatus,
            };

            await _unitOfWork.MenuItems.Add(item);
            _unitOfWork.Save();

            var employees = await _unitOfWork.Users.FindAll(u => u.RoleId == 3); 
            foreach (var employee in employees)
            {
                var notification = new UserNotification
                {
                    UserId = employee.UserId,
                    NotificationTypeId = 1, 
                    MenuItemId = item.MenuItemId,
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };
                await _unitOfWork.UserNotifications.Add(notification);
            }
            _unitOfWork.Save();

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

            _unitOfWork.MenuItems.Update(item);
            _unitOfWork.Save();

            if(notify)
            {
                var employees = await _unitOfWork.Users.FindAll(u => u.RoleId == 3);
                foreach (var employee in employees)
                {
                    var notification = new UserNotification
                    {
                        UserId = employee.UserId,
                        NotificationTypeId = 2,
                        MenuItemId = item.MenuItemId,
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };
                    await _unitOfWork.UserNotifications.Add(notification);
                }
                _unitOfWork.Save();
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
