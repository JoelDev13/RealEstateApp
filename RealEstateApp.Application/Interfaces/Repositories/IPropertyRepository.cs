using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Interfaces.Repositories
{
    public interface IPropertyRepository : IRepository<Property>
    {
        Task<List<Property>> GetByAgentAsync(string agentId);
        Task<List<Property>> GetAvailableByAgentAsync(string agentId);
        new Task<Property?> GetByIdAsync(int id);
        new Task<Property> AddAsync(Property property);
        new Task<Property> UpdateAsync(Property property);
        Task<bool> DeleteAsync(int id);
        Task<bool> CodeExistsAsync(string code);
    }
}

