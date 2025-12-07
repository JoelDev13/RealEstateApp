using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class ImprovementRepository
        : Repository<Improvement>, IImprovementRepository
    {
        public ImprovementRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _dbSet.Where(i => i.Name == name);

            if (excludeId.HasValue)
            {
                query = query.Where(i => i.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> HasPropertiesAsync(int improvementId)
        {
            return await _context.Properties
                .AnyAsync(p => p.Improvements.Any(im => im.Id == improvementId));
        }

        public async Task<int> GetPropertiesCountAsync(int improvementId)
        {
            return await _context.Properties
                .CountAsync(p => p.Improvements.Any(im => im.Id == improvementId));
        }

        public async Task<List<Improvement>> GetByIdsAsync(List<string> ids)
        {
            var improvementIds = ids
                .Where(id => int.TryParse(id, out _))
                .Select(id => int.Parse(id))
                .ToList();

            if (!improvementIds.Any())
            {
                return new List<Improvement>();
            }

            return await _dbSet
                .Where(i => improvementIds.Contains(i.Id))
                .ToListAsync();
        }
    }
}
