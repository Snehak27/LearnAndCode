using CafeteriaServer.DTO;
using CafeteriaServer.DTO.RequestModel;
using CafeteriaServer.DTO.ResponseModel;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands.Employee
{
    public class GetEmployeePreferenceCommand : ICommand
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<GetEmployeePreferenceCommand> _logger;

        public GetEmployeePreferenceCommand(IEmployeeService employeeService, ILogger<GetEmployeePreferenceCommand> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            _logger.LogInformation("GetEmployeePreference endpoint invoked");
            var response = new EmployeePreferenceResponse();

            try
            {
                var request = JsonConvert.DeserializeObject<EmployeePreferenceRequest>(requestData);
                var preference = await _employeeService.GetEmployeePreference(request.UserId);
                response.IsSuccess = true;
                response.PreferenceResponse = preference;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");

                response.IsSuccess = false;
                response.ErrorMessage = ex.ToString();
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}
