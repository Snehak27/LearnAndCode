using CafeteriaServer.DTO;
using System;

namespace CafeteriaServer.Service
{
    public interface IEmployeeService
    {
        Task<bool> ProvideFeedback(FeedbackRequest feedbackRequest);
        Task<EmployeeRecommendationResponse> GetRecommendations();
        Task SaveEmployeeResponse(EmployeeResponseRequest employeeResponseRequest);
        Task<List<PastOrderDTO>> GetPastOrders(int userId);
    }
}
