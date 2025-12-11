using Moq;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.Services;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;

namespace RealEstateApp.Unit.Tests.Services
{
    public class ClientFavoriteServiceTests
    {
        private readonly Mock<IFavoritePropertyRepository> _favoriteRepositoryMock;
        private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
        private readonly IFavoriteService _favoriteService;

        public ClientFavoriteServiceTests()
        {
            _favoriteRepositoryMock = new Mock<IFavoritePropertyRepository>();
            _propertyRepositoryMock = new Mock<IPropertyRepository>();
            
            _favoriteService = new FavoriteService(
                _favoriteRepositoryMock.Object,
                _propertyRepositoryMock.Object);
        }

        [Fact]
        public async Task AddToFavoritesAsync_DebeAgregarPropiedadAFavoritos()
        {
            // Arrange
            var clientId = "client-123";
            var propertyId = 1;
            var property = new Property { Id = propertyId, Code = "PROP001", Status = PropertyStatus.Disponible };

            _propertyRepositoryMock.Setup(x => x.GetByIdAsync(propertyId)).ReturnsAsync(property);
            _favoriteRepositoryMock.Setup(x => x.GetByClientAndPropertyAsync(clientId, propertyId)).ReturnsAsync((FavoriteProperty?)null);

            // Act
            await _favoriteService.AddToFavoritesAsync(clientId, propertyId);

            // Assert
            _favoriteRepositoryMock.Verify(x => x.AddAsync(It.IsAny<FavoriteProperty>()), Times.Once);
        }

        [Fact]
        public async Task AddToFavoritesAsync_NoDebeAgregarSiYaEsFavorita()
        {
            // Arrange
            var clientId = "client-123";
            var propertyId = 1;
            var property = new Property { Id = propertyId, Code = "PROP001", Status = PropertyStatus.Disponible };

            _propertyRepositoryMock.Setup(x => x.GetByIdAsync(propertyId)).ReturnsAsync(property);
            _favoriteRepositoryMock.Setup(x => x.GetByClientAndPropertyAsync(clientId, propertyId)).ReturnsAsync(new FavoriteProperty());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _favoriteService.AddToFavoritesAsync(clientId, propertyId));
        }

        [Fact]
        public async Task AddToFavoritesAsync_PermiteAgregarPropiedadVendida()
        {
            // Arrange
            var clientId = "client-123";
            var propertyId = 1;
            var property = new Property { Id = propertyId, Code = "PROP001", Status = PropertyStatus.Vendida };

            _propertyRepositoryMock.Setup(x => x.GetByIdAsync(propertyId)).ReturnsAsync(property);
            _favoriteRepositoryMock.Setup(x => x.GetByClientAndPropertyAsync(clientId, propertyId)).ReturnsAsync((FavoriteProperty?)null);

            // Act
            await _favoriteService.AddToFavoritesAsync(clientId, propertyId);

            // Assert - El servicio actual NO valida el estado de la propiedad
            _favoriteRepositoryMock.Verify(x => x.AddAsync(It.IsAny<FavoriteProperty>()), Times.Once);
        }

        [Fact]
        public async Task RemoveFromFavoritesAsync_DebeQuitarDeFavoritos()
        {
            // Arrange
            var clientId = "client-123";
            var propertyId = 1;

            var favorite = new FavoriteProperty { ClientId = clientId, PropertyId = propertyId };
            _favoriteRepositoryMock.Setup(x => x.GetByClientAndPropertyAsync(clientId, propertyId)).ReturnsAsync(favorite);

            // Act
            await _favoriteService.RemoveFromFavoritesAsync(clientId, propertyId);

            // Assert
            _favoriteRepositoryMock.Verify(x => x.RemoveAsync(clientId, propertyId), Times.Once);
        }

        [Fact]
        public async Task GetClientFavoritesAsync_DebeRetornarPropiedadesFavoritas()
        {
            // Arrange
            var clientId = "client-123";
            var favoriteProperties = new List<Property>
            {
                new Property { Id = 1, Code = "PROP001", Status = PropertyStatus.Disponible },
                new Property { Id = 2, Code = "PROP002", Status = PropertyStatus.Disponible }
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

            _favoriteRepositoryMock.Setup(x => x.ExistsAsync(clientId, propertyId)).ReturnsAsync(true);

            // Act
            var result = await _favoriteService.IsPropertyFavoriteAsync(clientId, propertyId);

            // Assert
            Assert.True(result);
            _favoriteRepositoryMock.Verify(x => x.ExistsAsync(clientId, propertyId), Times.Once);
        }
    }
}
