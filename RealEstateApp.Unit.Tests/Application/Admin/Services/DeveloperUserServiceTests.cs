using Moq;
using RealEstateApp.Application.Dtos.DeveloperUsers;
using RealEstateApp.Application.Exceptions;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Services;

namespace RealEstateApp.Unit.Tests.Application.Services
{
    public class DeveloperUserServiceTests
    {
        private readonly Mock<IDeveloperUserRepository> _developerRepoMock;
        private readonly DeveloperUserService _service;

        public DeveloperUserServiceTests()
        {
            _developerRepoMock = new Mock<IDeveloperUserRepository>();
            _service = new DeveloperUserService(_developerRepoMock.Object);
        }

        #region GetAllAsync

        [Fact]
        public async Task GetAllAsync_ReturnsListFromRepository()
        {
            // Arrange
            var expected = new List<DeveloperUserDto>
            {
                new DeveloperUserDto { Id = "1", Email = "dev1@test.com", FirstName = "Dev1" },
                new DeveloperUserDto { Id = "2", Email = "dev2@test.com", FirstName = "Dev2" }
            };

            _developerRepoMock
                .Setup(r => r.GetAllDevelopersAsync())
                .ReturnsAsync(expected);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Same(expected, result);
            _developerRepoMock.Verify(r => r.GetAllDevelopersAsync(), Times.Once);
        }

        #endregion

        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_ReturnsDeveloper_WhenExists()
        {
            // Arrange
            var id = "dev-1";
            var expected = new DeveloperUserDto { Id = id, Email = "dev@test.com" };

            _developerRepoMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(expected);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result!.Id);
            Assert.Equal(expected.Email, result.Email);
            _developerRepoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            var id = "missing";

            _developerRepoMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((DeveloperUserDto?)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.Null(result);
            _developerRepoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        #endregion

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_CallsRepositoryAndReturnsId()
        {
            // Arrange
            var dto = new DeveloperUserCreateDto
            {
                Email = "newdev@test.com",
                FirstName = "New",
                LastName = "Developer",
                Password = "Password123!"
            };

            _developerRepoMock
                .Setup(r => r.CreateDeveloperAsync(dto))
                .ReturnsAsync("generated-id");

            // Act
            var resultId = await _service.CreateAsync(dto);

            // Assert
            Assert.Equal("generated-id", resultId);
            _developerRepoMock.Verify(r => r.CreateDeveloperAsync(dto), Times.Once);
        }

        #endregion

        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_CallsRepository_WhenNotCurrentUser()
        {
            // Arrange
            var dto = new DeveloperUserUpdateDto
            {
                Id = "other-dev",
                Email = "updated@test.com"
            };

            var currentUserId = "current-dev";

            _developerRepoMock
                .Setup(r => r.UpdateDeveloperAsync(dto))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _service.UpdateAsync(dto, currentUserId);

            // Assert
            _developerRepoMock.Verify(r => r.UpdateDeveloperAsync(dto), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsApiException_WhenTryingToUpdateSelf()
        {
            // Arrange
            var dto = new DeveloperUserUpdateDto
            {
                Id = "same-dev",
                Email = "me@test.com"
            };

            var currentUserId = "same-dev";

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApiException>(
                () => _service.UpdateAsync(dto, currentUserId));

            Assert.Equal("No puedes editar tu propio usuario desarrollador.", ex.Message);
            Assert.Equal(400, ex.StatusCode);

            _developerRepoMock.Verify(
                r => r.UpdateDeveloperAsync(It.IsAny<DeveloperUserUpdateDto>()),
                Times.Never);
        }

        #endregion

        #region SetActiveStatusAsync

        [Fact]
        public async Task SetActiveStatusAsync_CallsRepository_WhenNotCurrentUser()
        {
            // Arrange
            var id = "other-dev";
            var isActive = true;
            var currentUserId = "current-dev";

            _developerRepoMock
                .Setup(r => r.SetActiveStatusAsync(id, isActive))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _service.SetActiveStatusAsync(id, isActive, currentUserId);

            // Assert
            _developerRepoMock.Verify(r => r.SetActiveStatusAsync(id, isActive), Times.Once);
        }

        [Fact]
        public async Task SetActiveStatusAsync_ThrowsApiException_WhenTryingToChangeOwnStatus()
        {
            // Arrange
            var id = "same-dev";
            var isActive = false;
            var currentUserId = "same-dev";

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApiException>(
                () => _service.SetActiveStatusAsync(id, isActive, currentUserId));

            Assert.Equal("No puedes cambiar el estado de tu propio usuario desarrollador.", ex.Message);
            Assert.Equal(400, ex.StatusCode);

            _developerRepoMock.Verify(
                r => r.SetActiveStatusAsync(It.IsAny<string>(), It.IsAny<bool>()),
                Times.Never);
        }

        #endregion
    }
}
