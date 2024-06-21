using CafeteriaServer.Commands.Admin;
using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands.Chef
{
    public class GetEmployeeOrdersCommand : ICommand
    {
        private readonly IChefService _chefService;
        private readonly ILogger<GetEmployeeOrdersCommand> _logger;


        public GetEmployeeOrdersCommand(IChefService chefService, ILogger<GetEmployeeOrdersCommand> logger)
        {
            _chefService = chefService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            _logger.LogInformation("Get employee orders endpoint invoked");

            var response = new ViewEmployeeOrdersResponse();

            try
            {
                var employeeOrders = await _chefService.GetEmployeeOrders();
                response.IsSuccess = true;
                response.EmployeeOrders = employeeOrders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                response.IsSuccess = false;
                response.ErrorMessage = $"An error occurred while fetching employee responses: {ex.Message}";
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}
