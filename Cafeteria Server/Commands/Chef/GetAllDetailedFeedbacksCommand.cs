using CafeteriaServer.DTO.ResponseModel;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CafeteriaServer.Commands.Chef
{
    public class GetAllDetailedFeedbacksCommand : ICommand
    {
        private readonly IChefService _chefService;
        private readonly ILogger<GetAllDetailedFeedbacksCommand> _logger;

        public GetAllDetailedFeedbacksCommand(IChefService chefService, ILogger<GetAllDetailedFeedbacksCommand> logger)
        {
            _chefService = chefService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            var response = new DetailedFeedbackResponse();

            try
            {
                var feedbacks = await _chefService.GetAllDetailedFeedbacks();
                response.DetailedFeedbacks = feedbacks.Select(f => new DetailedFeedbackDTO
                {
                    MenuItemName = f.MenuItem.ItemName,
                    FeedbackDate = f.FeedbackDate.ToString("yyyy-MM-dd"),
                    DislikeReason = f.DislikeReason,
                    PreferredTaste = f.PreferredTaste,
                    Recipe = f.Recipe
                }).ToList();

                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all detailed feedbacks");
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}
