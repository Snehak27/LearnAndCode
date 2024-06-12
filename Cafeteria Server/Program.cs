using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CafeteriaServer.Context;
using CafeteriaServer.Repositories;
using CafeteriaServer.Service;
using CafeteriaServer.UnitofWork;
using Microsoft.Extensions.Configuration;
using CafeteriaServer.Server;
using CafeteriaServer.Manager;
using CafeteriaServer.Commands;

public class Program
{
    private const int port = 8888;
    private static IServiceProvider serviceProvider;

    public static async Task Main(string[] args)
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        serviceProvider = serviceCollection.BuildServiceProvider();

        var authenticationService = serviceProvider.GetService<IAuthenticationService>();
        var authenticationController = new AuthenticationController(authenticationService);
        var dispatcher = new CommandDispatcher();

        dispatcher.RegisterCommand("login", new LoginCommand(authenticationController));
        ServerSocket server = new ServerSocket(8888, dispatcher);
        await server.Start();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        services.AddDbContext<CafeteriaContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Configure services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
    }
}
