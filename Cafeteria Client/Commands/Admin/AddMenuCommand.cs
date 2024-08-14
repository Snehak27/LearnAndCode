using CafeteriaClient.DTO;
using CafeteriaClient.Enums;
using CafeteriaClient.Utils;
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

            bool availabilityStatus = UserInputHandler.GetBooleanInput("Enter availability status (true/false, leave blank to default to false):");

            Console.WriteLine("Select food type:");
            DisplayOptions<FoodType>();
            FoodType foodType = (FoodType)UserInputHandler.GetValidSelection(Enum.GetValues(typeof(FoodType)).Length);

            Console.WriteLine("Select spice level:");
            DisplayOptions<SpiceLevel>();
            SpiceLevel spiceLevel = (SpiceLevel)UserInputHandler.GetValidSelection(Enum.GetValues(typeof(SpiceLevel)).Length);

            Console.WriteLine("Select cuisine type:");
            DisplayOptions<CuisineType>();
            CuisineType cuisineType = (CuisineType)UserInputHandler.GetValidSelection(Enum.GetValues(typeof(CuisineType)).Length);

            bool isSweet = UserInputHandler.GetBooleanInput("Is it a sweet dish? (true/false, leave blank to default to false):");

            var menuItem = new MenuItemRequest
            {
                ItemName = itemName,
                Price = price,
                AvailabilityStatus = availabilityStatus,
                FoodTypeId = (int)foodType,
                SpiceLevelId = (int)spiceLevel,
                CuisineTypeId = (int)cuisineType,
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

        private void DisplayOptions<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T)).Cast<T>();
            foreach (var value in values)
            {
                Console.WriteLine($"{(int)(object)value}) {value}");
            }
        }
    }
}
