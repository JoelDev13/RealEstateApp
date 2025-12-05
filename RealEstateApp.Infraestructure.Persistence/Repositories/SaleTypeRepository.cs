using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class SaleTypeRepository : Repository<SaleType>, ISaleTypeRepository
    {
        public SaleTypeRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _dbSet.Where(st => st.Name == name);

            if (excludeId.HasValue)
            {
                query = query.Where(st => st.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> HasPropertiesAsync(int saleTypeId)
        {
            return await _context.Properties
                .AnyAsync(p => p.SaleTypeId == saleTypeId);
        }

        public async Task<int> GetPropertiesCountAsync(int saleTypeId)
        {
            return await _context.Properties
                .CountAsync(p => p.SaleTypeId == saleTypeId);
        }
    }
}
