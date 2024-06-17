using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
using CafeteriaServer.UnitofWork;
using System;

namespace CafeteriaServer.Service
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SentimentAnalyzer _sentimentAnalyzer;
        private const int MaxRecommendations = 3;

        public RecommendationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _sentimentAnalyzer = new SentimentAnalyzer();
        }

        public async Task<List<MealTypeRecommendations>> GetRecommendations()
        {
            var mealTypes = await _unitOfWork.MealTypes.GetAll();
            var allFeedbacks = await _unitOfWork.Feedbacks.GetAll();

            var mealTypeRecommendations = new List<MealTypeRecommendations>();

            foreach (var mealType in mealTypes)
            {
                var feedbacks = allFeedbacks.Where(f => f.MealTypeId == mealType.MealTypeId).ToList();

                var feedbackWithSentiments = feedbacks.Select(f => new
                {
                    f.MenuItemId,
                    f.Rating,
                    SentimentScore = _sentimentAnalyzer.AnalyzeSentiment(f.Comment),
                    f.MenuItem.ItemName,
                    f.Comment
                }).ToList();

                var combinedScores = feedbackWithSentiments
                    .GroupBy(f => f.MenuItemId)
                    .Select(g => new
                    {
                        MenuItemId = g.Key,
                        CombinedScore = g.Average(f => f.Rating * 0.7 + f.SentimentScore * 0.3),
                        MenuItemName = g.First().ItemName
                    })
                    .OrderByDescending(x => x.CombinedScore)
                    .Take(MaxRecommendations)
                    .ToList();

                var recommendations = combinedScores
                    .Select(x => new RecommendedItemDTO
                    {
                        MenuItemId = x.MenuItemId,
                        MenuItemName = x.MenuItemName,
                        PredictedRating = x.CombinedScore,
                        Comments = feedbackWithSentiments.Where(f => f.MenuItemId == x.MenuItemId).Select(f => f.Comment).ToList()
                    })
                    .ToList();

                mealTypeRecommendations.Add(new MealTypeRecommendations
                {
                    MealTypeId = mealType.MealTypeId,
                    Recommendations = recommendations
                });
            }

            return mealTypeRecommendations;
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
        }
    }
}
