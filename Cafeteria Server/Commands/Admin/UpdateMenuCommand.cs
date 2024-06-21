using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class UpdateMenuCommand : ICommand
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<UpdateMenuCommand> _logger;


        public UpdateMenuCommand(IAdminService adminService, ILogger<UpdateMenuCommand> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            _logger.LogInformation("Update menu endpoint invoked");

            var responseMessage = new ResponseMessage();

            try
            {
                var menuItemRequest = JsonConvert.DeserializeObject<MenuItem>(requestData);
                bool updateSuccess = await _adminService.UpdateMenu(menuItemRequest);
                responseMessage.IsSuccess = updateSuccess;
                responseMessage.ErrorMessage = updateSuccess ? null : "Failed to update the menu item.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                responseMessage.IsSuccess = false;
                responseMessage.ErrorMessage = ex.ToString();
            }

            return JsonConvert.SerializeObject(responseMessage);
        }
    }
}
