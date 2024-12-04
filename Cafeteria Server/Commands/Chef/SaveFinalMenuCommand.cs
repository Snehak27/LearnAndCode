using CafeteriaServer.Commands.Admin;
using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class SaveFinalMenuCommand : ICommand
    {
        private readonly IChefService _chefService;
        private readonly ILogger<SaveFinalMenuCommand> _logger;

        public SaveFinalMenuCommand(IChefService chefService, ILogger<SaveFinalMenuCommand> logger)
        {
            _chefService = chefService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            _logger.LogInformation("Save final menu endpoint invoked");

            var response = new ResponseMessage();

            try
            {
                var request = JsonConvert.DeserializeObject<SaveFinalMenuRequest>(requestData);
                await _chefService.SaveFinalMenu(request.MealTypeMenuItems);
                response.IsSuccess = true;
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
