using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class GetAllMenuItemsCommand : ICommand
    {
        private readonly IUserService _userService;
        private readonly ILogger<GetAllMenuItemsCommand> _logger;

        public GetAllMenuItemsCommand(IUserService userService, ILogger<GetAllMenuItemsCommand> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            _logger.LogInformation("GetAllMenuItems endpoint invoked");

            var response = new ViewMenuItemsResponse();

            try
            {
                var menuItems = await _userService.GetAllMenuItems();
                response.IsSuccess = true;
                response.MenuItems = menuItems.ToList(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                response.IsSuccess = false;
                response.ErrorMessage = ex.ToString();
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}
