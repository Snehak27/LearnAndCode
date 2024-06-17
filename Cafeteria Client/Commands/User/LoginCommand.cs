using CafeteriaClient.Commands;
using CafeteriaClient.DTO;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient
{
    public class LoginCommand : ICommand
    {
        private readonly Func<int, Task> _onLoginSuccess;

        public LoginCommand(Func<int, Task> onLoginSuccess)
        {
            _onLoginSuccess = onLoginSuccess;
        }

        public async Task Execute(ClientSocket clientSocket)
        {
            Console.WriteLine("Enter your credentials to login!!");
            Console.WriteLine("Enter employee ID: ");
            string employeeId = Console.ReadLine();

            Console.WriteLine("Enter password: ");
            string password = Console.ReadLine();

            var authenticationRequest = new AuthenticationRequest
            {
                EmployeeId = employeeId,
                Password = password
            };

            string authenticationRequestJson = JsonConvert.SerializeObject(authenticationRequest);
            var request = new RequestObject
            {
                CommandName = "login",
                RequestData = authenticationRequestJson
            };

            string jsonResponse = await clientSocket.SendRequest(request);
            var response = JsonConvert.DeserializeObject<AuthenticationResult>(jsonResponse);

            if (response.IsAuthenticated)
            {
                Console.WriteLine("Authentication successful.");
                await _onLoginSuccess(response.User.UserId);
            }
            else
            {
                Console.WriteLine("Authentication failed: " + response.ErrorMessage);
            }
        }
    }
}
