using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class LoginCommand : ICommand
    {
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<LoginCommand> _logger;

        public LoginCommand(IUserService userService, INotificationService notificationService, ILogger<LoginCommand> logger)
        {
            _userService = userService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            var response = new AuthenticationResponse();
            try
            {
                _logger.LogInformation("Login endpoint invoked");

                var request = JsonConvert.DeserializeObject<AuthenticationRequest>(requestData);
                var result = await _userService.AuthenticateUser(request);
                response.IsAuthenticated = result.IsAuthenticated;
                response.User = result.User;
                response.ErrorMessage = result.ErrorMessage;

                if (response.IsAuthenticated)
                {
                    _logger.LogInformation("User {UserName} with EmployeeId: {EmployeeID} successfully logged in.", response.User.Name, request.EmployeeId);
                    var notifications = await _notificationService.GetUnreadNotifications(result.User.UserId);
                    response.Notifications = notifications.ToList();
                }
                else
                {
                    _logger.LogWarning("Failed authentication attempt for User: {EmployeeId}, Reason: {Reason}", request.EmployeeId, response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user authentication");
                response.IsAuthenticated = false;
                response.ErrorMessage = ex.Message;
            }
            return JsonConvert.SerializeObject(response);
        }
    }
}
