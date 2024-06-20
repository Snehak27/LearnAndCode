using CafeteriaClient.DTO;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient.Commands
{
    public class ViewMenuCommand : ICommand
    {
        public async Task Execute(ClientSocket clientSocket)
        {
            try
            {
                var request = new RequestObject
                {
                    CommandName = "viewMenu",
                    RequestData = string.Empty
                };

                string responseJson = await clientSocket.SendRequest(request);
                var response = JsonConvert.DeserializeObject<ViewMenuItemsResponse>(responseJson);

                if (response.IsSuccess)
                {
                    Console.WriteLine("Menu Items:");
                    Console.WriteLine("-----------------------------------------------------------------------------------------");
                    Console.WriteLine("| {0, -10} | {1, -20} | {2, 10} | {3,-20} ", "Sl No.", "Name", "Price", "AvailabilityStatus");
                    Console.WriteLine("-----------------------------------------------------------------------------------------");

                    int serialNumber = 1;
                    foreach (var menuItem in response.MenuItems)
                    {
                        Console.WriteLine("| {0, -10} | {1, -20} | {2, 10} | {3,-20} ", serialNumber, menuItem.ItemName, menuItem.Price, menuItem.AvailabilityStatus);
                        serialNumber++;
                    }

                    Console.WriteLine("-----------------------------------------------------------------------------------------");
                }
                else
                {
                    Console.WriteLine("Failed to retrieve menu items: " + response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
