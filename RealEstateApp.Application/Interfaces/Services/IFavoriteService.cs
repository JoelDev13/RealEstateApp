using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IFavoriteService
    {
        Task AddToFavoritesAsync(string clientId, int propertyId);
        Task RemoveFromFavoritesAsync(string clientId, int propertyId);
        Task<List<Property>> GetClientFavoritesAsync(string clientId);
        Task<bool> IsPropertyFavoriteAsync(string clientId, int propertyId);
    }
}
