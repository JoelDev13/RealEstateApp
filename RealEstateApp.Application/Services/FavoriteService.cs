using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoritePropertyRepository _favoriteRepository;
        private readonly IPropertyRepository _propertyRepository;

        public FavoriteService(
            IFavoritePropertyRepository favoriteRepository,
            IPropertyRepository propertyRepository)
        {
            _favoriteRepository = favoriteRepository;
            _propertyRepository = propertyRepository;
        }

        public async Task AddToFavoritesAsync(string clientId, int propertyId)
        {
            // Validate property exists
            var property = await _propertyRepository.GetByIdAsync(propertyId);
            if (property == null)
                throw new KeyNotFoundException("Property not found");

            // Check if already favorited
            var existing = await _favoriteRepository.GetByClientAndPropertyAsync(clientId, propertyId);
            if (existing != null)
                throw new InvalidOperationException("Property already in favorites");

            var favorite = new FavoriteProperty
            {
                ClientId = clientId,
                PropertyId = propertyId,
                AddedDate = DateTime.UtcNow
            };

            await _favoriteRepository.AddAsync(favorite);
        }

        public async Task RemoveFromFavoritesAsync(string clientId, int propertyId)
        {
            var favorite = await _favoriteRepository.GetByClientAndPropertyAsync(clientId, propertyId);
            if (favorite == null)
                throw new KeyNotFoundException("Favorite not found");

            await _favoriteRepository.RemoveAsync(clientId, propertyId);
        }

        public async Task<List<Property>> GetClientFavoritesAsync(string clientId)
        {
            return await _favoriteRepository.GetClientFavoritesAsync(clientId);
        }

        public async Task<bool> IsPropertyFavoriteAsync(string clientId, int propertyId)
        {
            return await _favoriteRepository.ExistsAsync(clientId, propertyId);
        }
    }
}
