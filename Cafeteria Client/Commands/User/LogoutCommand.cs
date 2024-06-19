using CafeteriaClient.DTO;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient.Commands
{
    public class LogoutCommand : ICommand
    {
        private readonly Action _onLogout;

        public LogoutCommand(Action onLogout)
        {
            _onLogout = onLogout;
        }

        public async Task Execute(ClientSocket clientSocket)
        {
            _onLogout.Invoke();
            Console.WriteLine("Logged out successfully");
            Console.WriteLine();
        }
    }
}
