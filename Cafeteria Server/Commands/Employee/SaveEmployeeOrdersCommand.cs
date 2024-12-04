using CafeteriaServer.Commands.Admin;
using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class SaveEmployeeOrdersCommand : ICommand
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<SaveEmployeeOrdersCommand> _logger;


        public SaveEmployeeOrdersCommand(IEmployeeService employeeService, ILogger<SaveEmployeeOrdersCommand> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            _logger.LogInformation("Save employee orders endpoint invoked");

            var response = new ResponseMessage();

            try
            {
                var employeeResponseRequest = JsonConvert.DeserializeObject<EmployeeOderRequest>(requestData);
                await _employeeService.SaveEmployeeOrders(employeeResponseRequest);
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
