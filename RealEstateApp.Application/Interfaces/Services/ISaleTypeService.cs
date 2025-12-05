using RealEstateApp.Application.Dtos.SaleTypes;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface ISaleTypeService
    {
        Task<List<SaleTypeDto>> GetAllAsync();
        Task<SaleTypeDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(SaleTypeDto dto);
        Task UpdateAsync(SaleTypeDto dto);
        Task ToggleStatusAsync(int id);
        Task DeleteAsync(int id);
    }
}
