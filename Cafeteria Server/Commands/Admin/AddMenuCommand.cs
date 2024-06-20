using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands.Admin
{
    public class AddMenuCommand : ICommand
    {
        private readonly IAdminService _adminService;

        public AddMenuCommand(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<string> Execute(string requestData)
        {
            var responseMessage = new ResponseMessage();

            try
            {
                var menuItemRequest = JsonConvert.DeserializeObject<MenuItemRequest>(requestData);
                responseMessage.IsSuccess = await _adminService.AddMenu(menuItemRequest);
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
