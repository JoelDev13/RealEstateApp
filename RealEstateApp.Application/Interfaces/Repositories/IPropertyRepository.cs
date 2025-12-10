using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Interfaces.Repositories
{
    public interface IPropertyRepository : IRepository<Property>
    {
        Task<List<Property>> GetAllAsync();
        Task<List<Property>> GetByAgentAsync(string agentId);
        Task<List<Property>> GetAvailableByAgentAsync(string agentId);
        new Task<Property?> GetByIdAsync(int id);
        Task<Property?> GetByCodeAsync(string code);
        new Task<Property> AddAsync(Property property);
        new Task<Property> UpdateAsync(Property property);
        Task<bool> DeleteAsync(int id);
        Task<bool> CodeExistsAsync(string code);
        Task<List<Property>> GetAvailablePropertiesAsync();
        Task<List<Property>> GetByIdsAsync(List<int> propertyIds);
    }
}

