using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
using CafeteriaServer.DTO.RequestModel;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands.Employee
{
    public class GetPendingFeedbackMenuItemsCommand : ICommand
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<GetPendingFeedbackMenuItemsCommand> _logger;

        public GetPendingFeedbackMenuItemsCommand(IEmployeeService employeeService, ILogger<GetPendingFeedbackMenuItemsCommand> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            _logger.LogInformation("GetPendingFeedbackMenuItems endpoint invoked");
            var response = new ViewMenuItemsResponse();

            try
            {
                var request = JsonConvert.DeserializeObject<PendingFeedbackMenuItemsRequest>(requestData);
                var pendingMenuItems = await _employeeService.GetPendingFeedbackMenuItems(request.UserId);
                response.IsSuccess = true;
                response.MenuItems = pendingMenuItems.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching pending feedback menu items");
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}
