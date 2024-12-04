using CafeteriaServer.Commands.Admin;
using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CafeteriaServer.Commands
{
    public class SubmitFeedbackCommand : ICommand
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<SubmitFeedbackCommand> _logger;


        public SubmitFeedbackCommand(IEmployeeService employeeService, ILogger<SubmitFeedbackCommand> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            _logger.LogInformation("Submit feedback endpoint invoked");

            var responseMessage = new ResponseMessage();

            try
            {
                var feedbackRequest = JsonConvert.DeserializeObject<FeedbackRequest>(requestData);
                bool feedbackSuccess = await _employeeService.ProvideFeedback(feedbackRequest);
                responseMessage.IsSuccess = feedbackSuccess;
                responseMessage.ErrorMessage = feedbackSuccess ? null : "Failed to submit feedback.";
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                responseMessage.IsSuccess = false;
                responseMessage.ErrorMessage = ex.ToString();
            }

            return JsonConvert.SerializeObject(responseMessage);
        }
    }
}
