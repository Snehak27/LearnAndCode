using CafeteriaClient.DTO;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient.Commands.Chef
{
    public class GetRecommendationsCommand : ICommand
    {
        public async Task Execute(ClientSocket clientSocket)
        {
            try
            {
                var request = new RequestObject
                {
                    CommandName = "getRecommendations",
                    RequestData = string.Empty
                };

                string responseJson = await clientSocket.SendRequest(request);
                var response = JsonConvert.DeserializeObject<RecommendationResponse>(responseJson);

                // Display the recommendations to the chef
                if (response.IsSuccess)
                {
                    Console.WriteLine("Recommendations:");
                    foreach (var mealTypeRecommendation in response.MealTypeRecommendations)
                    {
                        string mealTypeName = GetMealTypeName(mealTypeRecommendation.MealTypeId);
                        Console.WriteLine($"\n{mealTypeName}:");
                        foreach (var recommendation in mealTypeRecommendation.Recommendations)
                        {
                            Console.Write($"- Menu item: {recommendation.MenuItemName} Rating: {recommendation.PredictedRating:F2}");
                            foreach (var comment in recommendation.Comments)
                            {
                                Console.Write($"  Comment: {comment}");
                            }
                            Console.WriteLine();
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve recommendations: {response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing get recommendations command: {ex.Message}");
            }
        }

        private string GetMealTypeName(int mealTypeId)
        {
            return mealTypeId switch
            {
                1 => "Breakfast",
                2 => "Lunch",
                3 => "Dinner",
                _ => "Unknown Meal Type"
            };
        }
    }
}
