using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands.Admin
{
    public class DeleteMenuCommand : ICommand
    {
        private readonly IAdminService _adminService;

        public DeleteMenuCommand(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<string> Execute(string requestData)
        {
            var responseMessage = new ResponseMessage();

            try
            {
                var menuItemId = JsonConvert.DeserializeObject<int>(requestData);
                responseMessage.IsSuccess = await _adminService.DeleteMenu(menuItemId);
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
