using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class LoginCommand : ICommand
    {
        private readonly IUserService _userService;

        public LoginCommand(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<string> Execute(string requestData)
        {
            var request = JsonConvert.DeserializeObject<AuthenticationRequest>(requestData);
            var result = await _userService.AuthenticateUser(request);
            return JsonConvert.SerializeObject(result);
        }
    }
}
