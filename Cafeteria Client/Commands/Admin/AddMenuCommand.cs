using CafeteriaClient.DTO;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient.Commands
{
    public class AddMenuCommand : ICommand
    {
        public async Task Execute(ClientSocket clientSocket)
        {
            string itemName;
            do
            {
                Console.WriteLine("Enter menu item name (cannot be blank):");
                itemName = Console.ReadLine();
            } while (string.IsNullOrEmpty(itemName));

            double price;
            while (true)
            {
                Console.WriteLine("Enter price:");
                string priceInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(priceInput) && double.TryParse(priceInput, out price) && price > 0)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid price.");
                }
            }

            bool availabilityStatus = GetBooleanInput("Enter availability status (true/false, leave blank to default to false):");

            Console.WriteLine("Select food type:");
            var foodTypeOptions = new Dictionary<int, string>
            {
                { 1, "Vegetarian" },
                { 2, "Non-Vegetarian" },
                { 3, "Eggetarian" }
            };
            DisplayOptions(foodTypeOptions);
            int foodTypeId = GetValidSelection(foodTypeOptions.Count);

            // Display options and get the selection for spice level
            Console.WriteLine("Select spice level:");
            var spiceLevelOptions = new Dictionary<int, string>
            {
                { 1, "High" },
                { 2, "Medium" },
                { 3, "Low" }
            };
            DisplayOptions(spiceLevelOptions);
            int spiceLevelId = GetValidSelection(spiceLevelOptions.Count);

            // Display options and get the selection for cuisine type
            Console.WriteLine("Select cuisine type:");
            var cuisineTypeOptions = new Dictionary<int, string>
            {
                { 1, "North Indian" },
                { 2, "South Indian" },
                { 3, "Other" }
            };
            DisplayOptions(cuisineTypeOptions);
            int cuisineTypeId = GetValidSelection(cuisineTypeOptions.Count);

            bool isSweet = GetBooleanInput("Is it a sweet dish? (true/false, leave blank to default to false):");

            var menuItem = new MenuItemRequest
            {
                ItemName = itemName,
                Price = price,
                AvailabilityStatus = availabilityStatus,
                FoodTypeId = foodTypeId,
                SpiceLevelId = spiceLevelId,
                CuisineTypeId = cuisineTypeId,
                IsSweet = isSweet
            };

            string menurequestJson = JsonConvert.SerializeObject(menuItem);

            var request = new RequestObject
            {
                CommandName = "addMenu",
                RequestData = menurequestJson
            };

            string responseJson = await clientSocket.SendRequest(request);
            var response = JsonConvert.DeserializeObject<ResponseMessage>(responseJson);

            if (response.IsSuccess)
            {
                Console.WriteLine("Menu item added successfully.");
            }
            else
            {
                Console.WriteLine("Failed to add menu item: " + response.ErrorMessage);
            }
        }

        private void DisplayOptions(Dictionary<int, string> options)
        {
            foreach (var option in options)
            {
                Console.WriteLine($"{option.Key}) {option.Value}");
            }
        }

        private int GetValidSelection(int maxOption)
        {
            int selection;
            while (true)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out selection) && selection > 0 && selection <= maxOption)
                {
                    break;
                }
                else
                {
                    Console.WriteLine($"Invalid input. Please enter a number between 1 and {maxOption}.");
                }
            }
            return selection;
        }

        private bool GetBooleanInput(string prompt)
        {
            bool result = false;
            while (true)
            {
                Console.WriteLine(prompt);
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input) || bool.TryParse(input, out result))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 'true' or 'false'.");
                }
            }
            return result;
        }
    }
}
