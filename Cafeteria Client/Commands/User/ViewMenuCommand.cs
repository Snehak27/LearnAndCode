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
                    foreach (var menuItem in response.MenuItems)
                    {
                        Console.WriteLine($"ID: {menuItem.MenuItemId}, Name: {menuItem.ItemName}, Description: {menuItem.Description}, Price: {menuItem.Price}");
                    }
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
