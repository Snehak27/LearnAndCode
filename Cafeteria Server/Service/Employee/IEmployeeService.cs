using CafeteriaServer.DTO;
using System;

namespace CafeteriaServer.Service
{
    public interface IEmployeeService
    {
        Task<bool> ProvideFeedback(FeedbackRequest feedbackRequest);
        Task<EmployeeRecommendationResponse> GetRecommendations();
        Task SaveEmployeeOrder(EmployeeOderRequest employeeOrderRequest);
        Task<List<PastOrderResponse>> GetPastOrders(int userId);
    }
}
