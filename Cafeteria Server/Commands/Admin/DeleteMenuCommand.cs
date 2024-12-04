using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands.Admin
{
    public class DeleteMenuCommand : ICommand
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<DeleteMenuCommand> _logger;

        public DeleteMenuCommand(IAdminService adminService, ILogger<DeleteMenuCommand> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            _logger.LogInformation("Delete menu endpoint invoked");

            var responseMessage = new ResponseMessage();

            try
            {
                var menuItemId = JsonConvert.DeserializeObject<int>(requestData);
                responseMessage.IsSuccess = await _adminService.DeleteMenu(menuItemId);
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
