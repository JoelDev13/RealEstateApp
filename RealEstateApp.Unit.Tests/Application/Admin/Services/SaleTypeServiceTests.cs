using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealEstateApp.Application.Dtos.SaleTypes;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Services;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Unit.Tests.Application.Services
{
    public class SaleTypeServiceTests
    {
        private readonly Mock<ISaleTypeRepository> _saleTypeRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly SaleTypeService _service;

        public SaleTypeServiceTests()
        {
            _saleTypeRepositoryMock = new Mock<ISaleTypeRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new SaleTypeService(_saleTypeRepositoryMock.Object, _mapperMock.Object);
        }

        #region GetAllAsync

        [Fact]
        public async Task GetAllAsync_ReturnsMappedDtosOrderedByName()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new TestDbContext(options);

            var saleTypes = new List<SaleType>
            {
                new SaleType { Id = 1, Name = "Zeta", Description = "Desc Z", IsActive = true },
                new SaleType { Id = 2, Name = "Alpha", Description = "Desc A", IsActive = false }
            };

            await context.SaleTypes.AddRangeAsync(saleTypes);
            await context.SaveChangesAsync();

            _saleTypeRepositoryMock
                .Setup(r => r.Query())
                .Returns(context.SaleTypes);

            _mapperMock
                .Setup(m => m.Map<List<SaleTypeDto>>(It.IsAny<List<SaleType>>()))
                .Returns((List<SaleType> src) =>
                    src.Select(e => new SaleTypeDto
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
            var entity = new SaleType
            {
                Id = 10,
                Name = "Rent",
                Description = "Monthly rent",
                IsActive = true
            };

            var dto = new SaleTypeDto
            {
                Id = 10,
                Name = "Rent",
                Description = "Monthly rent",
                IsActive = true
            };

            _saleTypeRepositoryMock
                .Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(entity);

            _mapperMock
                .Setup(m => m.Map<SaleTypeDto>(entity))
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
            _saleTypeRepositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((SaleType?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
            _mapperMock.Verify(m => m.Map<SaleTypeDto>(It.IsAny<SaleType>()), Times.Never);
        }

        #endregion

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_AddsEntityAndReturnsGeneratedId()
        {
            // Arrange
            var dto = new SaleTypeDto
            {
                Name = "Sale",
                Description = "Full sale",
                IsActive = true
            };

            SaleType? capturedEntity = null;

            _mapperMock
                .Setup(m => m.Map<SaleType>(dto))
                .Returns(new SaleType
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    IsActive = dto.IsActive
                });

            _saleTypeRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<SaleType>()))
                .Callback<SaleType>(e =>
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
            var existingEntity = new SaleType
            {
                Id = 5,
                Name = "Old Name",
                Description = "Old Desc",
                IsActive = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };

            var dto = new SaleTypeDto
            {
                Id = 5,
                Name = "New Name",
                Description = "New Desc",
                IsActive = true
            };

            SaleType? updatedEntity = null;

            _saleTypeRepositoryMock
                .Setup(r => r.GetByIdAsync(dto.Id))
                .ReturnsAsync(existingEntity);

            _saleTypeRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<SaleType>()))
                .Callback<SaleType>(e => updatedEntity = e)
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
            var dto = new SaleTypeDto
            {
                Id = 999,
                Name = "Does not matter",
                Description = "Nope",
                IsActive = true
            };

            _saleTypeRepositoryMock
                .Setup(r => r.GetByIdAsync(dto.Id))
                .ReturnsAsync((SaleType?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(dto));
            Assert.Contains("SaleType con Id 999 no existe", ex.Message);
        }

        #endregion

        #region ToggleStatusAsync

        [Fact]
        public async Task ToggleStatusAsync_TogglesIsActive_WhenEntityExists()
        {
            // Arrange
            var entity = new SaleType
            {
                Id = 7,
                Name = "Lease",
                Description = "Lease option",
                IsActive = true
            };

            SaleType? updatedEntity = null;

            _saleTypeRepositoryMock
                .Setup(r => r.GetByIdAsync(entity.Id))
                .ReturnsAsync(entity);

            _saleTypeRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<SaleType>()))
                .Callback<SaleType>(e => updatedEntity = e)
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
            _saleTypeRepositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((SaleType?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ToggleStatusAsync(999));
            Assert.Contains("SaleType con Id 999 no existe", ex.Message);
        }

        #endregion

        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_DeletesEntity_WhenEntityExists()
        {
            // Arrange
            var entity = new SaleType
            {
                Id = 15,
                Name = "Special",
                Description = "Special sale",
                IsActive = true
            };

            _saleTypeRepositoryMock
                .Setup(r => r.GetByIdAsync(entity.Id))
                .ReturnsAsync(entity);

            _saleTypeRepositoryMock
                .Setup(r => r.DeleteAsync(entity))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _service.DeleteAsync(entity.Id);

            // Assert
            _saleTypeRepositoryMock.Verify(r => r.DeleteAsync(entity), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsKeyNotFound_WhenEntityDoesNotExist()
        {
            // Arrange
            _saleTypeRepositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((SaleType?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(999));
            Assert.Contains("SaleType con Id 999 no existe", ex.Message);
        }

        #endregion
        private class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options)
                : base(options)
            {
            }

            public DbSet<SaleType> SaleTypes { get; set; } = null!;
        }
    }
}
