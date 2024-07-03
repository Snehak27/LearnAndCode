using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
using CafeteriaServer.DTO.ResponseModel;
using System;

namespace CafeteriaServer.Service
{
    public interface IChefService
    {
        Task<List<FeedbackResponse>> GetAllFeedbacks();
        Task<MonthlyFeedbackReportResponse> GetMonthlyFeedbackReport(MonthlyFeedbackReportRequest request);
        Task<List<EmployeeOrderSummary>> GetEmployeeOrders();
        Task SaveFinalMenu(List<MealTypeMenuItemList> mealTypeMenuItems);
        Task<List<MealTypeRecommendations>> GetRecommendations();
        Task<List<DiscardMenuItem>> GetDiscardMenuItems();
        Task<bool> RemoveMenuItem(List<int> menuItemIds);
        Task<bool> RequestDetailedFeedback(List<int> menuItemIds);
        Task<DateTime?> GetLastDiscardDate();
        Task<List<DetailedFeedback>> GetAllDetailedFeedbacks();
    }
}
