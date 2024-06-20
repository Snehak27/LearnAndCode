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
using CafeteriaServer;

public class Program
{
    private const int port = 8888;
    private static IServiceProvider serviceProvider;

    public static async Task Main(string[] args)
    {
        var serviceCollection = new ServiceCollection();
        ServiceConfigurator.ConfigureServices(serviceCollection);
        serviceProvider = serviceCollection.BuildServiceProvider();

        var userService = serviceProvider.GetService<IUserService>();
        var adminService = serviceProvider.GetService<IAdminService>();
        var employeeService = serviceProvider.GetService<IEmployeeService>();
        var chefService = serviceProvider.GetService<IChefService>();
        var notificationService = serviceProvider.GetService<INotificationService>();

        var dispatcher = new CommandDispatcher();

        dispatcher.RegisterCommand("login", new LoginCommand(userService, notificationService));
        dispatcher.RegisterCommand("addMenu", new AddMenuCommand(adminService));
        dispatcher.RegisterCommand("updateMenu", new UpdateMenuCommand(adminService));
        dispatcher.RegisterCommand("deleteMenu", new DeleteMenuCommand(adminService));
        dispatcher.RegisterCommand("viewMenu", new ViewMenuCommand(userService));
        dispatcher.RegisterCommand("feedback", new FeedbackCommand(employeeService));
        dispatcher.RegisterCommand("viewFeedback", new ViewFeedbackCommand(chefService));
        dispatcher.RegisterCommand("getRecommendations", new ChefRecommendationCommand(chefService));
        dispatcher.RegisterCommand("saveFinalMenu", new SaveFinalMenuCommand(chefService));
        dispatcher.RegisterCommand("viewMonthlyFeedbackReport", new MonthlyFeedbackReportCommand(chefService));
        dispatcher.RegisterCommand("getEmployeeRecommendations", new EmployeeRecommendationCommand(employeeService));
        dispatcher.RegisterCommand("saveEmployeeOrders", new SaveEmployeeOrdersCommand(employeeService));
        dispatcher.RegisterCommand("viewEmployeeOrders", new ViewEmployeeOrdersCommand(chefService));
        dispatcher.RegisterCommand("getPastOrders", new GetPastOrdersCommand(employeeService));

        ServerSocket server = new ServerSocket(8888, dispatcher);
        await server.Start();
    }
}
