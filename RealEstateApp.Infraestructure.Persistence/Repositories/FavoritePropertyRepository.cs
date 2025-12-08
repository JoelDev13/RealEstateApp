using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class FavoritePropertyRepository : Repository<FavoriteProperty>, IFavoritePropertyRepository
    {
        public FavoritePropertyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Property>> GetClientFavoritesAsync(string clientId)
        {
            return await _context.FavoriteProperties
                .Where(fp => fp.ClientId == clientId)
                .Include(fp => fp.Property)
                    .ThenInclude(p => p.PropertyType)
                .Include(fp => fp.Property)
                    .ThenInclude(p => p.SaleType)
                .Include(fp => fp.Property)
                    .ThenInclude(p => p.Images)
                .Select(fp => fp.Property)
                .ToListAsync();
        }

        public async Task<FavoriteProperty?> GetByClientAndPropertyAsync(string clientId, int propertyId)
        {
            return await _context.FavoriteProperties
                .FirstOrDefaultAsync(fp => fp.ClientId == clientId && fp.PropertyId == propertyId);
        }

        public async Task<bool> ExistsAsync(string clientId, int propertyId)
        {
            return await _context.FavoriteProperties
                .AnyAsync(fp => fp.ClientId == clientId && fp.PropertyId == propertyId);
        }

        public new async Task<FavoriteProperty> AddAsync(FavoriteProperty favorite)
        {
            await _context.FavoriteProperties.AddAsync(favorite);
            await _context.SaveChangesAsync();
            return favorite;
        }

        public async Task<bool> RemoveAsync(string clientId, int propertyId)
        {
            var favorite = await GetByClientAndPropertyAsync(clientId, propertyId);
            if (favorite == null)
                return false;

            _context.FavoriteProperties.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
