using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class ChefRecommendationCommand : ICommand
    {
        private readonly IChefService _chefService;

        public ChefRecommendationCommand(IChefService chefService)
        {
            _chefService = chefService;
        }

        public async Task<string> Execute(string requestData)
        {
            var response = new RecommendationResponse();

            try
            {
                var mealTypeRecommendations = await _chefService.GetRecommendations();
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
