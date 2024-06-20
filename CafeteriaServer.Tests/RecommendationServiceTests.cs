using CafeteriaServer.DAL.Models;
using CafeteriaServer.Service;
using CafeteriaServer.UnitofWork;
using Moq;

namespace CafeteriaServer.Tests
{
    public class RecommendationServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ISentimentAnalyzer> _mockSentimentAnalyzer;
        private readonly RecommendationService _recommendationService;

        public RecommendationServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockSentimentAnalyzer = new Mock<ISentimentAnalyzer>();
            _recommendationService = new RecommendationService(_mockUnitOfWork.Object, _mockSentimentAnalyzer.Object);
        }

        [Fact]
        public async Task GetRecommendations_ReturnsCorrectData()
        {
            // Arrange
            var mealTypes = new List<MealType>
                {
                    new MealType { MealTypeId = 1, MealTypeName = "Breakfast" },
                    new MealType { MealTypeId = 2, MealTypeName = "Lunch" }
                };

            var feedbacks = new List<Feedback>
            {
                new Feedback { Rating = 4, Comment = "Great!", FeedbackDate = DateTime.UtcNow.AddDays(-1), OrderItem = new OrderItem { RecommendedItem = new RecommendedItem { MenuItemId = 1, MenuItem = new MenuItem { MenuItemId = 1, ItemName = "Pancakes" }, Recommendation = new Recommendation { MealTypeId = 1 } } } },
                new Feedback { Rating = 5, Comment = "Excellent!", FeedbackDate = DateTime.UtcNow, OrderItem = new OrderItem { RecommendedItem = new RecommendedItem { MenuItemId = 1, MenuItem = new MenuItem { MenuItemId = 1, ItemName = "Pancakes" }, Recommendation = new Recommendation { MealTypeId = 1 } } } },
                new Feedback { Rating = 3, Comment = "Good", FeedbackDate = DateTime.UtcNow.AddDays(-2), OrderItem = new OrderItem { RecommendedItem = new RecommendedItem { MenuItemId = 2, MenuItem = new MenuItem { MenuItemId = 2, ItemName = "Burger" }, Recommendation = new Recommendation { MealTypeId = 2 } } } }
            };

            _mockUnitOfWork.Setup(u => u.MealTypes.GetAll()).ReturnsAsync(mealTypes);
            _mockUnitOfWork.Setup(u => u.Feedbacks.GetAll()).ReturnsAsync(feedbacks);

            _mockSentimentAnalyzer.Setup(s => s.AnalyzeSentiment("Great!")).Returns(0.8);
            _mockSentimentAnalyzer.Setup(s => s.AnalyzeSentiment("Excellent!")).Returns(1.0);
            _mockSentimentAnalyzer.Setup(s => s.AnalyzeSentiment("Good")).Returns(0.5);

            _mockSentimentAnalyzer.Setup(s => s.GetSentimentLabel(0.9)).Returns("Positive");
            _mockSentimentAnalyzer.Setup(s => s.GetSentimentLabel(1.0)).Returns("Positive");
            _mockSentimentAnalyzer.Setup(s => s.GetSentimentLabel(0.5)).Returns("Neutral");

            // Act
            var result = await _recommendationService.GetRecommendations();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            var breakfastRecommendations = result.First(r => r.MealTypeId == 1).Recommendations;
            var lunchRecommendations = result.First(r => r.MealTypeId == 2).Recommendations;

            //Assert.Single(breakfastRecommendations);
            //Assert.Single(lunchRecommendations);

            var breakfastItem = breakfastRecommendations.First();
            var lunchItem = lunchRecommendations.First();

            Assert.Equal(1, breakfastItem.MenuItemId);
            Assert.Equal("Pancakes", breakfastItem.MenuItemName);
            Assert.Equal(3.42, breakfastItem.PredictedRating, 2);
            Assert.Equal(2, breakfastItem.VoteCount);
            Assert.Equal(4.5, breakfastItem.AverageRating, 1);
            Assert.Equal("Positive", breakfastItem.OverallSentiment);

            Assert.Equal(2, lunchItem.MenuItemId);
            Assert.Equal("Burger", lunchItem.MenuItemName);
            Assert.Equal(2.2499999999999996, lunchItem.PredictedRating, 1);
            Assert.Equal(1, lunchItem.VoteCount);
            Assert.Equal(3, lunchItem.AverageRating, 1);
            Assert.Equal("Neutral", lunchItem.OverallSentiment);
        }
    }
}
