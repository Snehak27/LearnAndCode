using CafeteriaServer.DTO;
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
    }
}
