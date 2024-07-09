using CafeteriaServer.Commands.Admin;
using CafeteriaServer.Commands.Chef;
using CafeteriaServer.Commands.Employee;
using CafeteriaServer.Commands;
using CafeteriaServer.Server;
using CafeteriaServer.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;

namespace CafeteriaServer
{
    public class CommandRegistrar
    {
        private readonly CommandDispatcher _dispatcher;
        private readonly IServiceProvider _serviceProvider;

        public CommandRegistrar(CommandDispatcher dispatcher, IServiceProvider serviceProvider)
        {
            _dispatcher = dispatcher;
            _serviceProvider = serviceProvider;
        }

        public void RegisterCommands()
        {
            var userService = _serviceProvider.GetService<IUserService>();
            var adminService = _serviceProvider.GetService<IAdminService>();
            var employeeService = _serviceProvider.GetService<IEmployeeService>();
            var chefService = _serviceProvider.GetService<IChefService>();
            var notificationService = _serviceProvider.GetService<INotificationService>();
            var sharedMenuService = _serviceProvider.GetService<ISharedMenuService>();
            var loggerFactory = _serviceProvider.GetService<ILoggerFactory>();

            _dispatcher.RegisterCommand("login", new LoginCommand(userService, notificationService, loggerFactory.CreateLogger<LoginCommand>()));
            _dispatcher.RegisterCommand("logout", new LogoutCommand(userService, loggerFactory.CreateLogger<LogoutCommand>()));
            _dispatcher.RegisterCommand("addMenu", new AddMenuCommand(adminService, loggerFactory.CreateLogger<AddMenuCommand>()));
            _dispatcher.RegisterCommand("updateMenu", new UpdateMenuCommand(adminService, loggerFactory.CreateLogger<UpdateMenuCommand>()));
            _dispatcher.RegisterCommand("deleteMenu", new DeleteMenuCommand(adminService, loggerFactory.CreateLogger<DeleteMenuCommand>()));
            _dispatcher.RegisterCommand("getAllMenuItems", new GetAllMenuItemsCommand(userService, loggerFactory.CreateLogger<GetAllMenuItemsCommand>()));
            _dispatcher.RegisterCommand("submitFeedback", new SubmitFeedbackCommand(employeeService, loggerFactory.CreateLogger<SubmitFeedbackCommand>()));
            _dispatcher.RegisterCommand("getAllFeedbacks", new GetAllFeedbacksCommand(chefService, loggerFactory.CreateLogger<GetAllFeedbacksCommand>()));
            _dispatcher.RegisterCommand("getChefRecommendations", new GetChefRecommendationsCommand(chefService, loggerFactory.CreateLogger<GetChefRecommendationsCommand>()));
            _dispatcher.RegisterCommand("saveFinalMenu", new SaveFinalMenuCommand(chefService, loggerFactory.CreateLogger<SaveFinalMenuCommand>()));
            _dispatcher.RegisterCommand("getFeedbackReport", new GetFeedbackReportCommand(chefService, loggerFactory.CreateLogger<GetFeedbackReportCommand>()));
            _dispatcher.RegisterCommand("getEmployeeRecommendations", new GetEmployeeRecommendationsCommand(employeeService, loggerFactory.CreateLogger<GetEmployeeRecommendationsCommand>()));
            _dispatcher.RegisterCommand("saveEmployeeOrders", new SaveEmployeeOrdersCommand(employeeService, loggerFactory.CreateLogger<SaveEmployeeOrdersCommand>()));
            _dispatcher.RegisterCommand("getEmployeeOrders", new GetEmployeeOrdersCommand(chefService, loggerFactory.CreateLogger<GetEmployeeOrdersCommand>()));
            _dispatcher.RegisterCommand("getPastOrders", new GetPastOrdersCommand(employeeService, loggerFactory.CreateLogger<GetPastOrdersCommand>()));
            _dispatcher.RegisterCommand("getDiscardMenuList", new GetDiscardMenuListCommand(sharedMenuService, loggerFactory.CreateLogger<GetDiscardMenuListCommand>()));
            _dispatcher.RegisterCommand("handleDiscardActions", new HandleDiscardActionsCommand(sharedMenuService, loggerFactory.CreateLogger<HandleDiscardActionsCommand>()));
            _dispatcher.RegisterCommand("updateProfile", new UpdateProfileCommand(employeeService));
            _dispatcher.RegisterCommand("getEmployeePreference", new GetEmployeePreferenceCommand(employeeService, loggerFactory.CreateLogger<GetEmployeePreferenceCommand>()));
            _dispatcher.RegisterCommand("getPendingFeedbackMenuItems", new GetPendingFeedbackMenuItemsCommand(employeeService, loggerFactory.CreateLogger<GetPendingFeedbackMenuItemsCommand>()));
            _dispatcher.RegisterCommand("submitDetailedFeedback", new SubmitDetailedFeedbackCommand(employeeService, loggerFactory.CreateLogger<SubmitDetailedFeedbackCommand>()));
            _dispatcher.RegisterCommand("getAllDetailedFeedbacks", new GetAllDetailedFeedbacksCommand(sharedMenuService, loggerFactory.CreateLogger<GetAllDetailedFeedbacksCommand>()));
        }
    }
}
