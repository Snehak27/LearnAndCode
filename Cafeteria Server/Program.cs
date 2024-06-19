using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CafeteriaServer.Context;
using CafeteriaServer.Service;
using CafeteriaServer.UnitofWork;
using Microsoft.Extensions.Configuration;
using CafeteriaServer.Server;
using CafeteriaServer.Commands;
using CafeteriaServer.DAL.Repositories;
using CafeteriaServer.Commands.Admin;
using CafeteriaServer.Commands.Chef;

public class Program
{
    private const int port = 8888;
    private static IServiceProvider serviceProvider;

    public static async Task Main(string[] args)
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        serviceProvider = serviceCollection.BuildServiceProvider();

        var userService = serviceProvider.GetService<IUserService>();
        var adminService = serviceProvider.GetService<IAdminService>();
        var employeeService = serviceProvider.GetService<IEmployeeService>();
        var chefService = serviceProvider.GetService<IChefService>();
        var recommendationService = serviceProvider.GetService<IRecommendationService>();
        var notificationService = serviceProvider.GetService<INotificationService>();

        var dispatcher = new CommandDispatcher();

        dispatcher.RegisterCommand("login", new LoginCommand(userService, notificationService));
        dispatcher.RegisterCommand("addMenu", new AddMenuCommand(adminService));
        dispatcher.RegisterCommand("updateMenu", new UpdateMenuCommand(adminService));
        dispatcher.RegisterCommand("deleteMenu", new DeleteMenuCommand(adminService));
        dispatcher.RegisterCommand("viewMenu", new ViewMenuCommand(userService));
        dispatcher.RegisterCommand("feedback", new FeedbackCommand(employeeService));
        dispatcher.RegisterCommand("viewFeedback", new ViewFeedbackCommand(chefService));
        dispatcher.RegisterCommand("getRecommendations", new RecommendationCommand(chefService));
        dispatcher.RegisterCommand("saveFinalMenu", new SaveFinalMenuCommand(chefService));
        dispatcher.RegisterCommand("viewMonthlyFeedbackReport", new MonthlyFeedbackReportCommand(chefService));
        dispatcher.RegisterCommand("getEmployeeRecommendations", new EmployeeRecommendationCommand(employeeService));
        dispatcher.RegisterCommand("saveEmployeeResponse", new SaveEmployeeResponseCommand(employeeService));
        dispatcher.RegisterCommand("viewEmployeeResponses", new ViewEmployeeResponsesCommand(chefService));
        dispatcher.RegisterCommand("getPastOrders", new GetPastOrdersCommand(employeeService));

        ServerSocket server = new ServerSocket(8888, dispatcher);
        await server.Start();
    }

    private static void ConfigureServices(IServiceCollection services)
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
    }
}
