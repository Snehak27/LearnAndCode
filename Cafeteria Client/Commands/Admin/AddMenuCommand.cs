using CafeteriaClient.DTO;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient.Commands
{
    public class AddMenuCommand : ICommand
    {
        public async Task Execute(ClientSocket clientSocket)
        {
            Console.WriteLine("Enter menu item name:");
            string itemName = Console.ReadLine();

            Console.WriteLine("Enter price:");
            double price = Convert.ToDouble(Console.ReadLine());

            Console.WriteLine("Enter availability status (true/false):");
            string availabilityStatusInput = Console.ReadLine();
            bool availabilityStatus = false;
            if (!string.IsNullOrEmpty(availabilityStatusInput) && bool.TryParse(availabilityStatusInput, out bool parsedAvailabilityStatus))
            {
                availabilityStatus = parsedAvailabilityStatus;
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
