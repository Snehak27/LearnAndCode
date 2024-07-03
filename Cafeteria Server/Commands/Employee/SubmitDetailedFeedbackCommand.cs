using CafeteriaServer.DTO;
using CafeteriaServer.DTO.RequestModel;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands.Employee
{
    public class SubmitDetailedFeedbackCommand : ICommand
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<SubmitDetailedFeedbackCommand> _logger;

        public SubmitDetailedFeedbackCommand(IEmployeeService employeeService, ILogger<SubmitDetailedFeedbackCommand> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            var response = new ResponseMessage();

            try
            {
                var request = JsonConvert.DeserializeObject<DetailedFeedbackRequest>(requestData);
                var success = await _employeeService.SubmitDetailedFeedback(request);

                response.IsSuccess = success;
                response.ErrorMessage = success ? null : "Failed to submit feedback.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while submitting feedback");
                response.IsSuccess = false;
                response.ErrorMessage = ex.ToString();
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}
