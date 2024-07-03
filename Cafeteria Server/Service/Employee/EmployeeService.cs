using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
using CafeteriaServer.DTO.RequestModel;
using CafeteriaServer.DTO.ResponseModel;
using CafeteriaServer.UnitofWork;

namespace CafeteriaServer.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRecommendationService _recommendationService;

        public EmployeeService(IUnitOfWork unitOfWork, IRecommendationService recommendationService)
        {
            _unitOfWork = unitOfWork;
            _recommendationService = recommendationService;
        }

        public async Task<bool> ProvideFeedback(FeedbackRequest feedbackRequest)
        {
            var feedback = new Feedback
            {
                UserId = feedbackRequest.UserId,
                Comment = feedbackRequest.Comment,
                Rating = feedbackRequest.Rating,
                FeedbackDate = DateTime.Now,
                OrderItemId = feedbackRequest.OrderItemId,
            };

            await _unitOfWork.Feedbacks.Add(feedback);
            _unitOfWork.Save();

            return true;
        }

        public async Task<EmployeeRecommendationResponse> GetRecommendations(int userId)
        {
            try
            {
                var preference = await GetEmployeePreference(userId);

                var mealTypes = await _unitOfWork.MealTypes.GetAll();
                var today = DateTime.Now.Date;
                var recommendations = (await _unitOfWork.Recommendations
                    .FindAll(r => r.RecommendationDate.Date == today))
                    .ToList();

                var recentRecommendations = recommendations
                    .GroupBy(r => r.MealTypeId)
                    .Select(g => g.OrderByDescending(r => r.RecommendationDate).First())
                    .ToList();

                var recommendationIds = recentRecommendations.Select(r => r.RecommendationId).ToList();
                var recommendedItems = (await _unitOfWork.RecommendedItems
                    .FindAll(ri => recommendationIds.Contains(ri.RecommendationId)))
                    .ToList();

                var updatedItems = await _recommendationService.GetRecommendations();

                var mealTypeRecommendations = new List<MealTypeRecommendation>();

                foreach (var mealType in mealTypes)
                {
                    var mealTypeRecommendation = recentRecommendations
                        .FirstOrDefault(r => r.MealTypeId == mealType.MealTypeId);

                    if (mealTypeRecommendation == null)
                    {
                        continue; // Skip if no recent recommendation is found
                    }

                    var items = recommendedItems
                        .Where(ri => ri.RecommendationId == mealTypeRecommendation.RecommendationId)
                        .Select(ri => new RecommendedItemResponse
                        {
                            MenuItemId = ri.MenuItemId,
                            MenuItemName = ri.MenuItem.ItemName,
                            PredictedRating = 0,
                            Comments = new List<string>(),
                            OverallSentiment = string.Empty,
                            RecommendedItemId = ri.RecommendedItemId,
                            FoodType = ri.MenuItem.FoodType.FoodTypeName, 
                            SpiceLevel = ri.MenuItem.SpiceLevel.Level, 
                            CuisineType = ri.MenuItem.CuisineType.Cuisine, 
                            IsSweet = ri.MenuItem.IsSweet 
                        })
                        .ToList();

                    foreach (var item in items)
                    {
                        var updatedItem = updatedItems
                            .SelectMany(m => m.Recommendations.Select(r => new { m.MealTypeId, r.MenuItemId, r.PredictedRating, r.Comments, r.OverallSentiment }))
                            .FirstOrDefault(u => u.MenuItemId == item.MenuItemId && u.MealTypeId == mealType.MealTypeId);
                        if (updatedItem != null)
                        {
                            item.PredictedRating = updatedItem.PredictedRating;
                            item.Comments = updatedItem.Comments;
                            item.OverallSentiment = updatedItem.OverallSentiment;
                        }
                    }

                    // Sort the items based on all employee preferences
                    items = items
                        .OrderByDescending(item => item.FoodType == preference.FoodPreference)
                        .ThenBy(item => item.SpiceLevel == preference.SpiceLevel)
                        .ThenBy(item => item.CuisineType == preference.CuisinePreference)
                        .ThenByDescending(item => item.IsSweet == preference.HasSweetTooth)
                        .ToList();

                    var topRecommendedItem = items.FirstOrDefault();

                    mealTypeRecommendations.Add(new MealTypeRecommendation
                    {
                        MealTypeId = mealType.MealTypeId,
                        MealTypeName = mealType.MealTypeName,
                        RecommendedItems = items,
                        TopRecommendedItem = topRecommendedItem
                    });
                }

                return new EmployeeRecommendationResponse
                {
                    IsSuccess = true,
                    MealTypeRecommendations = mealTypeRecommendations
                };
            }
            catch (Exception ex)
            {
                return new EmployeeRecommendationResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"An error occurred while fetching recommendations: {ex.Message}"
                };
            }
        }

        public async Task SaveEmployeeOrder(EmployeeOderRequest employeeOrderRequest)
        {
            var employeeOrder = new Order
            {
                UserId = employeeOrderRequest.UserId,
                OrderDate = DateTime.Now
            };

            await _unitOfWork.Orders.Add(employeeOrder);
            _unitOfWork.Save();

            foreach (var vote in employeeOrderRequest.OrderList)
            {
                var recommendedItem = await _unitOfWork.RecommendedItems.GetById(vote.RecommendedItemId);

                if (recommendedItem != null)
                {
                    var employeeOrderItem = new OrderItem
                    {
                        OrderId = employeeOrder.OrderId,
                        RecommendedItemId = recommendedItem.RecommendedItemId
                    };
                    await _unitOfWork.OrderItems.Add(employeeOrderItem);
                }
            }

            _unitOfWork.Save();
        }

        public async Task<List<PastOrderResponse>> GetPastOrders(int userId)
        {
            var daysLimit = DateTime.Now.AddDays(-3);

            var pastOrdersRaw = (await _unitOfWork.OrderItems
                .FindAll(eri => eri.Order.UserId == userId && eri.Order.OrderDate >= daysLimit))
                .ToList();

            var feedbacks = await _unitOfWork.Feedbacks.FindAll(f => f.UserId == userId);

            var pastOrders = pastOrdersRaw
                .Where(eri => !feedbacks.Any(f => f.OrderItemId == eri.OrderItemId))
                .Select(eri => new PastOrderResponse
                {
                    OrderId = eri.OrderId,
                    MenuItemId = eri.RecommendedItem.MenuItemId,
                    MenuItemName = eri.RecommendedItem.MenuItem.ItemName,
                    OrderDate = eri.Order.OrderDate,
                    MealTypeId = eri.RecommendedItem.Recommendation.MealTypeId,
                    OrderItemId = eri.OrderItemId
                })
                .ToList();

            return pastOrders;
        }

        public async Task<IEnumerable<FoodType>> GetAllFoodPreferences()
        {
            return await _unitOfWork.FoodPreferences.GetAll();
        }

        public async Task<IEnumerable<SpiceLevel>> GetAllSpiceLevels()
        {
            return await _unitOfWork.SpiceLevels.GetAll();
        }

        public async Task<IEnumerable<CuisineType>> GetAllCuisinePreferences()
        {
            return await _unitOfWork.CuisinePreferences.GetAll();
        }

        public async Task<bool> UpdateEmployeePreference(UpdateProfileRequest request)
        {
            var employeePreference = await _unitOfWork.EmployeePreferences.Find(u => u.UserId == request.UserId);

            if (employeePreference == null)
            {
                employeePreference = new EmployeePreference
                {
                    UserId = request.UserId,
                    FoodTypeId = request.FoodTypeId,
                    SpiceLevelId = request.SpiceLevelId,
                    CuisineTypeId = request.CuisineTypeId,
                    HasSweetTooth = request.HasSweetTooth
                };
                await _unitOfWork.EmployeePreferences.Add(employeePreference);
            }
            else
            {
                employeePreference.FoodTypeId = request.FoodTypeId;
                employeePreference.SpiceLevelId = request.SpiceLevelId;
                employeePreference.CuisineTypeId = request.CuisineTypeId;
                employeePreference.HasSweetTooth = request.HasSweetTooth;

                _unitOfWork.EmployeePreferences.Update(employeePreference);
            }

            _unitOfWork.Save();
            return true;
        }

        public async Task<PreferenceResponse> GetEmployeePreference(int userId)
        {
            var preference = await _unitOfWork.EmployeePreferences.Find(e => e.UserId == userId);
            if (preference != null)
            {
                return new PreferenceResponse
                {
                    FoodPreference = preference.FoodType.FoodTypeName,
                    SpiceLevel = preference.SpiceLevel.Level,
                    CuisinePreference = preference.CuisineType.Cuisine,
                    HasSweetTooth = preference.HasSweetTooth
                };
            }
            else
            {
                throw new Exception("Employee Preference not found");
            }
        }

        public async Task<bool> SubmitDetailedFeedback(DetailedFeedbackRequest request)
        {
            var feedback = new DetailedFeedback
            {
                UserId = request.UserId,
                MenuItemId = request.MenuItemId,
                DislikeReason = request.Answers[0],
                PreferredTaste = request.Answers[1],
                Recipe = request.Answers[2],
                FeedbackDate = DateTime.Now
            };

            await _unitOfWork.DetailedFeedbacks.Add(feedback);
            _unitOfWork.Save();
            return true;
        }

        public async Task<List<MenuItem>> GetPendingFeedbackMenuItems(int userId)
        {
            var notificationTypeId = 4;
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var notifications = await GetUserNotifications(userId, notificationTypeId, startOfMonth);
            var pendingFeedbackItems = notifications.Select(n => n.MenuItemId.GetValueOrDefault()).Distinct().ToList();
            var providedFeedbackItemIds = await GetProvidedFeedbackMenuItemIds(userId, pendingFeedbackItems);
            var pendingMenuItemIds = pendingFeedbackItems.Except(providedFeedbackItemIds).ToList();

            var pendingMenuItems = (await _unitOfWork.MenuItems
                .FindAll(m => pendingMenuItemIds.Contains(m.MenuItemId)))
                .ToList();

            return pendingMenuItems;
        }

        private async Task<List<UserNotification>> GetUserNotifications(int userId, int notificationTypeId, DateTime startOfMonth)
        {
            var notifications = (await _unitOfWork.UserNotifications
                .FindAll(n => n.UserId == userId && n.NotificationTypeId == notificationTypeId && n.CreatedAt >= startOfMonth))
                .Where(n => n.MenuItemId.HasValue)
                .ToList();

            return notifications;
        }

        private async Task<List<int>> GetProvidedFeedbackMenuItemIds(int userId, List<int> menuItemIds)
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var providedFeedbacks = (await _unitOfWork.DetailedFeedbacks
                .FindAll(f => f.UserId == userId && menuItemIds.Contains(f.MenuItemId) && f.FeedbackDate >= startOfMonth))
                .Select(f => f.MenuItemId)
                .ToList();

            return providedFeedbacks;
        }
    }
}
