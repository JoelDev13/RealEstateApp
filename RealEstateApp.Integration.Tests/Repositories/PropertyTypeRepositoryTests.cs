using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence;
using RealEstateApp.Infrastructure.Persistence.Repositories;

namespace RealEstateApp.Integration.Tests.Repositories
{
    public class PropertyTypeRepositoryTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task AddAsync_Should_Create_New_PropertyType()
        {
            // Arrange
            using var context = CreateContext();
            var repo = new PropertyTypeRepository(context);

            var entity = new PropertyType
            {
                Name = "Apartamento",
                Description = "Vivienda en edificio"
            };

            // Act
            await repo.AddAsync(entity);

            // Assert
            Assert.True(entity.Id > 0);
            var saved = await context.PropertyTypes.FindAsync(entity.Id);
            Assert.NotNull(saved);
            Assert.Equal("Apartamento", saved!.Name);
        }

        [Fact]
        public async Task ExistsByNameAsync_Should_Return_True_When_Name_Exists()
        {
            using var context = CreateContext();
            var repo = new PropertyTypeRepository(context);

            context.PropertyTypes.Add(new PropertyType { Name = "Casa" });
            await context.SaveChangesAsync();

            // Act
            var result = await repo.ExistsByNameAsync("Casa");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsByNameAsync_Should_Ignore_Entity_With_Excluded_Id()
        {
            using var context = CreateContext();
            var repo = new PropertyTypeRepository(context);

            var entity = new PropertyType { Name = "Villa" };
            context.PropertyTypes.Add(entity);
            await context.SaveChangesAsync();

            // Act
            var result = await repo.ExistsByNameAsync("Villa", excludeId: entity.Id);

            // Assert
            Assert.False(result); // porque se excluyó el único que existe
        }

        [Fact]
        public async Task HasPropertiesAsync_Should_Return_True_When_Properties_Exist()
        {
            using var context = CreateContext();
            var repo = new PropertyTypeRepository(context);

            var type = new PropertyType { Name = "Terreno" };
            context.PropertyTypes.Add(type);
            await context.SaveChangesAsync();

            context.Properties.Add(new Property
            {
                Code = "P001",
                Price = 100000,
                Description = "Solar amplio",
                SizeInSquareMeters = 250,
                Bedrooms = 0,
                Bathrooms = 0,
                PropertyTypeId = type.Id,
                SaleTypeId = 1,
                AgentId = "TEST"
            });

            await context.SaveChangesAsync();

            // Act
            var result = await repo.HasPropertiesAsync(type.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetPropertiesCountAsync_Should_Return_Correct_Count()
        {
            using var context = CreateContext();
            var repo = new PropertyTypeRepository(context);

            var type = new PropertyType { Name = "Local Comercial" };
            context.PropertyTypes.Add(type);
            await context.SaveChangesAsync();

            context.Properties.Add(new Property
            {
                Code = "L001",
                Price = 250000,
                Description = "Local en plaza",
                SizeInSquareMeters = 80,
                Bedrooms = 0,
                Bathrooms = 1,
                PropertyTypeId = type.Id,
                SaleTypeId = 1,
                AgentId = "TEST"
            });

            context.Properties.Add(new Property
            {
                Code = "L002",
                Price = 300000,
                Description = "Local céntrico",
                SizeInSquareMeters = 60,
                Bedrooms = 0,
                Bathrooms = 1,
                PropertyTypeId = type.Id,
                SaleTypeId = 1,
                AgentId = "TEST"
            });

            await context.SaveChangesAsync();

            // Act
            var count = await repo.GetPropertiesCountAsync(type.Id);

            // Assert
            Assert.Equal(2, count);
        }
    }
}
