using CafeteriaServer.DTO;
using System;

namespace CafeteriaServer.Service
{
    public interface IChefService
    {
        Task<List<FeedbackDTO>> GetAllFeedbacks();
        Task<MonthlyFeedbackReportResponse> GetMonthlyFeedbackReportAsync(MonthlyFeedbackReportRequest request);
    }
}
