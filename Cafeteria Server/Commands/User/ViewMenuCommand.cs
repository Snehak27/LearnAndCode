using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public  class ViewMenuCommand : ICommand
    {
        private readonly IUserService _userService;

        public ViewMenuCommand(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<string> Execute(string requestData)
        {
            var response = new ViewMenuItemsResponse();

            try
            {
                var menuItems = await _userService.GetAllMenuItems();
                response.IsSuccess = true;
                response.MenuItems = menuItems.ToList(); 
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ex.ToString();
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}
