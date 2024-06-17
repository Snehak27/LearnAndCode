using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class RecommendationCommand : ICommand
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendationCommand(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        public async Task<string> Execute(string requestData)
        {
            var response = new RecommendationResponse();

            try
            {
                var mealTypeRecommendations = await _recommendationService.GetRecommendations();
                response.IsSuccess = true;
                response.MealTypeRecommendations = mealTypeRecommendations;
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
