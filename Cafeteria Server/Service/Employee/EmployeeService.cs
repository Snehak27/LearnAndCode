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
                MenuItemId = feedbackRequest.MenuItemId,
                UserId = feedbackRequest.UserId,
                Comment = feedbackRequest.Comment,
                Rating = feedbackRequest.Rating,
                FeedbackDate = DateTime.UtcNow,
                MealTypeId = feedbackRequest.MealTypeId,
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
                var today = DateTime.UtcNow.Date;
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
                        .Select(ri => new RecommendedItemDTO
                        {
                            MenuItemId = ri.MenuItemId,
                            MenuItemName = ri.MenuItem.ItemName,
                            PredictedRating = 0, // Replace with actual logic if available
                            Comments = new List<string>(), // Replace with actual logic if available
                            OverallSentiment = string.Empty // Placeholder for sentiment
                        })
                        .ToList();

                    // Update PredictedRating, Comments, and OverallSentiment
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

        public async Task SaveEmployeeResponse(EmployeeResponseRequest employeeResponseRequest)
        {
            var employeeResponse = new Order
            {
                UserId = employeeResponseRequest.UserId,
                OrderDate = DateTime.UtcNow
            };

            await _unitOfWork.Orders.Add(employeeResponse);
            _unitOfWork.Save();

            foreach (var vote in employeeResponseRequest.Votes)
            {
                var recommendedItem = await _unitOfWork.RecommendedItems.GetById(vote.RecommendedItemId);

                if (recommendedItem != null)
                {
                    var employeeResponseItem = new OrderItem
                    {
                        OrderId = employeeResponse.OrderId,
                        RecommendedItemId = recommendedItem.RecommendedItemId
                    };
                    await _unitOfWork.OrderItems.Add(employeeResponseItem);
                }
            }

            _unitOfWork.Save();
        }

        public async Task<List<PastOrderDTO>> GetPastOrders(int userId)
        {
            var oneWeekAgo = DateTime.UtcNow.AddDays(-3);

            // Retrieve the past orders from the last week for the specified user
            var pastOrdersRaw = (await _unitOfWork.OrderItems
                .FindAll(eri => eri.Order.UserId == userId && eri.Order.OrderDate >= oneWeekAgo))
                .ToList();

            // Project the retrieved data into PastOrderDTO objects
            var pastOrders = pastOrdersRaw.Select(eri => new PastOrderDTO
            {
                OrderId = eri.OrderId,
                MenuItemId = eri.RecommendedItem.MenuItemId,
                MenuItemName = eri.RecommendedItem.MenuItem.ItemName,
                OrderDate = eri.Order.OrderDate,
                MealTypeId = eri.RecommendedItem.Recommendation.MealTypeId
            }).ToList();

            return pastOrders;
        }
    }
}
