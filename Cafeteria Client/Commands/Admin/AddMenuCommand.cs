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

            bool availabilityStatus = false;
            while (true)
            {
                Console.WriteLine("Enter availability status (true/false, leave blank to default to false):");
                string availabilityStatusInput = Console.ReadLine();
                if (string.IsNullOrEmpty(availabilityStatusInput) || bool.TryParse(availabilityStatusInput, out availabilityStatus))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 'true' or 'false'.");
                }
            }

            var menuItem= new MenuItemRequest
            {
                ItemName = itemName,
                Price = price,
                AvailabilityStatus = availabilityStatus,
            };

            string menurequestJson = JsonConvert.SerializeObject(menuItem);

            var request = new RequestObject();
            request.CommandName = "addMenu";
            request.RequestData = menurequestJson;

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
    }
}
