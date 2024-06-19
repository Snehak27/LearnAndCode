using CafeteriaClient.DTO;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient.Commands.Chef
{
    public class ViewFeedbackCommand : ICommand
    {
        public async Task Execute(ClientSocket clientSocket)
        {
            try
            {
                var request = new RequestObject
                {
                    CommandName = "viewFeedback",
                    RequestData = string.Empty
                };

                string responseJson = await clientSocket.SendRequest(request);
                var response = JsonConvert.DeserializeObject<ViewFeedbackResponse>(responseJson);

                if (response.IsSuccess)
                {
                    Console.WriteLine("Employee Feedback:");
                    Console.WriteLine("----------------------------------------------------------------------------------");
                    Console.WriteLine("| {0, -30} | {1, 6} | {2, -40} ", "Menu Item", "Rating", "Comment");
                    Console.WriteLine("----------------------------------------------------------------------------------");

                    foreach (var feedback in response.Feedbacks)
                    {
                        Console.WriteLine("| {0, -30} | {1, 6} | {2, -40} ", feedback.MenuItemName, feedback.Rating, feedback.Comment);
                    }

                    Console.WriteLine("-----------------------------------------------------------------------------------");
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve feedback: {response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing view feedback command: {ex.Message}");
            }
        }
    }
}
