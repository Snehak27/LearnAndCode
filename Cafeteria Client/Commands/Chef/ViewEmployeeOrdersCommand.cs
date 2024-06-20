using CafeteriaClient.DTO;
using CafeteriaClient.DTO.Response;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient.Commands
{
    public class ViewEmployeeOrdersCommand : ICommand
    {
        public async Task Execute(ClientSocket clientSocket)
        {
            try
            {
                var request = new RequestObject
                {
                    CommandName = "viewEmployeeOrders",
                    RequestData = string.Empty
                };

                string responseJson = await clientSocket.SendRequest(request);
                var response = JsonConvert.DeserializeObject<ViewEmployeeOrdersResponse>(responseJson);

                if (response.IsSuccess)
                {
                    foreach (var summary in response.EmployeeOrders)
                    {
                        Console.WriteLine($"\n{summary.MealTypeName}:");
                        Console.WriteLine("--------------------------------------------------");
                        Console.WriteLine("| {0,-10} | {1,-20} | {2,-10} ", "Sl No", "Menu Item", "Orders");
                        Console.WriteLine("--------------------------------------------------");

                        int serialNumber = 1;
                        foreach (var order in summary.MenuItemOrders)
                        {
                            Console.WriteLine("| {0,-10} | {1,-20} | {2,-10} ", serialNumber, order.MenuItemName, order.OrderCount);
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
