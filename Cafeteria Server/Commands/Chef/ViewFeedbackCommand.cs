using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class ViewFeedbackCommand : ICommand
    {
        private readonly IChefService _chefService;

        public ViewFeedbackCommand(IChefService chefService)
        {
            _chefService = chefService;
        }

        public async Task<string> Execute(string requestData)
        {
            var response = new ViewFeedbackResponse();

            try
            {
                var feedbacks = await _chefService.GetAllFeedbacks();
                response.IsSuccess = true;
                response.Feedbacks = feedbacks;
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
