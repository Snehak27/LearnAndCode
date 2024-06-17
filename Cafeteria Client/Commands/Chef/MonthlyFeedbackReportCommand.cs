using CafeteriaClient.DTO;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient.Commands
{
    public class MonthlyFeedbackReportCommand : ICommand
    {
        public async Task Execute(ClientSocket clientSocket)
        {
            try
            {
                Console.WriteLine("Enter the year for the feedback report:");
                int year = int.Parse(Console.ReadLine());

                Console.WriteLine("Enter the month for the feedback report (1-12):");
                int month = int.Parse(Console.ReadLine());

                var request = new MonthlyFeedbackReportRequest
                {
                    Year = year,
                    Month = month
                };

                string requestJson = JsonConvert.SerializeObject(request);
                var requestObject = new RequestObject
                {
                    CommandName = "viewMonthlyFeedbackReport",
                    RequestData = requestJson
                };

                string responseJson = await clientSocket.SendRequest(requestObject);
                var response = JsonConvert.DeserializeObject<MonthlyFeedbackReportResponse>(responseJson);

                if (response.IsSuccess)
                {
                    Console.WriteLine($"Feedback Report for {year}-{month:D2}:");
                    foreach (var summary in response.Report.FeedbackSummaries)
                    {
                        Console.WriteLine($"Menu Item: {summary.MenuItemName}, Average Rating: {summary.AverageRating:F2}, Feedback Count: {summary.FeedbackCount}");
                        foreach (var comment in summary.Comments)
                        {
                            Console.WriteLine($"  Comment: {comment}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve feedback report: {response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing view feedback report command: {ex.Message}");
            }
        }
    }
}
