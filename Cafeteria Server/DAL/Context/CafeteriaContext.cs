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
        public DbSet<EmployeeResponse> EmployeeResponse { get; set; }
        public DbSet<NotificationType> NotificationType { get; set; }
        public DbSet<Recommendation> Recommendation { get; set;}
        public DbSet<RecommendedItem> RecommendedItem { get; set;}
        public DbSet<UserNotification> UserNotification { get; set; }
        public DbSet<EmployeeResponseItem> EmployeeResponseItem { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
