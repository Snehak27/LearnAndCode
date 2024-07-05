using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using CafeteriaServer.UnitofWork;
using Moq;
using System;
using System.Linq.Expressions;

namespace CafeteriaServer.Tests
{
    public class AdminServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly AdminService _adminService;

        public AdminServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _adminService = new AdminService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task AddMenu_ShouldAddMenuItem()
        {
            // Arrange
            var menuItemRequest = new MenuItemRequest
            {
                ItemName = "Pizza",
                Price = 9.99,
                AvailabilityStatus = true,
                FoodTypeId = 1,
                SpiceLevelId = 2,
                CuisineTypeId = 3,
                IsSweet = false
            };

            var employees = new List<User>
        {
            new User { UserId = 1, RoleId = 3 },
            new User { UserId = 2, RoleId = 3 }
        };
            Expression<Func<User, bool>> employeeRoleExpression = u => u.RoleId == 3;
            _mockUnitOfWork.Setup(u => u.Users.FindAll(employeeRoleExpression))
                .ReturnsAsync(employees);

            // Act
            var result = await _adminService.AddMenu(menuItemRequest);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.MenuItems.Add(It.IsAny<MenuItem>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.UserNotifications.Add(It.IsAny<UserNotification>()), Times.Exactly(employees.Count));
            _mockUnitOfWork.Verify(u => u.Save(), Times.Exactly(2)); 
        }

        [Fact]
        public async Task DeleteMenu_ShouldDeleteMenuItem()
        {
            // Arrange
            var menuItem = new MenuItem
            {
                MenuItemId = 1,
                ItemName = "Pizza"
            };

            _mockUnitOfWork.Setup(u => u.MenuItems.GetById(It.IsAny<int>()))
                .ReturnsAsync(menuItem);

            // Act
            var result = await _adminService.DeleteMenu(menuItem.MenuItemId);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.MenuItems.Delete(It.IsAny<MenuItem>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
        }
    }
}
