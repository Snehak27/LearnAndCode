using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class UpdateMenuCommand : ICommand
    {
        private readonly IAdminService _adminService;

        public UpdateMenuCommand(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<string> Execute(string requestData)
        {
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
                responseMessage.IsSuccess = false;
                responseMessage.ErrorMessage = ex.ToString();
            }

            return JsonConvert.SerializeObject(responseMessage);
        }
    }
}
