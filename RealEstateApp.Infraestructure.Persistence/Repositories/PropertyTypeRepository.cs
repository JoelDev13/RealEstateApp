using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class PropertyTypeRepository : Repository<PropertyType>, IPropertyTypeRepository
    {
        public PropertyTypeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _dbSet.Where(pt => pt.Name == name);

            if (excludeId.HasValue)
            {
                query = query.Where(pt => pt.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> HasPropertiesAsync(int propertyTypeId)
        {
            return await _context.Properties
                .AnyAsync(p => p.PropertyTypeId == propertyTypeId);
        }

        public async Task<int> GetPropertiesCountAsync(int propertyTypeId)
        {
            return await _context.Properties
                .CountAsync(p => p.PropertyTypeId == propertyTypeId);
        }
    }
}
