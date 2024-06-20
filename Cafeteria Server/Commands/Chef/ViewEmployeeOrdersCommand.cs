using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands.Chef
{
    public class ViewEmployeeOrdersCommand : ICommand
    {
        private readonly IChefService _chefService;

        public ViewEmployeeOrdersCommand(IChefService chefService)
        {
            _chefService = chefService;
        }

        public async Task<string> Execute(string requestData)
        {
            var response = new ViewEmployeeOrdersResponse();

            try
            {
                var employeeOrders = await _chefService.GetEmployeeOrders();
                response.IsSuccess = true;
                response.EmployeeOrders = employeeOrders;
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
