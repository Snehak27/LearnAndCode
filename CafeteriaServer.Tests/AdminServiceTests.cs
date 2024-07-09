using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using CafeteriaServer.UnitofWork;
using Moq;
using System;

namespace CafeteriaServer.Tests
{
    public class AdminServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly AdminService _adminService;

        public AdminServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockNotificationService = new Mock<INotificationService>();
            _adminService = new AdminService(_mockUnitOfWork.Object, _mockNotificationService.Object);
        }

        [Fact]
        public async Task AddMenu_ValidMenuItem_AddsMenuItemAndNotifiesEmployees()
        {
            // Arrange
            var menuItemRequest = new MenuItemRequest
            {
                ItemName = "Test Item",
                Price = 10.0,
                AvailabilityStatus = true,
                FoodTypeId = 1,
                SpiceLevelId = 1,
                CuisineTypeId = 1,
                IsSweet = false
            };

            _mockUnitOfWork.Setup(u => u.MenuItems.Add(It.IsAny<MenuItem>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.Save()).Verifiable();

            // Act
            var result = await _adminService.AddMenu(menuItemRequest);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.MenuItems.Add(It.IsAny<MenuItem>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
            _mockNotificationService.Verify(n => n.NotifyEmployees(1, It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task UpdateMenu_ValidMenuItem_UpdatesMenuItemAndNotifiesEmployeesIfAvailabilityChanged()
        {
            // Arrange
            var menuItem = new MenuItem
            {
                MenuItemId = 1,
                ItemName = "Updated Item",
                Price = 15.0,
                AvailabilityStatus = true,
                FoodTypeId = 1,
                SpiceLevelId = 1,
                CuisineTypeId = 1,
                IsSweet = false
            };

            _mockUnitOfWork.Setup(u => u.MenuItems.GetById(It.IsAny<int>())).ReturnsAsync(new MenuItem { MenuItemId = 1, AvailabilityStatus = false });
            _mockUnitOfWork.Setup(u => u.MenuItems.Update(It.IsAny<MenuItem>())).Verifiable();
            _mockUnitOfWork.Setup(u => u.Save()).Verifiable();

            // Act
            var result = await _adminService.UpdateMenu(menuItem);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.MenuItems.GetById(It.IsAny<int>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.MenuItems.Update(It.IsAny<MenuItem>()), Times.Once);
            _mockNotificationService.Verify(n => n.NotifyEmployees(2, It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task DeleteMenu_ValidId_DeletesMenuItem()
        {
            // Arrange
            var menuItem = new MenuItem { MenuItemId = 1 };
            _mockUnitOfWork.Setup(u => u.MenuItems.GetById(It.IsAny<int>())).ReturnsAsync(menuItem);
            _mockUnitOfWork.Setup(u => u.MenuItems.Delete(It.IsAny<MenuItem>())).Verifiable();
            _mockUnitOfWork.Setup(u => u.Save()).Verifiable();

            // Act
            var result = await _adminService.DeleteMenu(1);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.MenuItems.GetById(It.IsAny<int>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.MenuItems.Delete(It.IsAny<MenuItem>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.Save(), Times.Once);
        }
    }
}