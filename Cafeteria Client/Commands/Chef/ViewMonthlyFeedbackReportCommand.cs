using CafeteriaClient.DTO;
using CafeteriaClient.DTO.Response;
using Newtonsoft.Json;
using System;

namespace CafeteriaClient.Commands
{
    public class ViewMonthlyFeedbackReportCommand : ICommand
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
                    CommandName = "getFeedbackReport",
                    RequestData = requestJson
                };

                string responseJson = await clientSocket.SendRequest(requestObject);
                var response = JsonConvert.DeserializeObject<MonthlyFeedbackReportResponse>(responseJson);

                if (response.IsSuccess)
                {
                    Console.WriteLine($"Feedback Report for {year}-{month:D2}:");
                    Console.WriteLine("----------------------------------------------------------------------------------");
                    Console.WriteLine("| {0, -10} | {1, -30} | {2, 15} | {3, 15} |","Sl No", "Menu Item", "Avg Rating", "Feedback Count");
                    Console.WriteLine("----------------------------------------------------------------------------------");

                    int serialNo = 1;
                    foreach (var summary in response.Report.FeedbackSummaries)
                    {
                        Console.WriteLine("| {0, -10} | {1, -30} | {2, 15:F2} | {3, 15} |", serialNo, summary.MenuItemName, summary.AverageRating, summary.FeedbackCount);
                        serialNo++;
                    }

                    Console.WriteLine("----------------------------------------------------------------------------------");
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
