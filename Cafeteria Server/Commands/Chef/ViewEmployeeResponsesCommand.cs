using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands.Chef
{
    public class ViewEmployeeResponsesCommand : ICommand
    {
        private readonly IChefService _chefService;

        public ViewEmployeeResponsesCommand(IChefService chefService)
        {
            _chefService = chefService;
        }

        public async Task<string> Execute(string requestData)
        {
            var response = new ViewEmployeeResponseResponse();

            try
            {
                var employeeResponses = await _chefService.GetEmployeeResponses();
                response.IsSuccess = true;
                response.EmployeeResponses = employeeResponses;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = $"An error occurred while fetching employee responses: {ex.Message}";
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}
