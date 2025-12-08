using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Interfaces.Repositories
{
    public interface IFavoritePropertyRepository : IRepository<FavoriteProperty>
    {
        Task<List<Property>> GetClientFavoritesAsync(string clientId);
        Task<FavoriteProperty?> GetByClientAndPropertyAsync(string clientId, int propertyId);
        Task<bool> ExistsAsync(string clientId, int propertyId);
        new Task<FavoriteProperty> AddAsync(FavoriteProperty favorite);
        Task<bool> RemoveAsync(string clientId, int propertyId);
    }
}
