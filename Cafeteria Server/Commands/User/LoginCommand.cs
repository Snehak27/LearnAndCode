using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class LoginCommand : ICommand
    {
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;

        public LoginCommand(IUserService userService, INotificationService notificationService)
        {
            _userService = userService;
            _notificationService = notificationService;
        }

        public async Task<string> Execute(string requestData)
        {
            var response = new AuthenticationResponse();
            try
            {
                var request = JsonConvert.DeserializeObject<AuthenticationRequest>(requestData);
                var result = await _userService.AuthenticateUser(request);
                response.IsAuthenticated = result.IsAuthenticated;
                response.User = result.User;
                response.ErrorMessage = result.ErrorMessage;

                if (response.IsAuthenticated)
                {
                    var notifications = await _notificationService.GetUnreadNotifications(result.User.UserId);
                    response.Notifications = notifications.ToList();
                }
            }
            catch (Exception ex)
            {
                response.IsAuthenticated = false;
                response.ErrorMessage = ex.Message;
            }
            return JsonConvert.SerializeObject(response);
        }
    }
}
