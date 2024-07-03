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

        public ChefService(IUnitOfWork unitOfWork, IRecommendationService recommendationService)
        {
            _unitOfWork = unitOfWork;
            _recommendationService = recommendationService;
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
                        .Take(3) 
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

        //public async Task<List<DiscardMenuItem>> GetDiscardMenuItems()
        //{
        //    var menuItems = await _unitOfWork.MenuItems.GetAll();
        //    var feedbacks = await _unitOfWork.Feedbacks.GetAll();

        //    var discardItems = menuItems
        //        .Select(item => new DiscardMenuItem
        //        {
        //            MenuItemId = item.MenuItemId,
        //            MenuItemName = item.ItemName,
        //            AverageRating = feedbacks.Where(f => f.OrderItem.RecommendedItem.MenuItemId == item.MenuItemId).Average(f => f.Rating),
        //            Sentiments = feedbacks.Where(f => f.OrderItem.RecommendedItem.MenuItemId == item.MenuItemId).Select(f => f.Comment).ToList()
        //        })
        //        .Where(item => item.AverageRating < 2 || item.Sentiments.Any(s => s.Contains("bad") || s.Contains("poor") || s.Contains("tasteless")))
        //        .ToList();

        //    return discardItems;
        //}
        public async Task<List<DiscardMenuItem>> GetDiscardMenuItems()
        {
            var allRecommendations = await _recommendationService.GetRecommendations();

            var filteredRecommendations = allRecommendations
                .SelectMany(r => r.Recommendations)
                .GroupBy(r => r.MenuItemId)
                .Select(g => new
                {
                    MenuItemId = g.Key,
                    MenuItemName = g.First().MenuItemName,
                    AverageRating = g.Average(r => r.AverageRating),
                    Comments = g.SelectMany(r => r.Comments).ToList()
                })
                .ToList();

            var discardItems = filteredRecommendations
                .Where(item => item.AverageRating < 2)
                .Select(item => new DiscardMenuItem
                {
                    MenuItemId = item.MenuItemId,
                    MenuItemName = item.MenuItemName,
                    AverageRating = item.AverageRating,
                    Sentiments = item.Comments
                })
                .ToList();

            return discardItems;
        }

        public async Task<bool> RemoveMenuItem(List<int> menuItemIds)
        {
            foreach (var menuItemId in menuItemIds)
            {
                var menuItem = await _unitOfWork.MenuItems.GetById(menuItemId);
                if (menuItem != null)
                {
                    //menuItem.IsDeleted = true;
                    _unitOfWork.MenuItems.Delete(menuItem);

                    // Log the discarded menu item
                    await AddDiscardedMenuItem(menuItem.ItemName, DateTime.Now);
                }
            }
            _unitOfWork.Save();
            return true;
        }

        private async Task AddDiscardedMenuItem(string menuItemName, DateTime discardDate)
        {
            var discardedMenuItem = new DiscardedMenuItem
            {
                MenuItemName = menuItemName,
                DiscardDate = discardDate
            };
            await _unitOfWork.DiscardedMenuItems.Add(discardedMenuItem);
            _unitOfWork.Save();
        }

        public async Task<bool> RequestDetailedFeedback(List<int> menuItemIds)
        {
            foreach (var menuItemId in menuItemIds)
            {
                var users = await _unitOfWork.Users.FindAll(u => u.RoleId == 3);
                var menuItem = await _unitOfWork.MenuItems.GetById(menuItemId);

                if (menuItem == null)
                {
                    return false;
                }

                foreach (var user in users)
                {
                    var userNotification = new UserNotification
                    {
                        UserId = user.UserId,
                        NotificationTypeId = 4,
                        IsRead = false,
                        CreatedAt = DateTime.Now,
                        MenuItemId = menuItemId,
                        //NotificationMessage = $"We are trying to improve your experience with {menuItem.ItemName}. Please provide your feedback."
                    };
                    await _unitOfWork.UserNotifications.Add(userNotification);
                }

                _unitOfWork.Save();
            }
            return true;
        }

        public async Task<DateTime?> GetLastDiscardDate()
        {
            var logs = await _unitOfWork.DiscardedMenuItems.GetAll();
            var log = logs.OrderByDescending(l => l.DiscardDate).FirstOrDefault();
            return log?.DiscardDate;
        }

        public async Task<List<DetailedFeedback>> GetAllDetailedFeedbacks()
        {
            return (await _unitOfWork.DetailedFeedbacks.GetAll()).ToList();
        }
    }
}
