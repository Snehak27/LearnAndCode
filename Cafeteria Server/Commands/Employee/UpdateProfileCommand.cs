using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO.RequestModel;
using CafeteriaServer.DTO.ResponseModel;
using CafeteriaServer.Service;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands.Employee
{
    public class UpdateProfileCommand : ICommand
    {
        private readonly IEmployeeService _employeeService;

        public UpdateProfileCommand(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<string> Execute(string requestData)
        {
            var response = new UpdateProfileResponse();

            try
            {
                var request = JsonConvert.DeserializeObject<UpdateProfileRequest>(requestData);

                var isUpdated = await _employeeService.UpdateEmployeePreference(request);

                response.IsSuccess = isUpdated;
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
