using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class MonthlyFeedbackReportCommand : ICommand
    {
        private readonly IChefService _chefService;

        public MonthlyFeedbackReportCommand(IChefService chefService)
        {
            _chefService = chefService;
        }

        public async Task<string> Execute(string requestData)
        {
            var request = JsonConvert.DeserializeObject<MonthlyFeedbackReportRequest>(requestData);
            var response = new MonthlyFeedbackReportResponse();

            try
            {
                var report = await _chefService.GetMonthlyFeedbackReportAsync(request);
                response.IsSuccess = true;
                response.Report = report.Report;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}
