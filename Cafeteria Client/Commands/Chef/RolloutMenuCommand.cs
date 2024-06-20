using CafeteriaClient.DTO;
using CafeteriaClient.DTO.Request;
using Newtonsoft.Json;
using System;

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

                DisplayRecommendations(recommendationsResponse);

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

                // Display Full Menu
                DisplayMenu(menuResponse);

                var selectedMealTypeMenuItems = SelectMenuItems(recommendationsResponse, menuResponse);

                bool isAnyMealTypeSelected = selectedMealTypeMenuItems.Any(m => m.MenuItemIds.Count > 0);

                if (!isAnyMealTypeSelected)
                {
                    Console.WriteLine("No items selected for any meal type. Nothing to roll out.");
                    return;
                }

                // Save Final Menu
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

        private void DisplayRecommendations(RecommendationResponse recommendationsResponse)
        {
            Console.WriteLine("Recommendations:");
            foreach (var mealTypeRecommendation in recommendationsResponse.MealTypeRecommendations)
            {
                string mealTypeName = GetMealTypeName(mealTypeRecommendation.MealTypeId);
                Console.WriteLine($"\n{mealTypeName}:");
                Console.WriteLine("-----------------------------------------------------------------------------------------------------");
                Console.WriteLine("| {0, -10} | {1, -20} | {2, -20} | {3, -10} | {4, -10} | {5, -10} | {6, -30} ",
                    "Sl No.", "Name", "Overall Rating", "No. of Votes", "Avg Rating", "Sentiment", "Comments");
                Console.WriteLine("-----------------------------------------------------------------------------------------------------");

                for (int i = 0; i < mealTypeRecommendation.Recommendations.Count; i++)
                {
                    var recommendation = mealTypeRecommendation.Recommendations[i];
                    Console.WriteLine("| {0, -10} | {1, -20} | {2, -20:F2} | {3, -10} | {4, -10:F2} | {5, -10} | {6, -30} ",
                        (i + 1), recommendation.MenuItemName, recommendation.PredictedRating, recommendation.VoteCount,
                        recommendation.AverageRating, recommendation.OverallSentiment, string.Join(", ", recommendation.Comments));
                }

                Console.WriteLine("-----------------------------------------------------------------------------------------------------");
            }
        }

        private void DisplayMenu(ViewMenuItemsResponse menuResponse)
        {
            Console.WriteLine("\nFull Menu Items:");
            Console.WriteLine("----------------------------------------------------------------------------------");
            Console.WriteLine("| {0, -10} | {1, -20} | {2, 10} ", "Sl No.", "Name", "Price");
            Console.WriteLine("-----------------------------------------------------------------------------------");

            for (int i = 0; i < menuResponse.MenuItems.Count; i++)
            {
                var menuItem = menuResponse.MenuItems[i];
                Console.WriteLine("| {0, -10} | {1, -20} | {2, 10}  ", (i + 1), menuItem.ItemName, menuItem.Price);
            }

            Console.WriteLine("-----------------------------------------------------------------------------------");
        }

        private List<MealTypeMenuItemList> SelectMenuItems(RecommendationResponse recommendationsResponse, ViewMenuItemsResponse menuResponse)
        {
            var selectedMealTypeMenuItems = new List<MealTypeMenuItemList>();

            foreach (var mealTypeRecommendation in recommendationsResponse.MealTypeRecommendations)
            {
                var selectedMenuItemIds = new List<int>();
                string mealTypeName = GetMealTypeName(mealTypeRecommendation.MealTypeId);

                // Select recommended items
                while (true)
                {
                    Console.WriteLine($"\nEnter the numbers of the recommended items to include in the final menu for {mealTypeName} (comma separated):");
                    string[] recommendedItemNumbers = Console.ReadLine()?.Split(',');

                    bool isValid = true;
                    if (recommendedItemNumbers != null && recommendedItemNumbers.Length > 0)
                    {
                        foreach (var itemNumber in recommendedItemNumbers)
                        {
                            if (!string.IsNullOrWhiteSpace(itemNumber) && (!int.TryParse(itemNumber.Trim(), out int index) || index <= 0 || index > mealTypeRecommendation.Recommendations.Count))
                            {
                                Console.WriteLine($"Invalid input: {itemNumber}. Please enter a valid number.");
                                isValid = false;
                                break;
                            }
                        }
                    }

                    if (isValid)
                    {
                        if (recommendedItemNumbers != null && recommendedItemNumbers.Length > 0)
                        {
                            foreach (var itemNumber in recommendedItemNumbers)
                            {
                                if (int.TryParse(itemNumber.Trim(), out int index) && index > 0 && index <= mealTypeRecommendation.Recommendations.Count)
                                {
                                    var recommendation = mealTypeRecommendation.Recommendations[index - 1];
                                    if (!selectedMenuItemIds.Contains(recommendation.MenuItemId))
                                    {
                                        selectedMenuItemIds.Add(recommendation.MenuItemId);
                                    }
                                }
                            }
                        }
                        break;
                    }
                }

                // Select additional menu items
                while (true)
                {
                    Console.WriteLine($"\nEnter the numbers of the menu items to include in the final menu for {mealTypeName} (comma separated):");
                    string[] menuItemNumbers = Console.ReadLine()?.Split(',');

                    bool isValid = true;
                    if (menuItemNumbers != null && menuItemNumbers.Length > 0)
                    {
                        foreach (var itemNumber in menuItemNumbers)
                        {
                            if (!string.IsNullOrWhiteSpace(itemNumber) && (!int.TryParse(itemNumber.Trim(), out int index) || index <= 0 || index > menuResponse.MenuItems.Count))
                            {
                                Console.WriteLine($"Invalid input: {itemNumber}. Please enter a valid number.");
                                isValid = false;
                                break;
                            }
                        }
                    }

                    if (isValid)
                    {
                        if (menuItemNumbers != null && menuItemNumbers.Length > 0)
                        {
                            foreach (var itemNumber in menuItemNumbers)
                            {
                                if (int.TryParse(itemNumber.Trim(), out int index) && index > 0 && index <= menuResponse.MenuItems.Count)
                                {
                                    var menuItem = menuResponse.MenuItems[index - 1];
                                    if (!selectedMenuItemIds.Contains(menuItem.MenuItemId))
                                    {
                                        selectedMenuItemIds.Add(menuItem.MenuItemId);
                                    }
                                }
                            }
                        }
                        break;
                    }
                }

                selectedMealTypeMenuItems.Add(new MealTypeMenuItemList
                {
                    MealTypeId = mealTypeRecommendation.MealTypeId,
                    MenuItemIds = selectedMenuItemIds
                });
                
            }

            return selectedMealTypeMenuItems;
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
