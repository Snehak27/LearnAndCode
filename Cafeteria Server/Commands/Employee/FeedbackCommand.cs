using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Newtonsoft.Json;

namespace CafeteriaServer.Commands
{
    public class FeedbackCommand : ICommand
    {
        private readonly IEmployeeService _employeeService;

        public FeedbackCommand(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<string> Execute(string requestData)
        {
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
                responseMessage.IsSuccess = false;
                responseMessage.ErrorMessage = ex.ToString();
            }

            return JsonConvert.SerializeObject(responseMessage);
        }
    }
}
