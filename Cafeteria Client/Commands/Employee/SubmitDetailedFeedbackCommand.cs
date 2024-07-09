using CafeteriaClient.DTO;
using CafeteriaClient.DTO.Request;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient.Commands.Employee
{
    public class SubmitDetailedFeedbackCommand : ICommand
    {
        private readonly Func<int> _getUserId;

        public SubmitDetailedFeedbackCommand(Func<int> getUserId)
        {
            _getUserId = getUserId;
        }

        public async Task Execute(ClientSocket clientSocket)
        {
            var userId = _getUserId();

            // Fetch pending feedback items
            var request = new RequestObject
            {
                CommandName = "getPendingFeedbackMenuItems",
                RequestData = JsonConvert.SerializeObject(new PendingFeedbackMenuItemsRequest { UserId = userId })
            };

            string itemsResponseJson = await clientSocket.SendRequest(request);
            var itemsResponse = JsonConvert.DeserializeObject<ViewMenuItemsResponse>(itemsResponseJson);

            if (itemsResponse.MenuItems == null || !itemsResponse.MenuItems.Any())
            {
                Console.WriteLine("No feedback requests found.");
                return;
            }

            var itemMapping = new Dictionary<int, int>();
            int serialNo = 1;
            Console.WriteLine("We are trying to improve your experience with the following food items. Please provide your feedback and help us.");
            Console.WriteLine("----------------------------------------------------------");
            foreach (var item in itemsResponse.MenuItems)
            {
                itemMapping[serialNo] = item.MenuItemId;
                Console.WriteLine($"Sl No: {serialNo}, Food Item: {item.ItemName}");
                serialNo++;
            }
            Console.WriteLine("----------------------------------------------------------");

            int selectedItem = GetValidInput("Enter Sl No of the Menu Item for which you want to provide feedback::", itemMapping.Keys.ToArray());

            if (itemMapping.TryGetValue(selectedItem, out int menuItemId))
            {
                Console.WriteLine("Q1. What didn’t you like about this item?");
                var dislikeReason = Console.ReadLine();

                Console.WriteLine("Q2. How would you like this item to taste?");
                var preferredTaste = Console.ReadLine();

                Console.WriteLine("Q3. Share your mom’s recipe:");
                var recipe = Console.ReadLine();

                var feedbackRequest = new DetailedFeedbackRequest
                {
                    UserId = userId,
                    MenuItemId = menuItemId,
                    Answers = new List<string> { dislikeReason, preferredTaste, recipe }
                };

                var requestObject = new RequestObject
                {
                    CommandName = "submitDetailedFeedback",
                    RequestData = JsonConvert.SerializeObject(feedbackRequest)
                };

                string responseJson = await clientSocket.SendRequest(requestObject);
                var response = JsonConvert.DeserializeObject<ResponseMessage>(responseJson);

                if (response.IsSuccess)
                {
                    Console.WriteLine("Feedback submitted successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to submit feedback: {response.ErrorMessage}");
                }
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }
        }

        private int GetValidInput(string prompt, int[] validOptions)
        {
            int choice = -1;
            while (!validOptions.Contains(choice))
            {
                Console.WriteLine(prompt);
                if (!int.TryParse(Console.ReadLine(), out choice) || !validOptions.Contains(choice))
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
            return choice;
        }
    }
}
