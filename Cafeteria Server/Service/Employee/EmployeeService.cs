using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
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
                //MenuItemId = feedbackRequest.MenuItemId,
                UserId = feedbackRequest.UserId,
                Comment = feedbackRequest.Comment,
                Rating = feedbackRequest.Rating,
                FeedbackDate = DateTime.Now,
                OrderItemId = feedbackRequest.OrderItemId,
                //MealTypeId = feedbackRequest.MealTypeId,
            };

            await _unitOfWork.Feedbacks.Add(feedback);
            _unitOfWork.Save();

            return true;
        }

        public async Task<EmployeeRecommendationResponse> GetRecommendations()
        {
            try
            {
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
                            RecommendedItemId = ri.RecommendedItemId
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

                    mealTypeRecommendations.Add(new MealTypeRecommendation
                    {
                        MealTypeId = mealType.MealTypeId,
                        MealTypeName = mealType.MealTypeName,
                        RecommendedItems = items
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
        //var daysLimit = DateTime.Now.AddDays(-3);

        //var pastOrdersRaw = (await _unitOfWork.OrderItems
        //    .FindAll(eri => eri.Order.UserId == userId && eri.Order.OrderDate >= daysLimit))
        //    .ToList();

        //var pastOrders = pastOrdersRaw.Select(eri => new PastOrderResponse
        //{
        //    OrderId = eri.OrderId,
        //    MenuItemId = eri.RecommendedItem.MenuItemId,
        //    MenuItemName = eri.RecommendedItem.MenuItem.ItemName,
        //    OrderDate = eri.Order.OrderDate,
        //    MealTypeId = eri.RecommendedItem.Recommendation.MealTypeId,
        //    OrderItemId = eri.OrderItemId
        //}).ToList();

        //return pastOrders;
    }
}

