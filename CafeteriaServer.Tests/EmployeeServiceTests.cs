using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
using CafeteriaServer.DTO.ResponseModel;
using CafeteriaServer.Service;
using CafeteriaServer.UnitofWork;
using System.Linq.Expressions;
using Moq;
using System;
using CafeteriaServer.DTO.RequestModel;
using System.Reflection;

namespace CafeteriaServer.Tests
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRecommendationService> _mockRecommendationService;
        private readonly EmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRecommendationService = new Mock<IRecommendationService>();
            _employeeService = new EmployeeService(_mockUnitOfWork.Object, _mockRecommendationService.Object);
        }

        // Tests for individual methods will go here

        [Fact]
        public async Task ProvideFeedback_ValidRequest_ReturnsTrue()
        {
            // Arrange
            var feedbackRequest = new FeedbackRequest
            {
                UserId = 1,
                Comment = "Great food!",
                Rating = 5,
                OrderItemId = 10
            };

            _mockUnitOfWork.Setup(u => u.Feedbacks.Add(It.IsAny<Feedback>())).Returns(Task.CompletedTask);

            // Act
            var result = await _employeeService.ProvideFeedback(feedbackRequest);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.Feedbacks.Add(It.IsAny<Feedback>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public async Task GetRecommendations_ValidUserId_ReturnsRecommendations()
        {
            // Arrange
            var userId = 1;

            var preferences = new PreferenceResponse
            {
                FoodPreference = "Vegetarian",
                SpiceLevel = "Medium",
                CuisinePreference = "South Indian",
                HasSweetTooth = true
            };

            var mealTypes = new List<MealType>
            {
                new MealType { MealTypeId = 1, MealTypeName = "Breakfast" },
                new MealType { MealTypeId = 2, MealTypeName = "Lunch" }
            };

            var recommendations = new List<Recommendation>
            {
                new Recommendation { RecommendationId = 1, MealTypeId = 1, RecommendationDate = DateTime.Now.Date },
                new Recommendation { RecommendationId = 2, MealTypeId = 2, RecommendationDate = DateTime.Now.Date }
            };

            var recommendedItems = new List<RecommendedItem>
            {
                new RecommendedItem { RecommendedItemId = 1, MenuItemId = 1, MenuItem = new MenuItem { ItemName = "Dosa", FoodType = new FoodType { FoodTypeName = "Vegetarian" }, SpiceLevel = new SpiceLevel { Level = "Medium" }, CuisineType = new CuisineType { Cuisine = "South Indian" }, IsSweet = false }, RecommendationId = 1 },
                new RecommendedItem { RecommendedItemId = 2, MenuItemId = 2, MenuItem = new MenuItem { ItemName = "Idli", FoodType = new FoodType { FoodTypeName = "Vegetarian" }, SpiceLevel = new SpiceLevel { Level = "Medium" }, CuisineType = new CuisineType { Cuisine = "South Indian" }, IsSweet = false }, RecommendationId = 2 }
            };

            var updatedRecommendations = new List<MealTypeRecommendations>
            {
                new MealTypeRecommendations
                {
                    MealTypeId = 1,
                    Recommendations = new List<RecommendedItemResponse>
                    {
                        new RecommendedItemResponse { MenuItemId = 1, PredictedRating = 4.5, Comments = new List<string> { "Great choice!" }, OverallSentiment = "Positive" }
                    }
                },
                new MealTypeRecommendations
                {
                    MealTypeId = 2,
                    Recommendations = new List<RecommendedItemResponse>
                    {
                        new RecommendedItemResponse { MenuItemId = 2, PredictedRating = 4.0, Comments = new List<string> { "Good taste!" }, OverallSentiment = "Positive" }
                    }
                }
            };

            _mockUnitOfWork.Setup(u => u.MealTypes.GetAll()).ReturnsAsync(mealTypes);

            _mockUnitOfWork.Setup(u => u.Recommendations.FindAll(It.IsAny<Expression<Func<Recommendation, bool>>>()))
                .ReturnsAsync(recommendations);

            _mockUnitOfWork.Setup(u => u.RecommendedItems.FindAll(It.IsAny<Expression<Func<RecommendedItem, bool>>>()))
                .ReturnsAsync(recommendedItems);

            _mockRecommendationService.Setup(r => r.GetRecommendations()).ReturnsAsync(updatedRecommendations);

            _mockUnitOfWork.Setup(u => u.EmployeePreferences.Find(It.IsAny<Expression<Func<EmployeePreference, bool>>>()))
                .ReturnsAsync(new EmployeePreference
                {
                    FoodType = new FoodType { FoodTypeName = "Vegetarian" },
                    SpiceLevel = new SpiceLevel { Level = "Medium" },
                    CuisineType = new CuisineType { Cuisine = "South Indian" },
                    HasSweetTooth = true
                });

            // Act
            var result = await _employeeService.GetRecommendations(userId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.MealTypeRecommendations.Count);

            var breakfastRecommendation = result.MealTypeRecommendations.First(r => r.MealTypeId == 1);
            var lunchRecommendation = result.MealTypeRecommendations.First(r => r.MealTypeId == 2);

            Assert.Equal("Breakfast", breakfastRecommendation.MealTypeName);
            Assert.Single(breakfastRecommendation.RecommendedItems);
            Assert.Equal("Dosa", breakfastRecommendation.TopRecommendedItem.MenuItemName);
            Assert.Equal(4.5, breakfastRecommendation.TopRecommendedItem.PredictedRating);
            Assert.Equal("Positive", breakfastRecommendation.TopRecommendedItem.OverallSentiment);

            Assert.Equal("Lunch", lunchRecommendation.MealTypeName);
            Assert.Single(lunchRecommendation.RecommendedItems);
            Assert.Equal("Idli", lunchRecommendation.TopRecommendedItem.MenuItemName);
            Assert.Equal(4.0, lunchRecommendation.TopRecommendedItem.PredictedRating);
            Assert.Equal("Positive", lunchRecommendation.TopRecommendedItem.OverallSentiment);
        }


        [Fact]
        public async Task SaveEmployeeOrders_ValidRequest_SavesOrders()
        {
            // Arrange
            var orderRequest = new EmployeeOderRequest
            {
                UserId = 1,
                OrderList = new List<OrderRequest>
                {
                    new OrderRequest { RecommendedItemId = 1 },
                    new OrderRequest { RecommendedItemId = 2 }
                }
            };

            var order = new Order { OrderId = 1, UserId = 1, OrderDate = DateTime.Now };

            _mockUnitOfWork.Setup(u => u.Orders.Add(It.IsAny<Order>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.RecommendedItems.GetById(It.IsAny<int>())).ReturnsAsync(new RecommendedItem { RecommendedItemId = 1 });
            _mockUnitOfWork.Setup(u => u.OrderItems.Add(It.IsAny<OrderItem>())).Returns(Task.CompletedTask);

            // Act
            await _employeeService.SaveEmployeeOrders(orderRequest);

            // Assert
            _mockUnitOfWork.Verify(u => u.Orders.Add(It.IsAny<Order>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.OrderItems.Add(It.IsAny<OrderItem>()), Times.Exactly(2));
            _mockUnitOfWork.Verify(u => u.Save(), Times.Exactly(2));
        }


        [Fact]
        public async Task GetPastOrders_ValidUserId_ReturnsPastOrders()
        {
            // Arrange
            var userId = 1;
            var orderDate = DateTime.Now.AddDays(-2);

            var pastOrders = new List<OrderItem>
            {
                new OrderItem
                {
                    OrderItemId = 1,
                    OrderId = 1,
                    RecommendedItem = new RecommendedItem { MenuItemId = 1, MenuItem = new MenuItem { ItemName = "Dosa" }, Recommendation = new Recommendation { MealTypeId = 1 } },
                    Order = new Order { OrderId = 1, UserId = userId, OrderDate = orderDate }
                }
            };

            _mockUnitOfWork.Setup(u => u.OrderItems.FindAll(It.IsAny<Expression<Func<OrderItem, bool>>>()))
                .ReturnsAsync(pastOrders);

            _mockUnitOfWork.Setup(u => u.Feedbacks.FindAll(It.IsAny<Expression<Func<Feedback, bool>>>()))
                .ReturnsAsync(new List<Feedback>());

            // Act
            var result = await _employeeService.GetPastOrders(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result.First().OrderItemId);
        }

        [Fact]
        public async Task UpdateEmployeePreference_ValidRequest_UpdatesPreference()
        {
            // Arrange
            var request = new UpdateProfileRequest
            {
                UserId = 1,
                FoodTypeId = 2,
                SpiceLevelId = 1,
                CuisineTypeId = 1,
                HasSweetTooth = true
            };

            var preference = new EmployeePreference { UserId = 1 };

            _mockUnitOfWork.Setup(u => u.EmployeePreferences.Find(It.IsAny<Expression<Func<EmployeePreference, bool>>>()))
                           .ReturnsAsync(preference);

            // Act
            var result = await _employeeService.UpdateEmployeePreference(request);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.EmployeePreferences.Update(It.IsAny<EmployeePreference>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public async Task GetEmployeePreference_PreferenceExists_ReturnsPreferenceResponse()
        {
            // Arrange
            int userId = 1;

            var preference = new EmployeePreference
            {
                UserId = userId,
                FoodType = new FoodType { FoodTypeName = "Vegetarian" },
                SpiceLevel = new SpiceLevel { Level = "Medium" },
                CuisineType = new CuisineType { Cuisine = "Indian" },
                HasSweetTooth = true
            };

            _mockUnitOfWork.Setup(u => u.EmployeePreferences.Find(It.IsAny<Expression<Func<EmployeePreference, bool>>>()))
                .ReturnsAsync(preference);

            // Act
            var result = await _employeeService.GetEmployeePreference(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Vegetarian", result.FoodPreference);
            Assert.Equal("Medium", result.SpiceLevel);
            Assert.Equal("Indian", result.CuisinePreference);
            Assert.True(result.HasSweetTooth);
        }

        [Fact]
        public async Task SubmitDetailedFeedback_ValidRequest_SubmitsFeedback()
        {
            // Arrange
            var request = new DetailedFeedbackRequest
            {
                UserId = 1,
                MenuItemId = 1,
                Answers = new List<string> { "Too Spicy", "Less Spicy", "New Recipe" }
            };

            _mockUnitOfWork.Setup(u => u.DetailedFeedbacks.Add(It.IsAny<DetailedFeedback>())).Returns(Task.CompletedTask);

            // Act
            var result = await _employeeService.SubmitDetailedFeedback(request);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.DetailedFeedbacks.Add(It.IsAny<DetailedFeedback>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
        }



        [Fact]
        public async Task GetPendingFeedbackMenuItems_WithPendingItems_ReturnsPendingMenuItems()
        {
            // Arrange
            int userId = 1;
            DateTime startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var notifications = new List<UserNotification>
            {
                new UserNotification { MenuItemId = 1 },
                new UserNotification { MenuItemId = 2 }
            };

            var providedFeedbackItemIds = new List<int> { 2 };

            var menuItems = new List<MenuItem>
            {
                new MenuItem { MenuItemId = 1, ItemName = "Item 1" }
            };

            // Mock GetUserNotifications method
            _mockUnitOfWork.Setup(u => u.UserNotifications
                .FindAll(It.IsAny<Expression<Func<UserNotification, bool>>>()))
                .ReturnsAsync(notifications);

            // Mock GetProvidedFeedbackMenuItemIds method
            _mockUnitOfWork.Setup(u => u.DetailedFeedbacks
                .FindAll(It.IsAny<Expression<Func<DetailedFeedback, bool>>>()))
                .ReturnsAsync(new List<DetailedFeedback>
                {
                new DetailedFeedback { MenuItemId = 2 }
                });

            // Mock the MenuItems repository
            _mockUnitOfWork.Setup(u => u.MenuItems
                .FindAll(It.IsAny<Expression<Func<MenuItem, bool>>>()))
                .ReturnsAsync(menuItems);

            // Act
            var result = await _employeeService.GetPendingFeedbackMenuItems(userId);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result.First().MenuItemId);
            Assert.Equal("Item 1", result.First().ItemName);
        }

        [Fact]
        public async Task GetProvidedFeedbackMenuItemIds_WithValidInputs_ReturnsMenuItemIds()
        {
            // Arrange
            int userId = 1;
            List<int> menuItemIds = new List<int> { 1, 2 };
            DateTime startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var feedbacks = new List<DetailedFeedback>
            {
                new DetailedFeedback { UserId = userId, MenuItemId = 1, FeedbackDate = startOfMonth.AddDays(1) },
                new DetailedFeedback { UserId = userId, MenuItemId = 2, FeedbackDate = startOfMonth.AddDays(2) }
            };

            _mockUnitOfWork.Setup(u => u.DetailedFeedbacks
                .FindAll(It.IsAny<Expression<Func<DetailedFeedback, bool>>>()))
                .ReturnsAsync(feedbacks);

            var method = typeof(EmployeeService).GetMethod("GetProvidedFeedbackMenuItemIds", BindingFlags.NonPublic | BindingFlags.Instance);
            var parameters = new object[] { userId, menuItemIds };

            // Act
            var result = await (Task<List<int>>)method.Invoke(_employeeService, parameters);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(1, result);
            Assert.Contains(2, result);
        }
    }
}
