using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealEstateApp.Application.Dtos.Improvements;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Services;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Unit.Tests.Application.Services
{
    public class ImprovementServiceTests
    {
        private readonly Mock<IImprovementRepository> _improvementRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ImprovementService _service;

        public ImprovementServiceTests()
        {
            _improvementRepositoryMock = new Mock<IImprovementRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new ImprovementService(_improvementRepositoryMock.Object, _mapperMock.Object);
        }

        #region GetAllAsync

        [Fact]
        public async Task GetAllAsync_ReturnsMappedDtosOrderedByName()
        {
            // Arrange: uso un DbContext en memoria para que ToListAsync funcione
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new TestDbContext(options);

            var improvements = new List<Improvement>
            {
                new Improvement { Id = 1, Name = "Zeta", Description = "Desc Z", IsActive = true },
                new Improvement { Id = 2, Name = "Alpha", Description = "Desc A", IsActive = false }
            };

            await context.Improvements.AddRangeAsync(improvements);
            await context.SaveChangesAsync();

            _improvementRepositoryMock
                .Setup(r => r.Query())
                .Returns(context.Improvements);

            _mapperMock
                .Setup(m => m.Map<List<ImprovementDto>>(It.IsAny<List<Improvement>>()))
                .Returns((List<Improvement> src) =>
                    src.Select(e => new ImprovementDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Description = e.Description,
                        IsActive = e.IsActive
                    }).ToList());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Alpha", result[0].Name);
            Assert.Equal("Zeta", result[1].Name);
        }

        #endregion

        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_ReturnsDto_WhenEntityExists()
        {
            // Arrange
            var entity = new Improvement
            {
                Id = 10,
                Name = "Pool",
                Description = "Nice pool",
                IsActive = true
            };

            var dto = new ImprovementDto
            {
                Id = 10,
                Name = "Pool",
                Description = "Nice pool",
                IsActive = true
            };

            _improvementRepositoryMock
                .Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(entity);

            _mapperMock
                .Setup(m => m.Map<ImprovementDto>(entity))
                .Returns(dto);

            // Act
            var result = await _service.GetByIdAsync(10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Id, result!.Id);
            Assert.Equal(dto.Name, result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenEntityDoesNotExist()
        {
            // Arrange
            _improvementRepositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Improvement?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
            _mapperMock.Verify(m => m.Map<ImprovementDto>(It.IsAny<Improvement>()), Times.Never);
        }

        #endregion

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_AddsEntityAndReturnsGeneratedId()
        {
            // Arrange
            var dto = new ImprovementDto
            {
                Name = "Garden",
                Description = "Green garden",
                IsActive = true
            };

            Improvement? capturedEntity = null;

            _mapperMock
                .Setup(m => m.Map<Improvement>(dto))
                .Returns(new Improvement
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    IsActive = dto.IsActive
                });

            _improvementRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Improvement>()))
                .Callback<Improvement>(e =>
                {
                    e.Id = 123;
                    capturedEntity = e;
                })
                .Returns(Task.CompletedTask);

            // Act
            var resultId = await _service.CreateAsync(dto);

            // Assert
            Assert.Equal(123, resultId);
            Assert.NotNull(capturedEntity);
            Assert.Equal(dto.Name, capturedEntity!.Name);
            Assert.Equal(dto.Description, capturedEntity.Description);
            Assert.Equal(dto.IsActive, capturedEntity.IsActive);
            Assert.NotEqual(default, capturedEntity.CreatedAt);
        }

        #endregion

        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_UpdatesEntity_WhenEntityExists()
        {
            // Arrange
            var existingEntity = new Improvement
            {
                Id = 5,
                Name = "Old Name",
                Description = "Old Desc",
                IsActive = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };

            var dto = new ImprovementDto
            {
                Id = 5,
                Name = "New Name",
                Description = "New Desc",
                IsActive = true
            };

            Improvement? updatedEntity = null;

            _improvementRepositoryMock
                .Setup(r => r.GetByIdAsync(dto.Id))
                .ReturnsAsync(existingEntity);

            _improvementRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Improvement>()))
                .Callback<Improvement>(e => updatedEntity = e)
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(dto);

            // Assert
            Assert.NotNull(updatedEntity);
            Assert.Equal(dto.Name, updatedEntity!.Name);
            Assert.Equal(dto.Description, updatedEntity.Description);
            Assert.Equal(dto.IsActive, updatedEntity.IsActive);
            Assert.NotEqual(default, updatedEntity.UpdatedAt);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsKeyNotFound_WhenEntityDoesNotExist()
        {
            // Arrange
            var dto = new ImprovementDto
            {
                Id = 999,
                Name = "Does not matter",
                Description = "Nope",
                IsActive = true
            };

            _improvementRepositoryMock
                .Setup(r => r.GetByIdAsync(dto.Id))
                .ReturnsAsync((Improvement?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(dto));
            Assert.Contains("Improvement con Id 999 no existe", ex.Message);
        }

        #endregion

        #region ToggleStatusAsync

        [Fact]
        public async Task ToggleStatusAsync_TogglesIsActive_WhenEntityExists()
        {
            // Arrange
            var entity = new Improvement
            {
                Id = 7,
                Name = "Garage",
                Description = "2 cars",
                IsActive = true
            };

            Improvement? updatedEntity = null;

            _improvementRepositoryMock
                .Setup(r => r.GetByIdAsync(entity.Id))
                .ReturnsAsync(entity);

            _improvementRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Improvement>()))
                .Callback<Improvement>(e => updatedEntity = e)
                .Returns(Task.CompletedTask);

            // Act
            await _service.ToggleStatusAsync(entity.Id);

            // Assert
            Assert.NotNull(updatedEntity);
            Assert.False(updatedEntity!.IsActive); // se invierte
            Assert.NotEqual(default, updatedEntity.UpdatedAt);
        }

        [Fact]
        public async Task ToggleStatusAsync_ThrowsKeyNotFound_WhenEntityDoesNotExist()
        {
            // Arrange
            _improvementRepositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Improvement?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ToggleStatusAsync(999));
            Assert.Contains("Improvement con Id 999 no existe", ex.Message);
        }

        #endregion

        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_DeletesEntity_WhenEntityExists()
        {
            // Arrange
            var entity = new Improvement
            {
                Id = 15,
                Name = "Balcony",
                Description = "Nice view",
                IsActive = true
            };

            _improvementRepositoryMock
                .Setup(r => r.GetByIdAsync(entity.Id))
                .ReturnsAsync(entity);

            _improvementRepositoryMock
                .Setup(r => r.DeleteAsync(entity))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _service.DeleteAsync(entity.Id);

            // Assert
            _improvementRepositoryMock.Verify(r => r.DeleteAsync(entity), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsKeyNotFound_WhenEntityDoesNotExist()
        {
            // Arrange
            _improvementRepositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Improvement?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(999));
            Assert.Contains("Improvement con Id 999 no existe", ex.Message);
        }

        #endregion

        private class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options)
                : base(options)
            {
            }

            public DbSet<Improvement> Improvements { get; set; } = null!;
        }
    }
}
