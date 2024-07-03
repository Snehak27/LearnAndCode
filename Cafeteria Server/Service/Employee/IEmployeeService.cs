using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
using CafeteriaServer.DTO.RequestModel;
using CafeteriaServer.DTO.ResponseModel;
using System;

namespace CafeteriaServer.Service
{
    public interface IEmployeeService
    {
        Task<bool> ProvideFeedback(FeedbackRequest feedbackRequest);
        Task<EmployeeRecommendationResponse> GetRecommendations(int userId);
        Task SaveEmployeeOrder(EmployeeOderRequest employeeOrderRequest);
        Task<List<PastOrderResponse>> GetPastOrders(int userId);
        Task<bool> UpdateEmployeePreference(UpdateProfileRequest request);
        Task<PreferenceResponse> GetEmployeePreference(int userId);
        Task<bool> SubmitDetailedFeedback(DetailedFeedbackRequest request);
        Task<List<MenuItem>> GetPendingFeedbackMenuItems(int userId);
    }
}
