using CafeteriaServer.DTO;
using System;

namespace CafeteriaServer.Service
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationResponse>> GetUnreadNotifications(int userId);
        Task NotifyEmployees(int notificationTypeId, int? menuItemId = null);
    }
}
