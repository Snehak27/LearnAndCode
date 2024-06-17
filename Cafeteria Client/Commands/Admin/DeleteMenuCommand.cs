using CafeteriaClient.DTO;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient.Commands.Admin
{
    public class DeleteMenuCommand : ICommand
    {
        public async Task Execute(ClientSocket clientSocket)
        {
            var request = new RequestObject();
            request.CommandName = "viewMenu";
            request.RequestData = string.Empty;

            string responseJson = await clientSocket.SendRequest(request);
            var response = JsonConvert.DeserializeObject<ViewMenuItemsResponse>(responseJson);

            if (response.IsSuccess)
            {
                int i = 0;
                Console.WriteLine("Menu Items:");
                foreach (var menuItem in response.MenuItems)
                {
                    Console.WriteLine($"{i + 1}. Name: {menuItem.ItemName}, Description: {menuItem.Description}, Price: {menuItem.Price}");
                    i++;
                }
            }
            else
            {
                Console.WriteLine("Failed to retrieve menu items: " + response.ErrorMessage);
                return;
            }

            Console.WriteLine("Enter the number of the menu item to delete:");
            int itemNumberToDelete;
            if (!int.TryParse(Console.ReadLine(), out itemNumberToDelete) || itemNumberToDelete < 1 || itemNumberToDelete > response.MenuItems.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            // Get the ID of the selected menu item
            int itemIdToDelete = response.MenuItems[itemNumberToDelete - 1].MenuItemId;

            string requestJson = JsonConvert.SerializeObject(itemIdToDelete);
            var deleteRequest = new RequestObject();
            deleteRequest.CommandName = "deleteMenu";
            deleteRequest.RequestData = requestJson;

            string deleteResponseJson = await clientSocket.SendRequest(deleteRequest);
            var deleteResponse = JsonConvert.DeserializeObject<ResponseMessage>(deleteResponseJson);

            if (deleteResponse.IsSuccess)
            {
                Console.WriteLine("Menu item deleted successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to delete menu item: {deleteResponse.ErrorMessage}");
            }
        }
    }
}
