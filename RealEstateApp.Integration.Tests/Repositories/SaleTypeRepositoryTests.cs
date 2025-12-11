using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence;
using RealEstateApp.Infrastructure.Persistence.Repositories;

namespace RealEstateApp.Integration.Tests.Repositories
{
    public class SaleTypeRepositoryTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task AddAsync_Should_Create_New_SaleType()
        {
            // Arrange
            using var context = CreateContext();
            var repo = new SaleTypeRepository(context);

            var entity = new SaleType
            {
                Name = "Venta Directa",
                Description = "Venta regular"
            };

            // Act
            await repo.AddAsync(entity);

            // Assert
            Assert.True(entity.Id > 0);
            var saved = await context.SaleTypes.FindAsync(entity.Id);
            Assert.NotNull(saved);
            Assert.Equal("Venta Directa", saved!.Name);
        }

        [Fact]
        public async Task ExistsByNameAsync_Should_Return_True_When_Name_Exists()
        {
            using var context = CreateContext();
            var repo = new SaleTypeRepository(context);

            context.SaleTypes.Add(new SaleType { Name = "Alquiler" });
            await context.SaveChangesAsync();

            // Act
            var result = await repo.ExistsByNameAsync("Alquiler");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsByNameAsync_Should_Ignore_Entity_With_Excluded_Id()
        {
            using var context = CreateContext();
            var repo = new SaleTypeRepository(context);

            var entity = new SaleType { Name = "Lease" };
            context.SaleTypes.Add(entity);
            await context.SaveChangesAsync();

            // Act
            var result = await repo.ExistsByNameAsync("Lease", excludeId: entity.Id);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task HasPropertiesAsync_Should_Return_True_When_Properties_Exist()
        {
            using var context = CreateContext();
            var repo = new SaleTypeRepository(context);

            var saleType = new SaleType { Name = "Renta" };
            context.SaleTypes.Add(saleType);
            await context.SaveChangesAsync();

            context.Properties.Add(new Property
            {
                Code = "P001",
                Price = 50000,
                Description = "Propiedad de prueba",
                SizeInSquareMeters = 120,
                Bedrooms = 2,
                Bathrooms = 1,
                PropertyTypeId = 1,
                SaleTypeId = saleType.Id,
                AgentId = "TEST"
            });

            await context.SaveChangesAsync();

            // Act
            var result = await repo.HasPropertiesAsync(saleType.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetPropertiesCountAsync_Should_Return_Correct_Count()
        {
            using var context = CreateContext();
            var repo = new SaleTypeRepository(context);

            var saleType = new SaleType { Name = "Financiado" };
            context.SaleTypes.Add(saleType);
            await context.SaveChangesAsync();

            context.Properties.Add(new Property
            {
                Code = "P001",
                Price = 120000,
                Description = "Propiedad 1",
                SizeInSquareMeters = 140,
                Bedrooms = 3,
                Bathrooms = 2,
                PropertyTypeId = 1,
                SaleTypeId = saleType.Id,
                AgentId = "TEST"
            });

            context.Properties.Add(new Property
            {
                Code = "P002",
                Price = 95000,
                Description = "Propiedad 2",
                SizeInSquareMeters = 100,
                Bedrooms = 2,
                Bathrooms = 1,
                PropertyTypeId = 1,
                SaleTypeId = saleType.Id,
                AgentId = "TEST"
            });

            await context.SaveChangesAsync();

            // Act
            var count = await repo.GetPropertiesCountAsync(saleType.Id);

            // Assert
            Assert.Equal(2, count);
        }
    }
}
