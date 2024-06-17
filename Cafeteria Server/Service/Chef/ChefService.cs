using CafeteriaServer.DTO;
using CafeteriaServer.UnitofWork;
using System;

namespace CafeteriaServer.Service
{
    public class ChefService : IChefService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChefService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<FeedbackDTO>> GetAllFeedbacks()
        {
            var feedbacks = await _unitOfWork.Feedbacks.GetAll();
            var feedbackDTOs = new List<FeedbackDTO>();

            foreach (var feedback in feedbacks)
            {
                feedbackDTOs.Add(new FeedbackDTO
                {
                    Comment = feedback.Comment,
                    Rating = feedback.Rating,
                    MenuItemName = feedback.MenuItem.ItemName
                });
            }

            return feedbackDTOs;
        }

        public async Task<MonthlyFeedbackReportResponse> GetMonthlyFeedbackReportAsync(MonthlyFeedbackReportRequest request)
        {
            var feedbacks = await _unitOfWork.Feedbacks.GetAll();

            var monthlyFeedbacks = feedbacks
                .Where(f => f.FeedbackDate.Year == request.Year && f.FeedbackDate.Month == request.Month)
                .GroupBy(f => f.MenuItem.ItemName)
                .Select(g => new FeedbackSummary
                {
                    MenuItemName = g.Key,
                    AverageRating = g.Average(f => f.Rating),
                    FeedbackCount = g.Count(),
                    Comments = g.Select(f => f.Comment).ToList()
                })
                .ToList();

            var report = new MonthlyFeedbackReport
            {
                Year = request.Year,
                Month = request.Month,
                FeedbackSummaries = monthlyFeedbacks
            };

            return new MonthlyFeedbackReportResponse
            {
                IsSuccess = true,
                Report = report
            };
        }
    }
}
