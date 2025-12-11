using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence;
using RealEstateApp.Infrastructure.Persistence.Repositories;

namespace RealEstateApp.Integration.Tests.Repositories
{
    public class FavoritePropertyRepositoryTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private async Task<Property> CreateTestProperty(ApplicationDbContext context)
        {
            var saleType = new SaleType { Name = "Venta" };
            var propertyType = new PropertyType { Name = "Casa" };
            context.SaleTypes.Add(saleType);
            context.PropertyTypes.Add(propertyType);
            await context.SaveChangesAsync();

            var property = new Property
            {
                Code = "P001",
                Price = 100000,
                Description = "Propiedad de prueba",
                SizeInSquareMeters = 150,
                Bedrooms = 3,
                Bathrooms = 2,
                PropertyTypeId = propertyType.Id,
                SaleTypeId = saleType.Id,
                AgentId = "AGENT001"
            };

            context.Properties.Add(property);
            await context.SaveChangesAsync();

            return property;
        }

        [Fact]
        public async Task AddAsync_Should_Add_FavoriteProperty()
        {
            using var context = CreateContext();
            var repo = new FavoritePropertyRepository(context);

            var property = await CreateTestProperty(context);

            var favorite = new FavoriteProperty
            {
                ClientId = "CLIENT1",
                PropertyId = property.Id
            };

            // Act
            await repo.AddAsync(favorite);

            // Assert
            Assert.NotEqual(Guid.Empty, favorite.Id);
            var saved = await context.FavoriteProperties.FindAsync(favorite.Id);
            Assert.NotNull(saved);
            Assert.Equal("CLIENT1", saved!.ClientId);
        }

        [Fact]
        public async Task ExistsAsync_Should_Return_True_When_Favorite_Exists()
        {
            using var context = CreateContext();
            var repo = new FavoritePropertyRepository(context);

            var property = await CreateTestProperty(context);

            context.FavoriteProperties.Add(new FavoriteProperty
            {
                ClientId = "CLIENT1",
                PropertyId = property.Id
            });

            await context.SaveChangesAsync();

            // Act
            var exists = await repo.ExistsAsync("CLIENT1", property.Id);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task GetByClientAndPropertyAsync_Should_Return_FavoriteProperty()
        {
            using var context = CreateContext();
            var repo = new FavoritePropertyRepository(context);

            var property = await CreateTestProperty(context);

            var favorite = new FavoriteProperty
            {
                ClientId = "CLIENT1",
                PropertyId = property.Id
            };

            context.FavoriteProperties.Add(favorite);
            await context.SaveChangesAsync();

            // Act
            var result = await repo.GetByClientAndPropertyAsync("CLIENT1", property.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(favorite.Id, result!.Id);
        }

        [Fact]
        public async Task GetClientFavoritesAsync_Should_Return_Property_List()
        {
            using var context = CreateContext();
            var repo = new FavoritePropertyRepository(context);

            var property = await CreateTestProperty(context);

            context.FavoriteProperties.Add(new FavoriteProperty
            {
                ClientId = "CLIENT1",
                PropertyId = property.Id
            });

            await context.SaveChangesAsync();

            // Act
            var favorites = await repo.GetClientFavoritesAsync("CLIENT1");

            // Assert
            Assert.Single(favorites);
            Assert.Equal(property.Id, favorites.First().Id);
        }

        [Fact]
        public async Task RemoveAsync_Should_Delete_FavoriteProperty()
        {
            using var context = CreateContext();
            var repo = new FavoritePropertyRepository(context);

            var property = await CreateTestProperty(context);

            var favorite = new FavoriteProperty
            {
                ClientId = "CLIENT1",
                PropertyId = property.Id
            };

            context.FavoriteProperties.Add(favorite);
            await context.SaveChangesAsync();

            // Act
            var removed = await repo.RemoveAsync("CLIENT1", property.Id);

            // Assert
            Assert.True(removed);
            Assert.False(await repo.ExistsAsync("CLIENT1", property.Id));
        }

        [Fact]
        public async Task RemoveAsync_Should_Return_False_When_Not_Found()
        {
            using var context = CreateContext();
            var repo = new FavoritePropertyRepository(context);

            // Act
            var result = await repo.RemoveAsync("CLIENT1", 999);

            // Assert
            Assert.False(result);
        }
    }
}
