using CafeteriaServer.Commands.Admin;
using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class GetFeedbackReportCommand : ICommand
    {
        private readonly IChefService _chefService;
        private readonly ILogger<GetFeedbackReportCommand> _logger;

        public GetFeedbackReportCommand(IChefService chefService, ILogger<GetFeedbackReportCommand> logger)
        {
            _chefService = chefService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            _logger.LogInformation("Get feedback report endpoint invoked");

            var request = JsonConvert.DeserializeObject<MonthlyFeedbackReportRequest>(requestData);
            var response = new MonthlyFeedbackReportResponse();

            try
            {
                var report = await _chefService.GetMonthlyFeedbackReport(request);
                response.IsSuccess = true;
                response.Report = report.Report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}
