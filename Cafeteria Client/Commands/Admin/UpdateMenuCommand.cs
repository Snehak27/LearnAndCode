using CafeteriaClient.DTO;
using CafeteriaClient.Utils;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient.Commands.Admin
{
    public class UpdateMenuCommand : ICommand
    {
        private readonly Dictionary<int, string> foodTypeOptions = new Dictionary<int, string>
        {
            { 1, "Vegetarian" },
            { 2, "Non-Vegetarian" },
            { 3, "Eggetarian" }
        };

        private readonly Dictionary<int, string> spiceLevelOptions = new Dictionary<int, string>
        {
            { 1, "High" },
            { 2, "Medium" },
            { 3, "Low" }
        };

        private readonly Dictionary<int, string> cuisineTypeOptions = new Dictionary<int, string>
        {
            { 1, "North Indian" },
            { 2, "South Indian" },
            { 3, "Other" }
        };

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
                    Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine("| {0, -5} | {1, -20} | {2, 10} | {3, -15} | {4, -10} | {5, -15} | {6, -10} | {7, -10} ", "ID", "Name", "Price", "FoodType", "SpiceLevel", "CuisineType", "IsSweet", "Availability");
                    Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");

                    int serialNumber = 1;
                    foreach (var menuItem in response.MenuItems)
                    {
                        Console.WriteLine("| {0, -5} | {1, -20} | {2, 10} | {3, -15} | {4, -10} | {5, -15} | {6, -10} | {7, -10} ",
                            serialNumber,
                            menuItem.ItemName,
                            menuItem.Price,
                            GetOptionName(foodTypeOptions, menuItem.FoodTypeId),
                            GetOptionName(spiceLevelOptions, menuItem.SpiceLevelId),
                            GetOptionName(cuisineTypeOptions, menuItem.CuisineTypeId),
                            menuItem.IsSweet ? "Yes" : "No",
                            menuItem.AvailabilityStatus ? "Available" : "Unavailable");
                        serialNumber++;
                    }

                    Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");
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

                bool newAvailabilityStatus = UserInputHandler.GetBooleanInput($"Enter new availability status (true/false, leave blank to keep current):", menuItemToUpdate.AvailabilityStatus);

                Console.WriteLine("Select new food type (leave blank to keep current):");
                DisplayOptions(foodTypeOptions);
                int newFoodTypeId = UserInputHandler.GetValidSelection(foodTypeOptions.Count, menuItemToUpdate.FoodTypeId);

                Console.WriteLine("Select new spice level (leave blank to keep current):");
                DisplayOptions(spiceLevelOptions);
                int newSpiceLevelId = UserInputHandler.GetValidSelection(spiceLevelOptions.Count, menuItemToUpdate.SpiceLevelId);

                Console.WriteLine("Select new cuisine type (leave blank to keep current):");
                DisplayOptions(cuisineTypeOptions);
                int newCuisineTypeId = UserInputHandler.GetValidSelection(cuisineTypeOptions.Count, menuItemToUpdate.CuisineTypeId);

                bool newIsSweet = UserInputHandler.GetBooleanInput("Is it a sweet dish? (true/false, leave blank to keep current):", menuItemToUpdate.IsSweet);

                var updateMenuItemRequest = new MenuItem
                {
                    MenuItemId = menuItemToUpdate.MenuItemId,
                    ItemName = newName,
                    Price = newPrice,
                    AvailabilityStatus = newAvailabilityStatus,
                    FoodTypeId = newFoodTypeId,
                    SpiceLevelId = newSpiceLevelId,
                    CuisineTypeId = newCuisineTypeId,
                    IsSweet = newIsSweet
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

        private string GetOptionName(Dictionary<int, string> options, int id)
        {
            return options.ContainsKey(id) ? options[id] : "Unknown";
        }

        private void DisplayOptions(Dictionary<int, string> options)
        {
            foreach (var option in options)
            {
                Console.WriteLine($"{option.Key}) {option.Value}");
            }
        }
    }
}

