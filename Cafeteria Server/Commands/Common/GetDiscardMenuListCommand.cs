using CafeteriaServer.DTO.ResponseModel;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class GetDiscardMenuListCommand : ICommand
    {
        private readonly ISharedMenuService _sharedMenuService;
        private readonly ILogger<GetDiscardMenuListCommand> _logger;

        public GetDiscardMenuListCommand(ISharedMenuService sharedMenuService, ILogger<GetDiscardMenuListCommand> logger)
        {
            _sharedMenuService = sharedMenuService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            var response = new DiscardMenuResponse();

            try
            {
                _logger.LogInformation("GetDiscardMenuListCommand executed");

                var discardItems = await _sharedMenuService.GetDiscardMenuItems();
                response.IsSuccess = true;
                response.DiscardItems = discardItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the discard menu list");
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}
