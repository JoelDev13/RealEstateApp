using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Infrastructure.Persistence;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class PropertyRepository : Repository<Property>, IPropertyRepository
    {
        private new readonly ApplicationDbContext _context;

        public PropertyRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Property>> GetAllAsync()
        {
            return await _context.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.SaleType)
                .Include(p => p.Images)
                .Include(p => p.Improvements)
                .ToListAsync();
        }

        public async Task<List<Property>> GetByAgentAsync(string agentId)
        {
            return await _context.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.SaleType)
                .Include(p => p.Images)
                .Include(p => p.Improvements)
                .Where(p => p.AgentId == agentId)
                .ToListAsync();
        }

        public async Task<List<Property>> GetAvailableByAgentAsync(string agentId)
        {
            return await _context.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.SaleType)
                .Include(p => p.Images)
                .Include(p => p.Improvements)
                .Where(p => p.AgentId == agentId && p.Status == Domain.Enums.PropertyStatus.Disponible)
                .ToListAsync();
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _context.Properties
                .AnyAsync(p => p.Code == code);
        }

        public async Task<Property?> GetByIdAsync(int id)
        {
            return await _context.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.SaleType)
                .Include(p => p.Images)
                .Include(p => p.Improvements)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Property?> GetByCodeAsync(string code)
        {
            return await _context.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.SaleType)
                .Include(p => p.Images)
                .Include(p => p.Improvements)
                .FirstOrDefaultAsync(p => p.Code == code);
        }

        public async Task<Property> AddAsync(Property property)
        {
            await _context.Properties.AddAsync(property);
            await _context.SaveChangesAsync();
            return property;
        }

        public async Task<Property> UpdateAsync(Property property)
        {
            _context.Properties.Update(property);
            await _context.SaveChangesAsync();
            return property;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var property = await GetByIdAsync(id);
            if (property == null)
            {
                return false;
            }

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Property>> GetAvailablePropertiesAsync()
        {
            return await _context.Properties
                .Where(p => p.Status == PropertyStatus.Disponible && p.IsActive)
                .Include(p => p.PropertyType)
                .Include(p => p.SaleType)
                .Include(p => p.Images)
                .Include(p => p.Improvements)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Property>> GetByIdsAsync(List<int> propertyIds)
        {
            return await _context.Properties
                .Where(p => propertyIds.Contains(p.Id) && p.IsActive)
                .Include(p => p.PropertyType)
                .Include(p => p.SaleType)
                .Include(p => p.Images)
                .Include(p => p.Improvements)
                .ToListAsync();
        }
    }
}
