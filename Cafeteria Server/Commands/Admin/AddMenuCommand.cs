using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands.Admin
{
    public class AddMenuCommand : ICommand
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AddMenuCommand> _logger;

        public AddMenuCommand(IAdminService adminService, ILogger<AddMenuCommand> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            _logger.LogInformation("Add menu endpoint invoked");

            var responseMessage = new ResponseMessage();

            try
            {
                var menuItemRequest = JsonConvert.DeserializeObject<MenuItemRequest>(requestData);
                responseMessage.IsSuccess = await _adminService.AddMenu(menuItemRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred:", ex);

                responseMessage.IsSuccess = false;
                responseMessage.ErrorMessage = ex.ToString();
            }

            return JsonConvert.SerializeObject(responseMessage);
        }
    }
}
