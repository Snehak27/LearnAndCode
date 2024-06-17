using CafeteriaClient.DTO;
using CafeteriaClient.DTO.Request;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient.Commands
{
    public class FeedbackCommand : ICommand
    {
        private readonly Func<int> _getUserId;

        public FeedbackCommand(Func<int> getUserId)
        {
            _getUserId = getUserId;
        }

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
                        Console.WriteLine($"{i + 1}. Name: {menuItem.ItemName}, Description: {menuItem.Description}, Price: {menuItem.Price:C}");
                    }
                }
                else
                {
                    Console.WriteLine("Failed to retrieve menu items: " + response.ErrorMessage);
                    return;
                }

                Console.WriteLine("Enter the number of the menu item to provide feedback:");
                int itemNumber;
                if (!int.TryParse(Console.ReadLine(), out itemNumber) || itemNumber < 1 || itemNumber > response.MenuItems.Count)
                {
                    Console.WriteLine("Invalid selection.");
                    return;
                }

                Console.WriteLine("Enter the meal type (1 for Breakfast, 2 for Lunch, 3 for Dinner):");
                int mealTypeId;
                if (!int.TryParse(Console.ReadLine(), out mealTypeId) || mealTypeId < 1 || mealTypeId > 3)
                {
                    Console.WriteLine("Invalid meal type. Please enter 1 for Breakfast, 2 for Lunch, or 3 for Dinner.");
                    return;
                }

                // Get the selected menu item
                var selectedMenuItem = response.MenuItems[itemNumber - 1];
                int userId = _getUserId();
              
                Console.WriteLine("Enter your rating (1 to 5):");
                int rating;
                if (!int.TryParse(Console.ReadLine(), out rating) || rating < 1 || rating > 5)
                {
                    Console.WriteLine("Invalid rating. Please enter a number between 1 and 5.");
                    return;
                }

                Console.WriteLine("Enter your feedback:");
                string feedback = Console.ReadLine();

                var feedbackRequest = new FeedbackRequest
                {
                    MenuItemId = selectedMenuItem.MenuItemId,
                    UserId = userId,
                    Comment = feedback,
                    Rating = rating,
                    MealTypeId = mealTypeId
                };

                string feedbackRequestJson = JsonConvert.SerializeObject(feedbackRequest);
                var feedbackRequestObject = new RequestObject
                {
                    CommandName = "feedback",
                    RequestData = feedbackRequestJson
                };

                string feedbackResponseJson = await clientSocket.SendRequest(feedbackRequestObject);
                var feedbackResponse = JsonConvert.DeserializeObject<ResponseMessage>(feedbackResponseJson);

                if (feedbackResponse.IsSuccess)
                {
                    Console.WriteLine("Feedback submitted successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to submit feedback: {feedbackResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing feedback command: {ex.Message}");
            }
        }
    }
}
