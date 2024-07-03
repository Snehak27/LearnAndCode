using CafeteriaServer.DAL.Repositories;
using System;

namespace CafeteriaServer.UnitofWork
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IMenuItemRepository MenuItems { get; }
        IFeedbackRepository Feedbacks { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
        IRecommendationRepository Recommendations { get; }
        IRecommendedItemRepository RecommendedItems { get; }
        IUserNotificationRepository UserNotifications { get; }
        IMealTypeRepository MealTypes { get; }
        INotificationTypeRepository NotificationTypes { get; }
        IFoodPreferenceRepository FoodPreferences { get; }
        ISpiceLevelRepository SpiceLevels { get; }
        ICuisinePreferenceRepository CuisinePreferences { get; }
        IEmployeePreferenceRepository EmployeePreferences { get; }
        IDetailedFeedbackRepository DetailedFeedbacks { get; }
        IDiscardedMenuItemRepository DiscardedMenuItems { get; }

        void Save();
    }
}
