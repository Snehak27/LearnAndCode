using System;

namespace CafeteriaClient.Commands
{
    public interface ICommand
    {
        Task Execute(ClientSocket clientSocket);
    }
}
