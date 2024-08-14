using CafeteriaClient;
using CafeteriaClient.Commands;
using System;

public class Program
{
    private const int port = 8888;

    public static async Task Main(string[] args)
    {
        var userSessionManager = new UserSessionManager();

        while (true)
        {
            var clientSocket = new ClientSocket("192.168.4.244", port);
            var dispatcher = new CommandDispatcher();
            var commandRegistrar = new CommandRegistrar();
            var roleBasedOperationsHandler = new RoleBasedOperationsHandler(dispatcher, commandRegistrar, userSessionManager, clientSocket);

            dispatcher.RegisterCommand("login", new LoginCommand(async (id, roleId) =>
            {
                userSessionManager.SetUser(id, roleId);
                await roleBasedOperationsHandler.DisplayOperations();
            }));

            dispatcher.RegisterCommand("logout", new LogoutCommand(userSessionManager.ClearUser, userSessionManager.GetUserId));

            await dispatcher.Dispatch("login", clientSocket);
        }
    }
}
