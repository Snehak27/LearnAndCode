using CafeteriaServer.Service;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class EmployeeRecommendationCommand : ICommand
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeRecommendationCommand(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<string> Execute(string requestData)
        {
            var response = await _employeeService.GetRecommendations();
            return JsonConvert.SerializeObject(response);
        }
    }
}
