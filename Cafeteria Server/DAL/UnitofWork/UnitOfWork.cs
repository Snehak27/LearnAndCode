using CafeteriaServer.Context;
using CafeteriaServer.DAL.Models;
using CafeteriaServer.DAL.Repositories;
using CafeteriaServer.Repositories;
using System;

namespace CafeteriaServer.UnitofWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CafeteriaContext _context;

        public UnitOfWork(CafeteriaContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            MenuItems = new MenuItemRepository(_context);
            Feedbacks = new FeedbackRepository(_context);
            Orders = new OrderRepository(_context);
            OrderItems = new OrderItemRepository(_context);
            Recommendations = new RecommendationRepository(_context);
            RecommendedItems = new RecommendedItemRepository(_context);
            UserNotifications = new UserNotificationRepository(_context);
            MealTypes = new MealTypeRepository(_context);
            NotificationTypes = new NotificationTypeRepository(_context);
            FoodPreferences = new FoodPreferenceRepository(_context);
            SpiceLevels = new SpiceLevelRepository(_context);
            CuisinePreferences = new CuisinePreferenceRepository(_context);
            EmployeePreferences = new EmployeePreferenceRepository(_context);
            DetailedFeedbacks = new DetailedFeedbackRepository(_context);
            DiscardedMenuItems = new DiscardMenuItemLogRepository(_context);
        }

        public IUserRepository Users { get; }
        public IMenuItemRepository MenuItems { get; }
        public IFeedbackRepository Feedbacks { get; }
        public IOrderRepository Orders { get; }
        public IOrderItemRepository OrderItems { get; }
        public IRecommendationRepository Recommendations { get; }
        public IRecommendedItemRepository RecommendedItems { get; }
        public IUserNotificationRepository UserNotifications { get; }
        public IMealTypeRepository MealTypes { get; }
        public INotificationTypeRepository NotificationTypes { get; }
        public IFoodPreferenceRepository FoodPreferences { get;  }
        public ISpiceLevelRepository SpiceLevels { get; }
        public ICuisinePreferenceRepository CuisinePreferences { get; }
        public IEmployeePreferenceRepository EmployeePreferences { get; }
        public IDetailedFeedbackRepository DetailedFeedbacks { get; }
        public IDiscardedMenuItemRepository DiscardedMenuItems { get; }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
