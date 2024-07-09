using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CafeteriaServer.Server;
using CafeteriaServer;
using NLog;
using NLog.Web;

public class Program
{
    private const int port = 8888;
    private static IServiceProvider serviceProvider;

    public static async Task Main(string[] args)
    {
        var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        try
        {
            var serviceCollection = new ServiceCollection();
            ServiceConfigurator.ConfigureServices(serviceCollection);
            serviceProvider = serviceCollection.BuildServiceProvider();

            var dispatcher = new CommandDispatcher();
            var commandRegistrar = new CommandRegistrar(dispatcher, serviceProvider);
            commandRegistrar.RegisterCommands();
           
            ServerSocket server = new ServerSocket(port, dispatcher);
            await server.Start();
        }
        catch(Exception ex)
        {
            logger.Error(ex);
            throw new Exception(ex.Message);
        }
        finally
        {
            NLog.LogManager.Shutdown();
        }
    }
}
