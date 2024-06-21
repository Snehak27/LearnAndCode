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
using NLog;
using NLog.Web;
using Microsoft.Extensions.Logging;

public class Program
{
    private const int port = 8888;
    private static IServiceProvider serviceProvider;

    public static async Task Main(string[] args)
    {
        var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        try
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

            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            dispatcher.RegisterCommand("login", new LoginCommand(userService, notificationService, loggerFactory.CreateLogger<LoginCommand>()));
            dispatcher.RegisterCommand("logout", new LogoutCommand(userService,loggerFactory.CreateLogger<LogoutCommand>()));
            dispatcher.RegisterCommand("addMenu", new AddMenuCommand(adminService, loggerFactory.CreateLogger<AddMenuCommand>()));
            dispatcher.RegisterCommand("updateMenu", new UpdateMenuCommand(adminService, loggerFactory.CreateLogger<UpdateMenuCommand>()));
            dispatcher.RegisterCommand("deleteMenu", new DeleteMenuCommand(adminService, loggerFactory.CreateLogger<DeleteMenuCommand>()));
            dispatcher.RegisterCommand("getAllMenuItems", new GetAllMenuItemsCommand(userService, loggerFactory.CreateLogger<GetAllMenuItemsCommand>()));
            dispatcher.RegisterCommand("submitFeedback", new SubmitFeedbackCommand(employeeService, loggerFactory.CreateLogger<SubmitFeedbackCommand>()));
            dispatcher.RegisterCommand("getFeedbacks", new GetFeedbacksCommand(chefService, loggerFactory.CreateLogger<GetFeedbacksCommand>()));
            dispatcher.RegisterCommand("getRecommendations", new GetChefRecommendationsCommand(chefService, loggerFactory.CreateLogger<GetChefRecommendationsCommand>()));
            dispatcher.RegisterCommand("saveFinalMenu", new SaveFinalMenuCommand(chefService, loggerFactory.CreateLogger<SaveFinalMenuCommand>()));
            dispatcher.RegisterCommand("getFeedbackReport", new GetFeedbackReportCommand(chefService, loggerFactory.CreateLogger<GetFeedbackReportCommand>()));
            dispatcher.RegisterCommand("getEmployeeRecommendations", new GetEmployeeRecommendationsCommand(employeeService, loggerFactory.CreateLogger<GetEmployeeRecommendationsCommand>()));
            dispatcher.RegisterCommand("saveEmployeeOrders", new SaveEmployeeOrdersCommand(employeeService, loggerFactory.CreateLogger<SaveEmployeeOrdersCommand>()));
            dispatcher.RegisterCommand("getEmployeeOrders", new GetEmployeeOrdersCommand(chefService, loggerFactory.CreateLogger<GetEmployeeOrdersCommand>()));
            dispatcher.RegisterCommand("getPastOrders", new GetPastOrdersCommand(employeeService, loggerFactory.CreateLogger<GetPastOrdersCommand>()));

            ServerSocket server = new ServerSocket(8888, dispatcher);
            await server.Start();
        }
        catch(Exception ex)
        {
            logger.Error(ex);
            throw new Exception(ex.Message);
        }
        finally
        {
            NLog.LogManager.Shutdown();
        }
    }
}
