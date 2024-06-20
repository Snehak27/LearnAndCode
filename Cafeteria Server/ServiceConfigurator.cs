using CafeteriaServer.Context;
using CafeteriaServer.DAL.Repositories;
using CafeteriaServer.Service;
using CafeteriaServer.UnitofWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CafeteriaServer
{
    public static class ServiceConfigurator
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddDbContext<CafeteriaContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IMenuItemRepository, MenuItemRepository>();
            services.AddScoped<IRecommendationRepository, RecommendationRepository>();
            services.AddScoped<IRecommendedItemRepository, RecommendedItemRepository>();
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Configure services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IChefService, ChefService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IRecommendationService, RecommendationService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ISentimentAnalyzer, SentimentAnalyzer>();
        }
    }
}
