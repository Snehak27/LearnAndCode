using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
using CafeteriaServer.DTO.ResponseModel;
using CafeteriaServer.UnitofWork;
using System;

namespace CafeteriaServer.Service
{
    public class ChefService : IChefService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRecommendationService _recommendationService;
        private readonly INotificationService _notificationService;

        public ChefService(IUnitOfWork unitOfWork, IRecommendationService recommendationService, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _recommendationService = recommendationService;
            _notificationService = notificationService;
        }

        public async Task<List<FeedbackResponse>> GetAllFeedbacks()
        {
            var feedbacks = await _unitOfWork.Feedbacks.GetAll();
            var feedbacksResponse = new List<FeedbackResponse>();

            foreach (var feedback in feedbacks)
            {
                feedbacksResponse.Add(new FeedbackResponse
                {
                    Comment = feedback.Comment,
                    Rating = feedback.Rating,
                    MenuItemName = feedback.OrderItem.RecommendedItem.MenuItem.ItemName
                });
            }

            return feedbacksResponse;
        }

        public async Task<MonthlyFeedbackReportResponse> GetMonthlyFeedbackReport(MonthlyFeedbackReportRequest request)
        {
            var feedbacks = await _unitOfWork.Feedbacks.GetAll();

            var monthlyFeedbacks = feedbacks
                .Where(f => f.FeedbackDate.Year == request.Year && f.FeedbackDate.Month == request.Month)
                .GroupBy(f => f.OrderItem.RecommendedItem.MenuItem.ItemName)
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

        public async Task<List<EmployeeOrderSummary>> GetEmployeeOrders()
        {
            var mealTypes = await _unitOfWork.MealTypes.GetAll();
            var orderItems = await _unitOfWork.OrderItems.GetAll();
            var recommendations = await _unitOfWork.Recommendations.GetAll();

            // Get the most recent recommendation for each meal type
            var recentRecommendations = recommendations
                .GroupBy(r => r.MealTypeId)
                .Select(g => g.OrderByDescending(r => r.RecommendationDate).First())
                .ToList();

            var recommendationIds = recentRecommendations.Select(r => r.RecommendationId).ToList();
            var recommendedItems = (await _unitOfWork.RecommendedItems
                .FindAll(ri => recommendationIds.Contains(ri.RecommendationId)))
                .ToList();

            var employeeResponseSummaries = new List<EmployeeOrderSummary>();

            foreach (var mealType in mealTypes)
            {
                var orders = orderItems
                    .Where(eri => recommendedItems.Any(ri => ri.RecommendedItemId == eri.RecommendedItemId && ri.Recommendation.MealTypeId == mealType.MealTypeId))
                    .GroupBy(eri => eri.RecommendedItem.MenuItemId)
                    .Select(g => new MenuItemOrder
                    {
                        MenuItemId = g.Key,
                        MenuItemName = g.First().RecommendedItem.MenuItem.ItemName,
                        OrderCount = g.Count()
                    })
                    .OrderByDescending(miv => miv.OrderCount)
                    .ToList();

                employeeResponseSummaries.Add(new EmployeeOrderSummary
                {
                    MealTypeId = mealType.MealTypeId,
                    MealTypeName = mealType.MealTypeName,
                    MenuItemOrders = orders
                });
            }

            return employeeResponseSummaries;
        }

        public async Task<List<MealTypeRecommendations>> GetRecommendations()
        {
            var allRecommendations = await _recommendationService.GetRecommendations();
            var topRecommendations = new List<MealTypeRecommendations>();

            foreach (var mealTypeRecommendation in allRecommendations)
            {
                var topMealTypeRecommendation = new MealTypeRecommendations
                {
                    MealTypeId = mealTypeRecommendation.MealTypeId,
                    Recommendations = mealTypeRecommendation.Recommendations
                        .OrderByDescending(r => r.PredictedRating)
                        .Take(4) 
                        .ToList()
                };

                topRecommendations.Add(topMealTypeRecommendation);
            }

            return topRecommendations;
        }

        public async Task SaveFinalMenu(List<MealTypeMenuItemList> mealTypeMenuItems)
        {
            foreach (var mealTypeMenuItem in mealTypeMenuItems)
            {
                var recommendation = new Recommendation
                {
                    MealTypeId = mealTypeMenuItem.MealTypeId,
                    RecommendationDate = DateTime.Now
                };

                await _unitOfWork.Recommendations.Add(recommendation);
                _unitOfWork.Save();

                foreach (var menuItemId in mealTypeMenuItem.MenuItemIds)
                {
                    var recommendedItem = new RecommendedItem
                    {
                        RecommendationId = recommendation.RecommendationId,
                        MenuItemId = menuItemId
                    };

                    await _unitOfWork.RecommendedItems.Add(recommendedItem);
                }
            }
            _unitOfWork.Save();

            await _notificationService.NotifyEmployees(notificationTypeId: 3);
        }
    }
}
