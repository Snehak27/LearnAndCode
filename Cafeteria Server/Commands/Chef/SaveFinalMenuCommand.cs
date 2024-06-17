using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class SaveFinalMenuCommand : ICommand
    {
        private readonly IRecommendationService _recommendationService;

        public SaveFinalMenuCommand(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        public async Task<string> Execute(string requestData)
        {
            var response = new ResponseMessage();

            try
            {
                var request = JsonConvert.DeserializeObject<SaveFinalMenuRequest>(requestData);
                await _recommendationService.SaveFinalMenuAsync(request.MealTypeMenuItems);
                response.IsSuccess = true;
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
