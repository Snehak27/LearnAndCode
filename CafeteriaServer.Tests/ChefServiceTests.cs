using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using CafeteriaServer.UnitofWork;
using Moq;
using System;
using System.Linq.Expressions;

namespace CafeteriaServer.Tests
{
    public class ChefServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRecommendationService> _mockRecommendationService;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly ChefService _chefService;

        public ChefServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRecommendationService = new Mock<IRecommendationService>();
            _mockNotificationService = new Mock<INotificationService>();
            _chefService = new ChefService(_mockUnitOfWork.Object, _mockRecommendationService.Object, _mockNotificationService.Object);
        }

        [Fact]
        public async Task GetAllFeedbacks_ReturnsFeedbackResponses()
        {
            // Arrange
            var feedbacks = new List<Feedback>
            {
                new Feedback { Comment = "Good", Rating = 4, OrderItem = new OrderItem { RecommendedItem = new RecommendedItem { MenuItem = new MenuItem { ItemName = "Pizza" } } } },
                new Feedback { Comment = "Bad", Rating = 1, OrderItem = new OrderItem { RecommendedItem = new RecommendedItem { MenuItem = new MenuItem { ItemName = "Burger" } } } }
            };

            _mockUnitOfWork.Setup(u => u.Feedbacks.GetAll()).ReturnsAsync(feedbacks);

            // Act
            var result = await _chefService.GetAllFeedbacks();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Pizza", result[0].MenuItemName);
            Assert.Equal("Good", result[0].Comment);
            Assert.Equal(4, result[0].Rating);
            Assert.Equal("Burger", result[1].MenuItemName);
            Assert.Equal("Bad", result[1].Comment);
            Assert.Equal(1, result[1].Rating);
        }

        [Fact]
        public async Task GetMonthlyFeedbackReport_ReturnsCorrectData()
        {
            // Arrange
            var request = new MonthlyFeedbackReportRequest { Year = 2023, Month = 7 };
            var feedbacks = new List<Feedback>
            {
                new Feedback { Comment = "Good", Rating = 4, FeedbackDate = new DateTime(2023, 7, 1), OrderItem = new OrderItem { RecommendedItem = new RecommendedItem { MenuItem = new MenuItem { ItemName = "Pizza" } } } },
                new Feedback { Comment = "Bad", Rating = 1, FeedbackDate = new DateTime(2023, 7, 2), OrderItem = new OrderItem { RecommendedItem = new RecommendedItem { MenuItem = new MenuItem { ItemName = "Burger" } } } }
            };

            _mockUnitOfWork.Setup(u => u.Feedbacks.GetAll()).ReturnsAsync(feedbacks);

            // Act
            var result = await _chefService.GetMonthlyFeedbackReport(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Report);
            Assert.Equal(2, result.Report.FeedbackSummaries.Count);
            Assert.Equal("Pizza", result.Report.FeedbackSummaries[0].MenuItemName);
            Assert.Equal(4, result.Report.FeedbackSummaries[0].AverageRating);
            Assert.Equal("Burger", result.Report.FeedbackSummaries[1].MenuItemName);
            Assert.Equal(1, result.Report.FeedbackSummaries[1].AverageRating);
        }


        [Fact]
        public async Task GetRecommendations_ReturnsTopRecommendations()
        {
            // Arrange
            var recommendations = new List<MealTypeRecommendations>
        {
            new MealTypeRecommendations
            {
                MealTypeId = 1,
                Recommendations = new List<RecommendedItemResponse>
                {
                    new RecommendedItemResponse { MenuItemId = 1, MenuItemName = "Pizza", PredictedRating = 4.5 },
                    new RecommendedItemResponse { MenuItemId = 2, MenuItemName = "Burger", PredictedRating = 4.0 }
                }
            },
            new MealTypeRecommendations
            {
                MealTypeId = 2,
                Recommendations = new List<RecommendedItemResponse>
                {
                    new RecommendedItemResponse { MenuItemId = 3, MenuItemName = "Pasta", PredictedRating = 3.5 }
                }
            }
        };

            _mockRecommendationService.Setup(r => r.GetRecommendations()).ReturnsAsync(recommendations);

            // Act
            var result = await _chefService.GetRecommendations();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            var breakfastRecommendations = result.FirstOrDefault(r => r.MealTypeId == 1);
            Assert.NotNull(breakfastRecommendations);
            Assert.Equal(2, breakfastRecommendations.Recommendations.Count);
            Assert.Equal("Pizza", breakfastRecommendations.Recommendations[0].MenuItemName);
            Assert.Equal(4.5, breakfastRecommendations.Recommendations[0].PredictedRating);
            Assert.Equal("Burger", breakfastRecommendations.Recommendations[1].MenuItemName);
            Assert.Equal(4.0, breakfastRecommendations.Recommendations[1].PredictedRating);

            var lunchRecommendations = result.FirstOrDefault(r => r.MealTypeId == 2);
            Assert.NotNull(lunchRecommendations);
            Assert.Single(lunchRecommendations.Recommendations);
            Assert.Equal("Pasta", lunchRecommendations.Recommendations[0].MenuItemName);
            Assert.Equal(3.5, lunchRecommendations.Recommendations[0].PredictedRating);
        }

        [Fact]
        public async Task SaveFinalMenu_SavesMenuRecommendations()
        {
            // Arrange
            var mealTypeMenuItems = new List<MealTypeMenuItemList>
            {
                new MealTypeMenuItemList { MealTypeId = 1, MenuItemIds = new List<int> { 1, 2 } },
                new MealTypeMenuItemList { MealTypeId = 2, MenuItemIds = new List<int> { 3 } }
            };

            _mockUnitOfWork.Setup(u => u.Recommendations.Add(It.IsAny<Recommendation>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.RecommendedItems.Add(It.IsAny<RecommendedItem>())).Returns(Task.CompletedTask);

            // Act
            await _chefService.SaveFinalMenu(mealTypeMenuItems);

            // Assert
            _mockUnitOfWork.Verify(u => u.Recommendations.Add(It.IsAny<Recommendation>()), Times.Exactly(2));
            _mockUnitOfWork.Verify(u => u.RecommendedItems.Add(It.IsAny<RecommendedItem>()), Times.Exactly(3));
            _mockNotificationService.Verify(n => n.NotifyEmployees(3, null), Times.Once);
        }
    }
}
