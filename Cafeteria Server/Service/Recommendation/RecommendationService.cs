using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
using CafeteriaServer.UnitofWork;
using System;

namespace CafeteriaServer.Service
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISentimentAnalyzer _sentimentAnalyzer;

        public RecommendationService(IUnitOfWork unitOfWork, ISentimentAnalyzer sentimentAnalyzer)
        {
            _unitOfWork = unitOfWork;
            _sentimentAnalyzer = sentimentAnalyzer;
        }

        public async Task<List<MealTypeRecommendations>> GetRecommendations()
        {
            var mealTypes = await _unitOfWork.MealTypes.GetAll();
            var allFeedbacks = await _unitOfWork.Feedbacks.GetAll();

            var mealTypeRecommendations = new List<MealTypeRecommendations>();

            foreach (var mealType in mealTypes)
            {
                var feedbacks = allFeedbacks.Where(f => f.OrderItem.RecommendedItem.Recommendation.MealTypeId == mealType.MealTypeId).ToList();

                var feedbackWithSentiments = feedbacks.Select(f => new
                {
                    f.OrderItem.RecommendedItem.MenuItemId,
                    f.Rating,
                    SentimentScore = _sentimentAnalyzer.AnalyzeSentiment(f.Comment),
                    f.OrderItem.RecommendedItem.MenuItem.ItemName,
                    f.Comment,
                    f.FeedbackDate
                }).ToList();

                var combinedScores = feedbackWithSentiments
                    .GroupBy(f => f.MenuItemId)
                    .Select(g => new
                    {
                        MenuItemId = g.Key,
                        CombinedScore = g.Average(f => f.Rating * 0.7 + f.SentimentScore * 0.3),
                        MenuItemName = g.First().ItemName,
                        VoteCount = g.Count(),
                        AverageRating = g.Average(f => f.Rating),
                        AverageSentimentScore = g.Average(f => f.SentimentScore),
                        //OverallSentiment = _sentimentAnalyzer.GetSentimentLabel(g.Average(f => f.SentimentScore)),
                        Comments = g.OrderByDescending(c => c.FeedbackDate)
                                    .Take(2)
                                    .Concat(g.OrderByDescending(c => Math.Abs(c.SentimentScore)).Take(1))
                                    .Distinct()
                                    .Select(c => c.Comment)
                                    .ToList()
                    })
                    .OrderByDescending(x => x.CombinedScore)
                    .ToList();

                var recommendations = combinedScores
                    .Select(x => new RecommendedItemResponse
                    {
                        MenuItemId = x.MenuItemId,
                        MenuItemName = x.MenuItemName,
                        PredictedRating = x.CombinedScore,
                        Comments = x.Comments,
                        VoteCount = x.VoteCount,
                        AverageRating = x.AverageRating,
                        OverallSentiment = _sentimentAnalyzer.GetSentimentLabel(x.AverageSentimentScore)
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
    }
}
