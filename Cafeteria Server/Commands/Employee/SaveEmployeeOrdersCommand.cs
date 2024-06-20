using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class SaveEmployeeOrdersCommand : ICommand
    {
        private readonly IEmployeeService _employeeService;

        public SaveEmployeeOrdersCommand(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<string> Execute(string requestData)
        {
            var response = new ResponseMessage();

            try
            {
                var employeeResponseRequest = JsonConvert.DeserializeObject<EmployeeOderRequest>(requestData);
                await _employeeService.SaveEmployeeOrder(employeeResponseRequest);
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}
