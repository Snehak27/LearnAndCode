using CafeteriaServer.Commands.Admin;
using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class GetChefRecommendationsCommand : ICommand
    {
        private readonly IChefService _chefService;
        private readonly ILogger<GetChefRecommendationsCommand> _logger;


        public GetChefRecommendationsCommand(IChefService chefService, ILogger<GetChefRecommendationsCommand> logger)
        {
            _chefService = chefService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            _logger.LogInformation("Get chef recommendations endpoint invoked");

            var response = new RecommendationResponse();

            try
            {
                var mealTypeRecommendations = await _chefService.GetRecommendations();
                response.IsSuccess = true;
                response.MealTypeRecommendations = mealTypeRecommendations;
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
