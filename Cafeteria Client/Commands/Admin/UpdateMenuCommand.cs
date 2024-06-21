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
                    CommandName = "getAllMenuItems",
                    RequestData = string.Empty
                };

                string responseJson = await clientSocket.SendRequest(request);
                var response = JsonConvert.DeserializeObject<ViewMenuItemsResponse>(responseJson);

                if (response.IsSuccess)
                {
                    Console.WriteLine("Menu Items:");
                    Console.WriteLine("-----------------------------------------------------------------------------------");
                    Console.WriteLine("| {0, -10} | {1, -20}  | {2, 10} | {3, -20} ", "ID", "Name", "Price", "AvailabilityStatus");
                    Console.WriteLine("-----------------------------------------------------------------------------------");

                    int serialNumber = 1;
                    foreach (var menuItem in response.MenuItems)
                    {
                        Console.WriteLine("| {0, -10} | {1, -20} | {2, 10} | {3,-20} ", serialNumber, menuItem.ItemName, menuItem.Price, menuItem.AvailabilityStatus);
                        serialNumber++;
                    }

                    Console.WriteLine("-----------------------------------------------------------------------------------");
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

                Console.WriteLine("Enter new price (leave blank to keep current):");
                string newPriceInput = Console.ReadLine();
                double newPrice = menuItemToUpdate.Price;
                if (!string.IsNullOrEmpty(newPriceInput) && double.TryParse(newPriceInput, out double parsedPrice))
                {
                    newPrice = parsedPrice;
                }

                Console.WriteLine("Enter new availability status (true/false, leave blank to keep current):");
                string newAvailabilityStatusInput = Console.ReadLine();
                bool newAvailabilityStatus = menuItemToUpdate.AvailabilityStatus;
                if (!string.IsNullOrEmpty(newAvailabilityStatusInput))
                {
                    while (!bool.TryParse(newAvailabilityStatusInput, out newAvailabilityStatus))
                    {
                        Console.WriteLine("Invalid input. Please enter 'true' or 'false' for the availability status, or leave blank to keep current:");
                        newAvailabilityStatusInput = Console.ReadLine();

                        if (string.IsNullOrEmpty(newAvailabilityStatusInput))
                        {
                            newAvailabilityStatus = menuItemToUpdate.AvailabilityStatus;
                            break;
                        }
                    }
                }

                var updateMenuItemRequest = new MenuItem
                {
                    MenuItemId = menuItemToUpdate.MenuItemId,
                    ItemName = newName,
                    Price = newPrice,
                    AvailabilityStatus = newAvailabilityStatus
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
