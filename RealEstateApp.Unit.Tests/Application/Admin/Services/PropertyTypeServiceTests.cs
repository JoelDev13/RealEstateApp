using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealEstateApp.Application.Dtos.PropertyTypes;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Services;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Unit.Tests.Application.Services
{
    public class PropertyTypeServiceTests
    {
        private readonly Mock<IPropertyTypeRepository> _propertyTypeRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly PropertyTypeService _service;

        public PropertyTypeServiceTests()
        {
            _propertyTypeRepositoryMock = new Mock<IPropertyTypeRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new PropertyTypeService(_propertyTypeRepositoryMock.Object, _mapperMock.Object);
        }

        #region GetAllAsync

        [Fact]
        public async Task GetAllAsync_ReturnsMappedDtosOrderedByName()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new TestDbContext(options);

            var propertyTypes = new List<PropertyType>
            {
                new PropertyType { Id = 1, Name = "Zeta", Description = "Desc Z", IsActive = true },
                new PropertyType { Id = 2, Name = "Alpha", Description = "Desc A", IsActive = false }
            };

            await context.PropertyTypes.AddRangeAsync(propertyTypes);
            await context.SaveChangesAsync();

            _propertyTypeRepositoryMock
                .Setup(r => r.Query())
                .Returns(context.PropertyTypes);

            _mapperMock
                .Setup(m => m.Map<List<PropertyTypeDto>>(It.IsAny<List<PropertyType>>()))
                .Returns((List<PropertyType> src) =>
                    src.Select(e => new PropertyTypeDto
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
            var entity = new PropertyType
            {
                Id = 10,
                Name = "Apartment",
                Description = "Nice apt",
                IsActive = true
            };

            var dto = new PropertyTypeDto
            {
                Id = 10,
                Name = "Apartment",
                Description = "Nice apt",
                IsActive = true
            };

            _propertyTypeRepositoryMock
                .Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(entity);

            _mapperMock
                .Setup(m => m.Map<PropertyTypeDto>(entity))
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
            _propertyTypeRepositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((PropertyType?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
            _mapperMock.Verify(m => m.Map<PropertyTypeDto>(It.IsAny<PropertyType>()), Times.Never);
        }

        #endregion

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_AddsEntityAndReturnsGeneratedId()
        {
            // Arrange
            var dto = new PropertyTypeDto
            {
                Name = "House",
                Description = "Family house",
                IsActive = true
            };

            PropertyType? capturedEntity = null;

            _mapperMock
                .Setup(m => m.Map<PropertyType>(dto))
                .Returns(new PropertyType
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    IsActive = dto.IsActive
                });

            _propertyTypeRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<PropertyType>()))
                .Callback<PropertyType>(e =>
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
            var existingEntity = new PropertyType
            {
                Id = 5,
                Name = "Old Name",
                Description = "Old Desc",
                IsActive = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };

            var dto = new PropertyTypeDto
            {
                Id = 5,
                Name = "New Name",
                Description = "New Desc",
                IsActive = true
            };

            PropertyType? updatedEntity = null;

            _propertyTypeRepositoryMock
                .Setup(r => r.GetByIdAsync(dto.Id))
                .ReturnsAsync(existingEntity);

            _propertyTypeRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<PropertyType>()))
                .Callback<PropertyType>(e => updatedEntity = e)
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
            var dto = new PropertyTypeDto
            {
                Id = 999,
                Name = "Does not matter",
                Description = "Nope",
                IsActive = true
            };

            _propertyTypeRepositoryMock
                .Setup(r => r.GetByIdAsync(dto.Id))
                .ReturnsAsync((PropertyType?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(dto));
            Assert.Contains("PropertyType con Id 999 no existe", ex.Message);
        }

        #endregion

        #region ToggleStatusAsync

        [Fact]
        public async Task ToggleStatusAsync_TogglesIsActive_WhenEntityExists()
        {
            // Arrange
            var entity = new PropertyType
            {
                Id = 7,
                Name = "Condo",
                Description = "Condominium",
                IsActive = true
            };

            PropertyType? updatedEntity = null;

            _propertyTypeRepositoryMock
                .Setup(r => r.GetByIdAsync(entity.Id))
                .ReturnsAsync(entity);

            _propertyTypeRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<PropertyType>()))
                .Callback<PropertyType>(e => updatedEntity = e)
                .Returns(Task.CompletedTask);

            // Act
            await _service.ToggleStatusAsync(entity.Id);

            // Assert
            Assert.NotNull(updatedEntity);
            Assert.False(updatedEntity!.IsActive);
            Assert.NotEqual(default, updatedEntity.UpdatedAt);
        }

        [Fact]
        public async Task ToggleStatusAsync_ThrowsKeyNotFound_WhenEntityDoesNotExist()
        {
            // Arrange
            _propertyTypeRepositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((PropertyType?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ToggleStatusAsync(999));
            Assert.Contains("PropertyType con Id 999 no existe", ex.Message);
        }

        #endregion

        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_DeletesEntity_WhenEntityExists()
        {
            // Arrange
            var entity = new PropertyType
            {
                Id = 15,
                Name = "Villa",
                Description = "Luxury villa",
                IsActive = true
            };

            _propertyTypeRepositoryMock
                .Setup(r => r.GetByIdAsync(entity.Id))
                .ReturnsAsync(entity);

            _propertyTypeRepositoryMock
                .Setup(r => r.DeleteAsync(entity))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _service.DeleteAsync(entity.Id);

            // Assert
            _propertyTypeRepositoryMock.Verify(r => r.DeleteAsync(entity), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsKeyNotFound_WhenEntityDoesNotExist()
        {
            // Arrange
            _propertyTypeRepositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((PropertyType?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(999));
            Assert.Contains("PropertyType con Id 999 no existe", ex.Message);
        }

        #endregion

        private class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options)
                : base(options)
            {
            }

            public DbSet<PropertyType> PropertyTypes { get; set; } = null!;
        }
    }
}
