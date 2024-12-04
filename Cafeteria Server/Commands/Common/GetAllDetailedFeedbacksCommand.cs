using CafeteriaServer.DTO.ResponseModel;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CafeteriaServer.Commands.Chef
{
    public class GetAllDetailedFeedbacksCommand : ICommand
    {
        private readonly ISharedMenuService _sharedMenuService;
        private readonly ILogger<GetAllDetailedFeedbacksCommand> _logger;

        public GetAllDetailedFeedbacksCommand(ISharedMenuService sharedMenuService, ILogger<GetAllDetailedFeedbacksCommand> logger)
        {
            _sharedMenuService = sharedMenuService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            var response = new DetailedFeedbacksResponse();

            try
            {
                var feedbacks = await _sharedMenuService.GetAllDetailedFeedbacks();
                response.DetailedFeedbacks = feedbacks.Select(f => new DetailedFeedbackResponse
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
