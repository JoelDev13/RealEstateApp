using Moq;
using RealEstateApp.Application.Dtos.AdminUsers;
using RealEstateApp.Application.Exceptions;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Services;

namespace RealEstateApp.Unit.Tests.Application.Services
{
    public class AdminUserServiceTests
    {
        private readonly Mock<IAdminUserRepository> _adminUserRepositoryMock;
        private readonly AdminUserService _service;

        public AdminUserServiceTests()
        {
            _adminUserRepositoryMock = new Mock<IAdminUserRepository>();
            _service = new AdminUserService(_adminUserRepositoryMock.Object);
        }

        #region GetAllAsync

        [Fact]
        public async Task GetAllAsync_ReturnsListFromRepository()
        {
            // Arrange
            var expected = new List<AdminUserDto>
            {
                new AdminUserDto { Id = "1", Email = "admin1@test.com", FirstName = "Admin1" },
                new AdminUserDto { Id = "2", Email = "admin2@test.com", FirstName = "Admin2" }
            };

            _adminUserRepositoryMock
                .Setup(r => r.GetAllAdminsAsync())
                .ReturnsAsync(expected);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Same(expected, result);
            _adminUserRepositoryMock.Verify(r => r.GetAllAdminsAsync(), Times.Once);
        }

        #endregion

        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_ReturnsAdmin_WhenExists()
        {
            // Arrange
            var id = "admin-1";
            var expected = new AdminUserDto { Id = id, Email = "admin@test.com" };

            _adminUserRepositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(expected);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result!.Id);
            Assert.Equal(expected.Email, result.Email);
            _adminUserRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            var id = "missing";

            _adminUserRepositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((AdminUserDto?)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.Null(result);
            _adminUserRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        #endregion

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_CallsRepositoryAndReturnsId()
        {
            // Arrange
            var dto = new AdminUserCreateDto
            {
                Email = "newadmin@test.com",
                FirstName = "New",
                LastName = "Admin",
                Password = "Password123!"
            };

            _adminUserRepositoryMock
                .Setup(r => r.CreateAdminAsync(dto))
                .ReturnsAsync("generated-id");

            // Act
            var resultId = await _service.CreateAsync(dto);

            // Assert
            Assert.Equal("generated-id", resultId);
            _adminUserRepositoryMock.Verify(r => r.CreateAdminAsync(dto), Times.Once);
        }

        #endregion

        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_CallsRepository_WhenNotCurrentUser()
        {
            // Arrange
            var dto = new AdminUserUpdateDto
            {
                Id = "other-admin",
                Email = "updated@test.com"
            };

            var currentUserId = "current-admin";

            _adminUserRepositoryMock
                .Setup(r => r.UpdateAdminAsync(dto))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _service.UpdateAsync(dto, currentUserId);

            // Assert
            _adminUserRepositoryMock.Verify(r => r.UpdateAdminAsync(dto), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsApiException_WhenTryingToUpdateSelf()
        {
            // Arrange
            var dto = new AdminUserUpdateDto
            {
                Id = "same-admin",
                Email = "me@test.com"
            };

            var currentUserId = "same-admin";

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApiException>(
                () => _service.UpdateAsync(dto, currentUserId));

            Assert.Equal("No puede editar su propio usuario administrador.", ex.Message);
            Assert.Equal(400, ex.StatusCode);

            _adminUserRepositoryMock.Verify(
                r => r.UpdateAdminAsync(It.IsAny<AdminUserUpdateDto>()),
                Times.Never);
        }

        #endregion

        #region SetActiveStatusAsync

        [Fact]
        public async Task SetActiveStatusAsync_CallsRepository_WhenNotCurrentUser()
        {
            // Arrange
            var id = "other-admin";
            var isActive = true;
            var currentUserId = "current-admin";

            _adminUserRepositoryMock
                .Setup(r => r.SetActiveStatusAsync(id, isActive))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _service.SetActiveStatusAsync(id, isActive, currentUserId);

            // Assert
            _adminUserRepositoryMock.Verify(r => r.SetActiveStatusAsync(id, isActive), Times.Once);
        }

        [Fact]
        public async Task SetActiveStatusAsync_ThrowsApiException_WhenTryingToChangeOwnStatus()
        {
            // Arrange
            var id = "same-admin";
            var isActive = false;
            var currentUserId = "same-admin";

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApiException>(
                () => _service.SetActiveStatusAsync(id, isActive, currentUserId));

            Assert.Equal("No puede cambiar el estado de su propio usuario administrador.", ex.Message);
            Assert.Equal(400, ex.StatusCode);

            _adminUserRepositoryMock.Verify(
                r => r.SetActiveStatusAsync(It.IsAny<string>(), It.IsAny<bool>()),
                Times.Never);
        }

        #endregion
    }
}
