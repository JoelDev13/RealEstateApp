using RealEstateApp.Application.Dtos.Improvements;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IImprovementService
    {
        Task<List<ImprovementDto>> GetAllAsync();
        Task<ImprovementDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(ImprovementDto dto);
        Task UpdateAsync(ImprovementDto dto);
        Task ToggleStatusAsync(int id);
        Task DeleteAsync(int id);
    }
}
