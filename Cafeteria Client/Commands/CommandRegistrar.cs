using CafeteriaClient.Commands.Admin;
using CafeteriaClient.Commands.Chef;
using CafeteriaClient.Commands.Employee;
using CafeteriaClient.Commands;
using System;

namespace CafeteriaClient
{
    public class CommandRegistrar
    {
        public void RegisterCommands(int roleId, CommandDispatcher dispatcher, Func<int> getUserId)
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
                    //Register chef commands
                    dispatcher.RegisterCommand("1", new ViewMenuCommand());
                    dispatcher.RegisterCommand("2", new ViewFeedbackCommand());
                    dispatcher.RegisterCommand("3", new ViewMonthlyFeedbackReportCommand());
                    dispatcher.RegisterCommand("4", new RolloutMenuCommand());
                    dispatcher.RegisterCommand("5", new ViewEmployeeOrdersCommand());
                    dispatcher.RegisterCommand("6", new ViewDiscardMenuListCommand());
                    break;

                case 3:
                    //Register employee commands
                    dispatcher.RegisterCommand("1", new UpdateProfileCommand(getUserId));
                    dispatcher.RegisterCommand("2", new ViewMenuCommand());
                    dispatcher.RegisterCommand("3", new SumitFeedbackCommand(getUserId));
                    dispatcher.RegisterCommand("4", new ViewEmployeeRecommendationsCommand(getUserId));
                    dispatcher.RegisterCommand("5", new ProvideFeedbackCommand(getUserId));
                    break;

                default:
                    Console.WriteLine("Invalid role.");
                    break;
            }
        }
    }
}
