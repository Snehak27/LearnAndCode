using CafeteriaServer.DTO;
using CafeteriaServer.DTO.RequestModel;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class HandleDiscardActionsCommand : ICommand
    {
        private readonly ISharedMenuService _sharedMenuServiceService;
        private readonly ILogger<HandleDiscardActionsCommand> _logger;

        public HandleDiscardActionsCommand(ISharedMenuService sharedMenuServiceService, ILogger<HandleDiscardActionsCommand> logger)
        {
            _sharedMenuServiceService = sharedMenuServiceService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            var response = new ResponseMessage();

            try
            {
                var request = JsonConvert.DeserializeObject<DiscardActionRequest>(requestData);

                switch (request.Action)
                {
                    case "Remove":
                        var lastDiscardDate = await _sharedMenuServiceService.GetLastDiscardDate();
                        if (lastDiscardDate.HasValue && lastDiscardDate.Value.AddMonths(1) > DateTime.Now)
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "You can only discard menu items once a month.";
                        }
                        else
                        {
                            _logger.LogInformation("Removing menu items with IDs: {MenuItemIds}", string.Join(", ", request.MenuItemIds));
                            response.IsSuccess = await _sharedMenuServiceService.RemoveMenuItems(request.MenuItemIds);
                        }
                        break;

                    case "Feedback":
                        _logger.LogInformation("Requesting detailed feedback for menu items with IDs: {MenuItemIds}", string.Join(", ", request.MenuItemIds));
                        response.IsSuccess = await _sharedMenuServiceService.RequestDetailedFeedback(request.MenuItemIds);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.ErrorMessage = "Invalid action.";
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling discard action");
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}
