using System;
using CafeteriaClient.Commands;
using ICommand = CafeteriaClient.Commands.ICommand;

namespace CafeteriaClient
{
    public class CommandDispatcher
    {
        private readonly Dictionary<string, ICommand> _commands;

        public CommandDispatcher()
        {
            _commands = new Dictionary<string, ICommand>();
        }

        public void RegisterCommand(string commandName, ICommand command)
        {
            _commands[commandName.ToLower()] = command;
        }

        public async Task Dispatch(string commandName, ClientSocket clientSocket)
        {
            if (_commands.TryGetValue(commandName.ToLower(), out var command))
            {
               await command.Execute(clientSocket);
            }
            else
            {
                Console.WriteLine("Invalid command.");
            }
        }
    }
}
