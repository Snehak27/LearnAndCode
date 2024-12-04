using System;

namespace CafeteriaServer.Commands
{
    public interface ICommand
    {
        Task<string> Execute(string requestData);
    }
}
