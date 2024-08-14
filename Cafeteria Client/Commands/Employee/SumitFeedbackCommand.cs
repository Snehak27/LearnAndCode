﻿using CafeteriaClient.DTO;
using CafeteriaClient.DTO.Request;
using CafeteriaClient.Enums;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace CafeteriaClient.Commands
{
    public class SumitFeedbackCommand : ICommand
    {
        private readonly Func<int> _getUserId;

        public SumitFeedbackCommand(Func<int> getUserId)
        {
            _getUserId = getUserId;
        }

        public async Task Execute(ClientSocket clientSocket)
        {
            try
            {
                var userId = _getUserId();

                var requestPastOrders = new RequestObject
                {
                    CommandName = "getPastOrders",
                    RequestData = JsonConvert.SerializeObject(new { UserId = userId })
                };

                string pastOrdersResponseJson = await clientSocket.SendRequest(requestPastOrders);
                var pastOrdersResponse = JsonConvert.DeserializeObject<PastOrdersResponse>(pastOrdersResponseJson);

                if (!pastOrdersResponse.IsSuccess)
                {
                    Console.WriteLine($"Failed to retrieve past orders: {pastOrdersResponse.ErrorMessage}");
                    return;
                }

                var pastOrders = pastOrdersResponse.PastOrders;

                if (pastOrders.Count == 0)
                {
                    Console.WriteLine("You have no past orders for the last week.");
                    return;
                }

                Console.WriteLine("Past Orders for the Last Week:");
                Console.WriteLine("--------------------------------------------------------------------------------");
                Console.WriteLine("| {0, -5} | {1, -30} | {2, -25} | {3, -10} ", "Sl No", "Menu Item", "Order Date", "Meal Type");
                Console.WriteLine("--------------------------------------------------------------------------------");

                int serialNumber = 1;
                var orderMapping = new Dictionary<int, PastOrderResponse>();
                foreach (var order in pastOrders)
                {
                    orderMapping[serialNumber] = order;
                    Console.WriteLine("| {0, -5} | {1, -30} | {2, -25} | {3, -10} ", serialNumber, order.MenuItemName, order.OrderDate, GetMealTypeName((MealType)order.MealTypeId));
                    serialNumber++;
                }

                Console.WriteLine("--------------------------------------------------------------------------------");

                Console.WriteLine("Enter the serial number for which you want to provide feedback:");
                int serialNo;
                if (!int.TryParse(Console.ReadLine(), out serialNo) || !orderMapping.ContainsKey(serialNo))
                {
                    Console.WriteLine("Invalid serial number.");
                    return;
                }

                var selectedOrder = orderMapping[serialNo];

                Console.WriteLine("Enter your rating (1-5):");
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
                    MenuItemId = selectedOrder.MenuItemId,
                    UserId = userId,
                    Comment = feedback,
                    Rating = rating,
                    MealTypeId = selectedOrder.MealTypeId,
                    OrderItemId = selectedOrder.OrderItemId,
                };

                string feedbackRequestJson = JsonConvert.SerializeObject(feedbackRequest);
                var feedbackRequestObject = new RequestObject
                {
                    CommandName = "submitFeedback",
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

        private string GetMealTypeName(MealType mealType)
        {
            return Enum.GetName(typeof(MealType), mealType);
        }
    }
}
