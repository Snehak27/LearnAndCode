using CafeteriaClient.DTO;
using CafeteriaClient.DTO.Response;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient.Commands
{
    public class ViewEmployeeResponseCommand : ICommand
    {
        public async Task Execute(ClientSocket clientSocket)
        {
            try
            {
                var request = new RequestObject
                {
                    CommandName = "viewEmployeeResponses",
                    RequestData = string.Empty
                };

                string responseJson = await clientSocket.SendRequest(request);
                var response = JsonConvert.DeserializeObject<ViewEmployeeResponseResponse>(responseJson);

                if (response.IsSuccess)
                {
                    foreach (var summary in response.EmployeeResponses)
                    {
                        Console.WriteLine($"\n{summary.MealTypeName}:");
                        Console.WriteLine("--------------------------------------------------");
                        Console.WriteLine("| {0,-10} | {1,-20} | {2,-10} ", "Sl No", "Menu Item", "Votes");
                        Console.WriteLine("--------------------------------------------------");

                        int serialNumber = 1;
                        foreach (var vote in summary.MenuItemVotes)
                        {
                            Console.WriteLine("| {0,-10} | {1,-20} | {2,-10} ", serialNumber, vote.MenuItemName, vote.VoteCount);
                            serialNumber++;
                        }

                        Console.WriteLine("--------------------------------------------------");
                    }
                }
                else
                {
                    Console.WriteLine("Failed to retrieve employee responses: " + response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
