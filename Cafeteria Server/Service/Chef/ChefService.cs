using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
using CafeteriaServer.UnitofWork;
using System;

namespace CafeteriaServer.Service
{
    public class ChefService : IChefService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRecommendationService _recommendationService;

        public ChefService(IUnitOfWork unitOfWork, IRecommendationService recommendationService)
        {
            _unitOfWork = unitOfWork;
            _recommendationService = recommendationService;
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

        public async Task<List<EmployeeResponseSummary>> GetEmployeeResponses()
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

            var employeeResponseSummaries = new List<EmployeeResponseSummary>();

            foreach (var mealType in mealTypes)
            {
                var votes = orderItems
                    .Where(eri => recommendedItems.Any(ri => ri.RecommendedItemId == eri.RecommendedItemId && ri.Recommendation.MealTypeId == mealType.MealTypeId))
                    .GroupBy(eri => eri.RecommendedItem.MenuItemId)
                    .Select(g => new MenuItemVote
                    {
                        MenuItemId = g.Key,
                        MenuItemName = g.First().RecommendedItem.MenuItem.ItemName,
                        VoteCount = g.Count()
                    })
                    .OrderByDescending(miv => miv.VoteCount)
                    .ToList();

                employeeResponseSummaries.Add(new EmployeeResponseSummary
                {
                    MealTypeId = mealType.MealTypeId,
                    MealTypeName = mealType.MealTypeName,
                    MenuItemVotes = votes
                });
            }

            return employeeResponseSummaries;
        }

        //public async Task<List<MealTypeRecommendations>> GetRecommendations()
        //{
        //    var allRecommendations = await _recommendationService.GetRecommendations();
        //    var topRecommendations = new List<MealTypeRecommendations>();

        //    foreach (var mealTypeRecommendation in allRecommendations)
        //    {
        //        var topMealTypeRecommendation = new MealTypeRecommendations
        //        {
        //            MealTypeId = mealTypeRecommendation.MealTypeId,
        //            Recommendations = mealTypeRecommendation.Recommendations
        //                .OrderByDescending(r => r.PredictedRating)
        //                .Take(3)
        //                .ToList()
        //        };
        //        topRecommendations.Add(topMealTypeRecommendation);
        //    }
        //    return topRecommendations;
        //    //var mealTypeRecommendations = await _recommendationService.GetRecommendations();

        //    //return mealTypeRecommendations;
        //}
        public async Task<List<MealTypeRecommendations>> GetRecommendations()
        {
            // Fetch all recommendations
            var allRecommendations = await _recommendationService.GetRecommendations();
            var topRecommendations = new List<MealTypeRecommendations>();

            foreach (var mealTypeRecommendation in allRecommendations)
            {
                // Create a new MealTypeRecommendations object for each meal type
                var topMealTypeRecommendation = new MealTypeRecommendations
                {
                    MealTypeId = mealTypeRecommendation.MealTypeId,
                    Recommendations = mealTypeRecommendation.Recommendations
                        .OrderByDescending(r => r.PredictedRating)
                        .Take(3) // Take the top 3 recommendations
                        .ToList()
                };

                // Add to the topRecommendations list
                topRecommendations.Add(topMealTypeRecommendation);
            }

            // Return the top recommendations
            return topRecommendations;
        }
        public async Task SaveFinalMenuAsync(List<MealTypeMenuItem> mealTypeMenuItems)
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

            // Add notifications for all users
            var users = await _unitOfWork.Users.FindAll(u => u.RoleId == 3);
            foreach (var user in users)
            {
                var userNotification = new UserNotification
                {
                    UserId = user.UserId,
                    NotificationTypeId = 3,
                    IsRead = false,
                    CreatedAt = DateTime.Now,
                    MenuItemId = null,
                };
                await _unitOfWork.UserNotifications.Add(userNotification);
            }

            _unitOfWork.Save();
        }
    }
}
