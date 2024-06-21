using CafeteriaServer.Commands.Admin;
using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class GetPastOrdersCommand : ICommand
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<GetPastOrdersCommand> _logger;


        public GetPastOrdersCommand(IEmployeeService employeeService, ILogger<GetPastOrdersCommand> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            _logger.LogInformation("Get past orders endpoint invoked");

            try
            {
                var pastOrderRequest = JsonConvert.DeserializeObject<PastOrderRequest>(requestData);
                var pastOrders = await _employeeService.GetPastOrders(pastOrderRequest.UserId);

                return JsonConvert.SerializeObject(new PastOrdersResponse
                {
                    IsSuccess = true,
                    PastOrders = pastOrders
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                return JsonConvert.SerializeObject(new PastOrdersResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                });
            }
        }
    }
}
