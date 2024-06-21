using CafeteriaServer.DTO;
using CafeteriaServer.DTO.RequestModel;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class LogoutCommand : ICommand
    {
        private readonly IUserService _userService;
        private readonly ILogger<LogoutCommand> _logger;

        public LogoutCommand(IUserService userService, ILogger<LogoutCommand> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            _logger.LogInformation("Logout endpoint invoked");

            var response = new AuthenticationResponse();
            try
            {
                var request = JsonConvert.DeserializeObject<LogoutRequest>(requestData);
                var result = await _userService.GetUserById(request.UserId);
                response.IsAuthenticated = true;
                response.User = result;

                _logger.LogInformation("User {Name} with EmployeeId: {EmployeeId} logged out successfully.",result.Name, result.EmployeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user logout");
                response.IsAuthenticated = false;
                response.ErrorMessage = ex.Message;
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}
