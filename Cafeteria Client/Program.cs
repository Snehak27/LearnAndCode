﻿using CafeteriaClient;
using CafeteriaClient.Commands;
using CafeteriaClient.Commands.Admin;
using CafeteriaClient.Commands.Chef;
using CafeteriaClient.Commands.Employee;
using System;

public class Program
{
    //private const string server = "192.168.1.3";
    private const int port = 8888;
    private static int userId;
    private static int userRoleId;

    public static async Task Main(string[] args)
    {
        while (true)
        {
            var clientSocket = new ClientSocket("192.168.4.224", port);
            var _dispatcher = new CommandDispatcher();

            _dispatcher.RegisterCommand("login", new LoginCommand(async (id, roleId) =>
            {
                userId = id;
                userRoleId = roleId;
                await DisplayOperations(userRoleId, _dispatcher, clientSocket);
            }));

            _dispatcher.RegisterCommand("logout", new LogoutCommand(ClearUserId, () => userId));

            await _dispatcher.Dispatch("login", clientSocket);
        }
    }

    public static void ClearUserId()
    {
        userId = 0;
    }

    static async Task DisplayOperations(int roleId, CommandDispatcher dispatcher, ClientSocket clientSocket)
    {
        switch (roleId)
        {
            case 1:
                // Register admin commands
                dispatcher.RegisterCommand("1", new AddMenuCommand());
                dispatcher.RegisterCommand("2", new UpdateMenuCommand());
                dispatcher.RegisterCommand("3", new DeleteMenuCommand());
                dispatcher.RegisterCommand("4", new ViewMenuCommand());
                break;

            case 2:
                //Register chef commands;
                dispatcher.RegisterCommand("1", new ViewMenuCommand());
                dispatcher.RegisterCommand("2", new ViewFeedbackCommand());
                dispatcher.RegisterCommand("3", new ViewMonthlyFeedbackReportCommand());
                dispatcher.RegisterCommand("4", new RolloutMenuCommand());
                dispatcher.RegisterCommand("5", new ViewEmployeeOrdersCommand());
                dispatcher.RegisterCommand("6", new ViewDiscardMenuListCommand());

                break;

            case 3:
                //Register employee commands
                dispatcher.RegisterCommand("1", new UpdateProfileCommand(() => userId));
                dispatcher.RegisterCommand("2", new ViewMenuCommand());
                dispatcher.RegisterCommand("3", new SumitFeedbackCommand(() => userId));
                dispatcher.RegisterCommand("4", new ViewEmployeeRecommendationsCommand(() => userId));
                dispatcher.RegisterCommand("5", new ProvideFeedbackCommand(() => userId));
                break;

            default:
                Console.WriteLine("Invalid role.");
                break;
        }

        bool exit = false;
        while (!exit)
        {
            DisplayRoleOperations(roleId);

            Console.WriteLine("Enter command: ");
            string commandKey = Console.ReadLine();

            if (commandKey.ToLower() == "exit")
            {
                exit = true;
            }
            else if ((roleId == 1 && commandKey == "5") || (roleId == 2 && commandKey == "7") || (roleId == 3 && commandKey == "6"))
            {
                exit = true;
                await dispatcher.Dispatch("logout", clientSocket);
            }
            else
            {
                await dispatcher.Dispatch(commandKey, clientSocket);
            }
        }
    }

    static void DisplayRoleOperations(int roleId)
    {
        switch (roleId)
        {
            case 1:
                Console.WriteLine("\nAdmin operations:");
                Console.WriteLine("1) Add menu");
                Console.WriteLine("2) Update menu");
                Console.WriteLine("3) Delete menu");
                Console.WriteLine("4) View Menu");
                Console.WriteLine("5) Logout");
                break;

            case 2:
                Console.WriteLine("\nChef operations:");
                Console.WriteLine("1) View Menu");
                Console.WriteLine("2) View Employee Feedback");
                Console.WriteLine("3) View Monthly Feedback report");
                Console.WriteLine("4) Roll out menu for next day");
                Console.WriteLine("5) View Employee Orders");
                Console.WriteLine("6) View dicard Menu list");
                Console.WriteLine("7) Logout");
                break;

            case 3:
                Console.WriteLine("\nEmployee operations");
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
