using RealEstateApp.Application.Dtos.Property;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IPropertyService
    {
        Task<List<Property>> GetPropertiesByAgentAsync(string agentId);
        Task<List<Property>> GetAvailablePropertiesByAgentAsync(string agentId);
        Task<Property?> GetPropertyByIdAsync(string id);
        Task<Property> CreatePropertyAsync(CreatePropertyDto dto);
        Task<Property> UpdatePropertyAsync(UpdatePropertyDto dto);
        Task<bool> DeletePropertyAsync(string id);
        Task<string> GenerateUniquePropertyCodeAsync();
    }
}
