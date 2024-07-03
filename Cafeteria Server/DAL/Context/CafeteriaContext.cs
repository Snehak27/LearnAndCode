using CafeteriaServer.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeteriaServer.Context
{
    public class CafeteriaContext : DbContext
    {
        public CafeteriaContext(DbContextOptions<CafeteriaContext> options)
        : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<MenuItem> MenuItem { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<MealType> MealType { get; set; }
        public DbSet<Order> EmployeeResponse { get; set; }
        public DbSet<NotificationType> NotificationType { get; set; }
        public DbSet<Recommendation> Recommendation { get; set;}
        public DbSet<RecommendedItem> RecommendedItem { get; set;}
        public DbSet<UserNotification> UserNotification { get; set; }
        public DbSet<OrderItem> EmployeeResponseItem { get; set; }
        public DbSet<FoodType> FoodTypes { get; set; }
        public DbSet<SpiceLevel> SpiceLevels { get; set; }
        public DbSet<CuisineType> CuisineTypes { get; set; }
        public DbSet<EmployeePreference> EmployeePreferences { get; set; }
        public DbSet<DetailedFeedback> DetailedFeedbacks { get; set; }
        public DbSet<DiscardedMenuItem> DiscardedMenuItem { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
