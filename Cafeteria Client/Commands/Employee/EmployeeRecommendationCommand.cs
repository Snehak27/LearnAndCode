using CafeteriaClient.DTO;
using CafeteriaClient.DTO.Request;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient.Commands.Employee
{
    public class ViewEmployeeRecommendationsCommand : ICommand
    {
        private readonly Func<int> _getUserId;

        public ViewEmployeeRecommendationsCommand(Func<int> getUserId)
        {
            _getUserId = getUserId;
        }
        public async Task Execute(ClientSocket clientSocket)
        {
            try
            {
                var request = new RequestObject
                {
                    CommandName = "getEmployeeRecommendations",
                    RequestData = string.Empty
                };

                string responseJson = await clientSocket.SendRequest(request);
                var response = JsonConvert.DeserializeObject<EmployeeRecommendationResponse>(responseJson);

                if (response.IsSuccess)
                {
                    Console.WriteLine("Recommendations:");
                    int serialNo = 1;
                    var menuItemMapping = new Dictionary<int, (int MenuItemId, int MealTypeId, int RecommendedItemId)>();

                    foreach (var mealTypeRecommendation in response.MealTypeRecommendations)
                    {
                        Console.WriteLine($"\n{mealTypeRecommendation.MealTypeName}:");
                        Console.WriteLine("-----------------------------------------------------------------------------------------------------");
                        Console.WriteLine("| {0, -10} | {1, -30} | {2, 20} |{3, -20} |{4, -20}", "Sl No.", "Name", "Overall Rating", "Comments", "Sentiment");
                        Console.WriteLine("-----------------------------------------------------------------------------------------------------");

                        foreach (var recommendedItem in mealTypeRecommendation.RecommendedItems)
                        {
                            Console.WriteLine("| {0, -10} | {1, -30} | {2, 20:F2} | {3, -20} | {4, -20} ", serialNo, recommendedItem.MenuItemName, recommendedItem.PredictedRating, string.Join(", ", recommendedItem.Comments), recommendedItem.OverallSentiment);
                            menuItemMapping[serialNo] = (recommendedItem.MenuItemId, mealTypeRecommendation.MealTypeId, recommendedItem.RecommendedItemId);
                            serialNo++;
                        }
                        Console.WriteLine("----------------------------------------------------------------------------------------------------");
                    }

                    // Prompt the user to vote for menu items
                    var votes = new List<Vote>();

                    Console.WriteLine("\nEnter the serial numbers of the menu items to vote for (comma separated):");
                    string[] itemNumbers = Console.ReadLine().Split(',');

                    foreach (var itemNumber in itemNumbers)
                    {
                        if (int.TryParse(itemNumber.Trim(), out int serial))
                        {
                            if (menuItemMapping.TryGetValue(serial, out var item))
                            {
                                votes.Add(new Vote
                                {
                                    MenuItemId = item.MenuItemId,
                                    MealTypeId = item.MealTypeId,
                                    RecommendedItemId = item.RecommendedItemId
                                });
                            }
                        }
                    }
                    int userId = _getUserId();

                    // Send the votes to the server
                    var employeeResponseRequest = new EmployeeResponseRequest
                    {
                        UserId = userId,
                        Votes = votes
                    };

                    string requestJson = JsonConvert.SerializeObject(employeeResponseRequest);
                    var responseRequest = new RequestObject
                    {
                        CommandName = "saveEmployeeResponse",
                        RequestData = requestJson
                    };

                    string responseResultJson = await clientSocket.SendRequest(responseRequest);
                    var responseResult = JsonConvert.DeserializeObject<ResponseMessage>(responseResultJson);

                    if (responseResult.IsSuccess)
                    {
                        Console.WriteLine("Your votes have been successfully recorded.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to record your votes: " + responseResult.ErrorMessage);
                    }
                }
                else
                {
                    Console.WriteLine("Failed to retrieve recommendations: " + response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
