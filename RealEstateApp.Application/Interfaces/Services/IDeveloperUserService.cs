using RealEstateApp.Application.Dtos.DeveloperUsers;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IDeveloperUserService
    {
        Task<List<DeveloperUserDto>> GetAllAsync();
        Task<DeveloperUserDto?> GetByIdAsync(string id);
        Task<string> CreateAsync(DeveloperUserCreateDto dto);
        Task UpdateAsync(DeveloperUserUpdateDto dto, string currentUserId);
        Task SetActiveStatusAsync(string id, bool isActive, string currentUserId);
    }
}
