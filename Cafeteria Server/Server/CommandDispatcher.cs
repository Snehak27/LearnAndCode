using CafeteriaServer.Commands;

namespace CafeteriaServer.Server
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

        public async Task<string> Dispatch(string commandName, string requestData)
        {
            if (_commands.TryGetValue(commandName.ToLower(), out var command))
            {
                return await command.Execute(requestData);
            }
            else
            {
                throw new ArgumentException("Invalid command.");
            }
        }
    }
}
