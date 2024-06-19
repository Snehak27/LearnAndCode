using CafeteriaClient.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CafeteriaClient.Commands
{
    public class RolloutMenuCommand : ICommand
    {
        public async Task Execute(ClientSocket clientSocket)
        {
            try
            {
                var recommendationsRequest = new RequestObject
                {
                    CommandName = "getRecommendations",
                    RequestData = string.Empty
                };

                string recommendationsResponseJson = await clientSocket.SendRequest(recommendationsRequest);
                var recommendationsResponse = JsonConvert.DeserializeObject<RecommendationResponse>(recommendationsResponseJson);

                if (!recommendationsResponse.IsSuccess)
                {
                    Console.WriteLine($"Failed to retrieve recommendations: {recommendationsResponse.ErrorMessage}");
                    return;
                }

                // Display recommendations to the chef
                Console.WriteLine("Recommendations:");
                foreach (var mealTypeRecommendation in recommendationsResponse.MealTypeRecommendations)
                {
                    string mealTypeName = GetMealTypeName(mealTypeRecommendation.MealTypeId);
                    Console.WriteLine($"\n{mealTypeName}:");
                    Console.WriteLine("-----------------------------------------------------------------------------------------------------");
                    Console.WriteLine("| {0, -4} | {1, -20} | {2, -20} | {3, -10} | {4, -10} | {5, -10} | {6, -30} ",
                        "Sl No.", "Name", "Overall Rating", "No. of Votes", "Avg Rating", "Sentiment", "Comments");
                    Console.WriteLine("-----------------------------------------------------------------------------------------------------");

                    for (int i = 0; i < mealTypeRecommendation.Recommendations.Count; i++)
                    {
                        var recommendation = mealTypeRecommendation.Recommendations[i];
                        Console.WriteLine("| {0, -4} | {1, -20} | {2, -20:F2} | {3, -10} | {4, -10:F2} | {5, -10} | {6, -30} ",
                            (i + 1), recommendation.MenuItemName, recommendation.PredictedRating, recommendation.VoteCount,
                            recommendation.AverageRating, recommendation.OverallSentiment, string.Join(", ", recommendation.Comments));
                    }

                    Console.WriteLine("-----------------------------------------------------------------------------------------------------");
                }

                // Fetch the full menu list from the server
                var menuRequest = new RequestObject
                {
                    CommandName = "viewMenu",
                    RequestData = string.Empty
                };

                string menuResponseJson = await clientSocket.SendRequest(menuRequest);
                var menuResponse = JsonConvert.DeserializeObject<ViewMenuItemsResponse>(menuResponseJson);

                if (!menuResponse.IsSuccess)
                {
                    Console.WriteLine($"Failed to retrieve menu items: {menuResponse.ErrorMessage}");
                    return;
                }

                // Display full menu list to the chef
                Console.WriteLine("\nFull Menu Items:");
                Console.WriteLine("----------------------------------------------------------------------------------");
                Console.WriteLine("| {0, -4} | {1, -20} | {2, 10} ", "Sl No.", "Name", "Price");
                Console.WriteLine("-----------------------------------------------------------------------------------");

                for (int i = 0; i < menuResponse.MenuItems.Count; i++)
                {
                    var menuItem = menuResponse.MenuItems[i];
                    Console.WriteLine("| {0, -4} | {1, -20} | {2, 10}  ", (i + 1), menuItem.ItemName, menuItem.Price);
                }

                Console.WriteLine("-----------------------------------------------------------------------------------");

                // Allow chef to select items from recommendations and full menu for each meal type
                var selectedMealTypeMenuItems = new List<MealTypeMenuItem>();

                foreach (var mealTypeRecommendation in recommendationsResponse.MealTypeRecommendations)
                {
                    var selectedMenuItemIds = new List<int>();
                    string mealTypeName = GetMealTypeName(mealTypeRecommendation.MealTypeId);

                    Console.WriteLine($"\nEnter the numbers of the recommended items to include in the final menu for {mealTypeName} (comma separated):");
                    string[] recommendedItemNumbers = Console.ReadLine().Split(',');

                    foreach (var itemNumber in recommendedItemNumbers)
                    {
                        if (int.TryParse(itemNumber.Trim(), out int index) && index > 0 && index <= mealTypeRecommendation.Recommendations.Count)
                        {
                            selectedMenuItemIds.Add(mealTypeRecommendation.Recommendations[index - 1].MenuItemId);
                        }
                    }

                    Console.WriteLine($"\nEnter the numbers of the menu items to include in the final menu for {mealTypeName} (comma separated):");
                    string[] menuItemNumbers = Console.ReadLine().Split(',');

                    foreach (var itemNumber in menuItemNumbers)
                    {
                        if (int.TryParse(itemNumber.Trim(), out int index) && index > 0 && index <= menuResponse.MenuItems.Count)
                        {
                            selectedMenuItemIds.Add(menuResponse.MenuItems[index - 1].MenuItemId);
                        }
                    }

                    selectedMealTypeMenuItems.Add(new MealTypeMenuItem
                    {
                        MealTypeId = mealTypeRecommendation.MealTypeId,
                        MenuItemIds = selectedMenuItemIds
                    });
                }

                var finalMenuRequest = new SaveFinalMenuRequest
                {
                    MealTypeMenuItems = selectedMealTypeMenuItems
                };

                string finalMenuRequestJson = JsonConvert.SerializeObject(finalMenuRequest);
                var finalMenuRequestObject = new RequestObject
                {
                    CommandName = "saveFinalMenu",
                    RequestData = finalMenuRequestJson
                };

                string finalMenuResponseJson = await clientSocket.SendRequest(finalMenuRequestObject);
                var finalMenuResponse = JsonConvert.DeserializeObject<ResponseMessage>(finalMenuResponseJson);

                if (finalMenuResponse.IsSuccess)
                {
                    Console.WriteLine("Final menu rolled out successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to roll out the final menu: {finalMenuResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing roll out final menu command: {ex.Message}");
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
