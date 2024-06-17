using CafeteriaClient.DTO;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient.Commands.Admin
{
    public class UpdateMenuCommand : ICommand
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
                    for (int i = 0; i < response.MenuItems.Count; i++)
                    {
                        var menuItem = response.MenuItems[i];
                        Console.WriteLine($"{i + 1}. Name: {menuItem.ItemName}, Description: {menuItem.Description}, Price: {menuItem.Price}");
                    }
                }
                else
                {
                    Console.WriteLine("Failed to retrieve menu items: " + response.ErrorMessage);
                    return;
                }

                Console.WriteLine("Enter the number of the menu item to update:");
                int itemNumberToUpdate;
                if (!int.TryParse(Console.ReadLine(), out itemNumberToUpdate) || itemNumberToUpdate < 1 || itemNumberToUpdate > response.MenuItems.Count)
                {
                    Console.WriteLine("Invalid selection.");
                    return;
                }

                // Get the selected menu item
                var menuItemToUpdate = response.MenuItems[itemNumberToUpdate - 1];

                Console.WriteLine("Enter new name (leave blank to keep current):");
                string newName = Console.ReadLine();
                if (string.IsNullOrEmpty(newName)) newName = menuItemToUpdate.ItemName;

                Console.WriteLine("Enter new description (leave blank to keep current):");
                string newDescription = Console.ReadLine();
                if (string.IsNullOrEmpty(newDescription)) newDescription = menuItemToUpdate.Description;

                Console.WriteLine("Enter new price (leave blank to keep current):");
                string newPriceInput = Console.ReadLine();
                double newPrice = menuItemToUpdate.Price;
                if (!string.IsNullOrEmpty(newPriceInput) && double.TryParse(newPriceInput, out double parsedPrice))
                {
                    newPrice = parsedPrice;
                }

                var updateMenuItemRequest = new MenuItem
                {
                    MenuItemId = menuItemToUpdate.MenuItemId,
                    ItemName = newName,
                    Description = newDescription,
                    Price = newPrice
                };

                string updateRequestJson = JsonConvert.SerializeObject(updateMenuItemRequest);
                var updateRequest = new RequestObject
                {
                    CommandName = "updateMenu",
                    RequestData = updateRequestJson
                };

                string updateResponseJson = await clientSocket.SendRequest(updateRequest);
                var updateResponse = JsonConvert.DeserializeObject<ResponseMessage>(updateResponseJson);

                if (updateResponse.IsSuccess)
                {
                    Console.WriteLine("Menu item updated successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to update menu item: {updateResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing update menu command: {ex.Message}");
            }
        }
    }
}

