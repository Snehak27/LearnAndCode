using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class GetPastOrdersCommand : ICommand
    {
        private readonly IEmployeeService _employeeService;

        public GetPastOrdersCommand(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<string> Execute(string requestData)
        {
            try
            {
                var pastOrderRequest = JsonConvert.DeserializeObject<PastOrderRequest>(requestData);
                var pastOrders = await _employeeService.GetPastOrders(pastOrderRequest.UserId);

                return JsonConvert.SerializeObject(new PastOrdersResponse
                {
                    IsSuccess = true,
                    PastOrders = pastOrders
                });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new PastOrdersResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                });
            }
        }
    }
}
