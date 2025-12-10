using RealEstateApp.Application.Dtos.Property;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IPropertyService
    {
        Task<List<Property>> GetPropertiesByAgentAsync(string agentId);
        Task<List<Property>> GetAvailablePropertiesByAgentAsync(string agentId);
        Task<List<Property>> GetSoldPropertiesByAgentAsync(string agentId);
        Task<Property?> GetPropertyByIdAsync(string id);
        Task<Property> CreatePropertyAsync(CreatePropertyDto dto, string agentId);
        Task<Property> UpdatePropertyAsync(int propertyId, UpdatePropertyDto dto, string agentId);
        Task<bool> DeletePropertyAsync(int propertyId, string agentId);
        Task<Property> GetPropertyDetailAsync(int propertyId);
        Task<string> GenerateUniquePropertyCodeAsync();
        Task<bool> IsPropertyOwnedByAgentAsync(int propertyId, string agentId);
        Task<List<Property>> GetAvailablePropertiesAsync();
        Task<List<Property>> GetPropertiesByIdsAsync(List<int> propertyIds);
        Task<IEnumerable<Property>> GetAgentPropertiesAsync(string agentId);
    }
}
