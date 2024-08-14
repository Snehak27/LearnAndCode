using CafeteriaClient.DTO;
using CafeteriaClient.DTO.Request;
using CafeteriaClient.DTO.Response;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient.Commands
{
    public class ViewDiscardMenuListCommand : ICommand
    {
        public async Task Execute(ClientSocket clientSocket)
        {
            var request = new RequestObject
            {
                CommandName = "getDiscardMenuList",
                RequestData = string.Empty
            };

            string responseJson = await clientSocket.SendRequest(request);
            var response = JsonConvert.DeserializeObject<DiscardMenuResponse>(responseJson);

            if (response.DiscardItems != null && response.DiscardItems.Any())
            {
                Console.WriteLine("Discard Menu Item List:");
                var itemMapping = new Dictionary<int, int>();
                int serialNo = 1;

                Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("| {0, -5} | {1, -20} | {2, -15} | {3, -40} ", "Sl No", "Food Item", "Avg Rating", "Sentiments");
                Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------");
                foreach (var item in response.DiscardItems)
                {
                    itemMapping[serialNo] = item.MenuItemId;
                    Console.WriteLine("| {0, -5} | {1, -20} | {2, -15:F2} | {3, -40} ", serialNo, item.MenuItemName, item.AverageRating, string.Join(", ", item.Sentiments));
                    Console.WriteLine("----------------------------------------------------------------------------------------------------------------------------");
                    serialNo++;
                }

                Console.WriteLine("Options:");
                Console.WriteLine("1) Remove Food Item");
                Console.WriteLine("2) Get Detailed Feedback for the menu item");
                Console.WriteLine("3) View all Detailed Feedbacks");
                Console.WriteLine("4) Exit");

                var option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        List<int> menuItemIdsToRemove; 
                        do
                        {
                            Console.WriteLine("Enter Sl Nos of the Food Items to remove (comma separated):");
                            var slNosToRemove = Console.ReadLine().Split(',');

                            menuItemIdsToRemove = slNosToRemove
                                .Select(slNo => int.TryParse(slNo.Trim(), out int parsedSlNo) && itemMapping.TryGetValue(parsedSlNo, out int menuItemId) ? menuItemId : (int?)null)
                                .Where(id => id.HasValue)
                                .Select(id => id.Value)
                                .ToList();

                            var invalidSlNos = slNosToRemove
                                .Where(slNo => !int.TryParse(slNo.Trim(), out int parsedSlNo) || !itemMapping.ContainsKey(parsedSlNo))
                                .ToList();

                            if (invalidSlNos.Any())
                            {
                                Console.WriteLine($"Invalid Sl Nos: {string.Join(", ", invalidSlNos)}. Please enter valid serial numbers.");
                            }
                        } while (!menuItemIdsToRemove.Any());

                        await HandleDiscardAction(clientSocket, "Remove", menuItemIdsToRemove); 
                        break;

                    case "2":
                        Console.WriteLine("Enter Sl No of the Food Item to get detailed feedback:");
                        if (int.TryParse(Console.ReadLine(), out int slNoToFeedback) && itemMapping.TryGetValue(slNoToFeedback, out int menuItemIdToFeedback))
                        {
                            await HandleDiscardAction(clientSocket, "Feedback", new List<int> {menuItemIdToFeedback});
                        }
                        else
                        {
                            Console.WriteLine("Invalid Sl No");
                        }
                        break;

                    case "3":
                        await ViewAllDetailedFeedbacks(clientSocket);
                        break;

                    case "4":
                        return;

                    default:
                        Console.WriteLine("Invalid option");
                        break;
                }
            }
            else
            {
                Console.WriteLine("No items to discard.");
            }
        }

        private async Task HandleDiscardAction(ClientSocket clientSocket, string action, List<int> menuItemIds)
        {
            var request = new RequestObject
            {
                CommandName = "handleDiscardActions",
                RequestData = JsonConvert.SerializeObject(new DiscardActionRequest
                {
                    Action = action,
                    MenuItemIds = menuItemIds
                })
            };

            string responseJson = await clientSocket.SendRequest(request);
            var response = JsonConvert.DeserializeObject<ResponseMessage>(responseJson);

            if (response.IsSuccess)
            {
                Console.WriteLine("Request successful");
            }
            else
            {
                Console.WriteLine($"Failed to perform action: {response.ErrorMessage}");
            }
        }

        private async Task ViewAllDetailedFeedbacks(ClientSocket clientSocket)
        {
            var request = new RequestObject
            {
                CommandName = "getAllDetailedFeedbacks",
                RequestData = string.Empty
            };

            string responseJson = await clientSocket.SendRequest(request);
            var response = JsonConvert.DeserializeObject<DetailedFeedbacksResponse>(responseJson);

            if (response.DetailedFeedbacks != null && response.DetailedFeedbacks.Any())
            {
                Console.WriteLine("All Detailed Feedbacks:");
                Console.WriteLine("--------------------------------------------------------------");
                foreach (var feedback in response.DetailedFeedbacks)
                {
                    Console.WriteLine($"Food Item: {feedback.MenuItemName}");
                    Console.WriteLine($"Feedback Date: {feedback.FeedbackDate}");
                    Console.WriteLine($"Dislike Reason: {feedback.DislikeReason}");
                    Console.WriteLine($"Preferred Taste: {feedback.PreferredTaste}");
                    Console.WriteLine($"Recipe: {feedback.Recipe}");
                    Console.WriteLine("------------------------------------------------------------");
                }
            }
            else
            {
                Console.WriteLine("No detailed feedbacks available.");
            }
        }
    }
}