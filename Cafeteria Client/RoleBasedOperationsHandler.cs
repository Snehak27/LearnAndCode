using System;

namespace CafeteriaClient
{
    public class RoleBasedOperationsHandler
    {
        private readonly CommandDispatcher _commandDispatcher;
        private readonly CommandRegistrar _commandRegistrar;
        private readonly UserSessionManager _userSessionManager;
        private readonly ClientSocket _clientSocket;

        public RoleBasedOperationsHandler(CommandDispatcher commandDispatcher, CommandRegistrar commandRegistrar, UserSessionManager userSessionManager, ClientSocket clientSocket)
        {
            _commandDispatcher = commandDispatcher;
            _commandRegistrar = commandRegistrar;
            _userSessionManager = userSessionManager;
            _clientSocket = clientSocket;
        }

        public async Task DisplayOperations()
        {
            _commandRegistrar.RegisterCommands(_userSessionManager.UserRoleId, _commandDispatcher, _userSessionManager.GetUserId);

            bool exit = false;
            while (!exit)
            {
                DisplayRoleBasedOperations(_userSessionManager.UserRoleId);

                Console.WriteLine("Enter your choice: ");
                string commandKey = Console.ReadLine();

                if (commandKey.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    exit = true;
                }
                else if ((_userSessionManager.UserRoleId == 1 && commandKey == "6") || (_userSessionManager.UserRoleId == 2 && commandKey == "7") || (_userSessionManager.UserRoleId == 3 && commandKey == "6"))
                {
                    exit = true;
                    await _commandDispatcher.Dispatch("logout", _clientSocket);
                }
                else
                {
                    await _commandDispatcher.Dispatch(commandKey, _clientSocket);
                }
            }
        }

        private void DisplayRoleBasedOperations(int roleId)
        {
            switch (roleId)
            {
                case 1:
                    Console.WriteLine("\nAdmin operations:");
                    Console.WriteLine("1) Add menu");
                    Console.WriteLine("2) Update menu");
                    Console.WriteLine("3) Delete menu");
                    Console.WriteLine("4) View Menu");
                    Console.WriteLine("5) View discard Menu list");
                    Console.WriteLine("6) Logout");
                    break;

                case 2:
                    Console.WriteLine("\nChef operations:");
                    Console.WriteLine("1) View Menu");
                    Console.WriteLine("2) View Employee Feedback");
                    Console.WriteLine("3) View Monthly Feedback report");
                    Console.WriteLine("4) Roll out menu for next day");
                    Console.WriteLine("5) View Employee Orders");
                    Console.WriteLine("6) View discard Menu list");
                    Console.WriteLine("7) Logout");
                    break;

                case 3:
                    Console.WriteLine("\nEmployee operations:");
                    Console.WriteLine("1) View Profile");
                    Console.WriteLine("2) View Menu");
                    Console.WriteLine("3) Give Feedback");
                    Console.WriteLine("4) View Recommendations for next day and order");
                    Console.WriteLine("5) Provide detailed feedback for discard menu items");
                    Console.WriteLine("6) Logout");
                    break;

                default:
                    Console.WriteLine("Invalid role");
                    break;
            }
        }
    }
}
