using CafeteriaServer.Commands.Admin;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class GetEmployeeRecommendationsCommand : ICommand
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<GetEmployeeRecommendationsCommand> _logger;


        public GetEmployeeRecommendationsCommand(IEmployeeService employeeService, ILogger<GetEmployeeRecommendationsCommand> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            _logger.LogInformation("Get employee recommendations endpoint invoked");

            var response = await _employeeService.GetRecommendations();
            return JsonConvert.SerializeObject(response);
        }
    }
}
