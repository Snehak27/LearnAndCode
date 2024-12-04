using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class SaveEmployeeResponseCommand : ICommand
    {
        private readonly IEmployeeService _employeeService;

        public SaveEmployeeResponseCommand(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<string> Execute(string requestData)
        {
            var response = new ResponseMessage();

            try
            {
                var employeeResponseRequest = JsonConvert.DeserializeObject<EmployeeResponseRequest>(requestData);
                await _employeeService.SaveEmployeeResponse(employeeResponseRequest);
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
