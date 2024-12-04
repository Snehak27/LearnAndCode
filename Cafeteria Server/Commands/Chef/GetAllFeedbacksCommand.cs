using CafeteriaServer.Commands.Admin;
using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class GetAllFeedbacksCommand : ICommand
    {
        private readonly IChefService _chefService;
        private readonly ILogger<GetAllFeedbacksCommand> _logger;

        public GetAllFeedbacksCommand(IChefService chefService, ILogger<GetAllFeedbacksCommand> logger)
        {
            _chefService = chefService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            _logger.LogInformation("Get feedbacks endpoint invoked");

            var response = new ViewFeedbackResponse();

            try
            {
                var feedbacks = await _chefService.GetAllFeedbacks();
                response.IsSuccess = true;
                response.Feedbacks = feedbacks;
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
