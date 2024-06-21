using CafeteriaClient.DTO;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient.Commands
{
    public class LogoutCommand : ICommand
    {
        private readonly Action _onLogout;
        private readonly Func<int> _getUserId;

        public LogoutCommand(Action onLogout, Func<int> getUserId)
        {
            _onLogout = onLogout;
            _getUserId = getUserId;
        }

        public async Task Execute(ClientSocket clientSocket)
        {
            var userId = _getUserId.Invoke();
            var logoutRequest = new RequestObject
            {
                CommandName = "logout",
                RequestData = JsonConvert.SerializeObject(new { UserId = userId })
            };

            string responseJson = await clientSocket.SendRequest(logoutRequest);
            var response = JsonConvert.DeserializeObject<AuthenticationResult>(responseJson);

            if (response.IsAuthenticated)
            {
                _onLogout.Invoke();
                Console.WriteLine("Logged out successfully");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Failed to log out: " + response.ErrorMessage);
            }
        }
    }
}
