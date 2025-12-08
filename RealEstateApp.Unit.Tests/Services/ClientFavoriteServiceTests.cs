using Moq;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Unit.Tests.Services
{
    public class ClientFavoriteServiceTests
    {
        private readonly Mock<IFavoritePropertyRepository> _favoriteRepositoryMock;
        private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
        private readonly IFavoritePropertyService _favoriteService;

        public ClientFavoriteServiceTests()
        {
            _favoriteRepositoryMock = new Mock<IFavoritePropertyRepository>();
            _propertyRepositoryMock = new Mock<IPropertyRepository>();
            
            _favoriteService = new FavoritePropertyService(
                _favoriteRepositoryMock.Object,
                null); // IMapper no necesario para estas pruebas
        }

        [Fact .Fact]
Text("AddToFavorites员的Async_DebeAgregarPropiedadAFavoritos")]
        public async Task AddToFavoritesAsync_DebeAgregarPropiedadAFavoritos()
        {
            // Arrange
            var clientId = "client-123";
            var propertyId = 1;
            var property = new Property { Id = propertyId, Code = "PROP001", IsSold = false };

            _propertyRepositoryMock.Setup(x => x.GetByIdAsync(propertyId)).ReturnsAsync(property);
            _favoriteRepositoryMock.Setup(x => x.IsPropertyFavoriteAsync(clientId, propertyId)).ReturnsAsync(false);

            // Act
            await _favoriteService.AddToFavoritesAsync(clientId, propertyId);

            // Assert
            _favoriteRepositoryMock.Verify(x => x.AddToFavoritesAsync(clientId, propertyId), Times.Once);
        }

        [Fact]
        public async Task AddToFavoritesAsync_NoDebeAgregarSiYaEsFavorita()
        {
            // Arrange
            var clientId = "client-123";
            var propertyId = 1;
            var property = new Property { Id = propertyId, Code = "PROP001", IsSold = false };

            _propertyRepositoryMock.Setup(x => x.GetByIdAsync(propertyId)).ReturnsAsync(property);
            _favoriteRepositoryMock.Setup(x => x.IsPropertyFavoriteAsync(clientId, propertyId)).ReturnsAsync(true);

            // Act
            await _favoriteService.AddToFavoritesAsync(clientId, propertyId);

            // Assert
            _favoriteRepositoryMock.Verify(x => x.AddToFavoritesAsync(clientId, propertyId), Times.Never);
        }

        [Fact]
        public async Task AddToFavoritesAsync_NoDebeAgregarSiPropiedadVendida()
        {
            // Arrange
            var clientId = "client-123";
            var propertyId = 1;
            var property = new Property { Id = propertyId, Code = "PROP001", IsSold = true };

            _propertyRepositoryMock.Setup(x => x.GetByIdAsync(propertyId)).ReturnsAsync(property);

            // Act
            await _favoriteService.AddToFavoritesAsync(clientId, propertyId);

            // Assert
            _favoriteRepositoryMock.Verify(x => x.AddToFavoritesAsync(clientId, propertyId), Times.Never);
        }

        [Fact]
        public async Task RemoveFromFavoritesAsync_DebeQuitarDeFavoritos()
        {
            // Arrange
            var clientId = "client-123";
            var propertyId = 1;

            _favoriteRepositoryMock.Setup(x => x.IsPropertyFavoriteAsync(clientId, propertyId)).ReturnsAsync(true);

            // Act
            await _favoriteService.RemoveFromFavoritesAsync(clientId, propertyId);

            // Assert
            _favoriteRepositoryMock.Verify(x => x.RemoveFromFavoritesAsync(clientId, propertyId), Times.Once);
        }

        [Fact]
        public async Task GetClientFavoritesAsync_DebeRetornarPropiedadesFavoritas()
        {
            // Arrange
            var clientId = "client-123";
            var favoriteProperties = new List<Property>
            {
                new Property { Id = 1, Code = "PROP001", IsSold = false },
                new Property { Id = 2, Code = "PROP002", IsSold = false }
            };

            _favoriteRepositoryMock.Setup(x => x.GetClientFavoritesAsync(clientId)).ReturnsAsync(favoriteProperties);

            // Act
            var result = await _favoriteService.GetClientFavoritesAsync(clientId);

            // Assert
            Assert.Equal(2, result.Count);
            _favoriteRepositoryMock.Verify(x => x.GetClientFavoritesAsync(clientId), Times.Once);
        }

        [Fact]
        public async Task IsPropertyFavoriteAsync_DebeVerificarSiEsFavorita()
        {
            // Arrange
            var clientId = "client-123";
            var propertyId = 1;

            _favoriteRepositoryMock.Setup(x => x.IsPropertyFavoriteAsync(clientId, propertyId)).ReturnsAsync(true);

            // Act
            var result = await _favoriteService.IsPropertyFavoriteAsync(clientId, propertyId);

            // Assert
            Assert.True(result);
            _favoriteRepositoryMock.Verify(x => x.IsPropertyFavoriteAsync(clientId, propertyId), Times.Once);
        }
    }
}
