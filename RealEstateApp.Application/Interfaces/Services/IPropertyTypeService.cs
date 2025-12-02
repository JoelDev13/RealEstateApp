using RealEstateApp.Application.Dtos.PropertyTypes;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IPropertyTypeService
    {
        Task<List<PropertyTypeDto>> GetAllAsync();
        Task<PropertyTypeDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(PropertyTypeDto dto);
        Task UpdateAsync(PropertyTypeDto dto);
        Task ToggleStatusAsync(int id);
        Task DeleteAsync(int id);
    }
}
