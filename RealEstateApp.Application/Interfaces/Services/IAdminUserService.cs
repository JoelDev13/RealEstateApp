using RealEstateApp.Application.Dtos.AdminUsers;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IAdminUserService
    {
        Task<List<AdminUserDto>> GetAllAsync();
        Task<AdminUserDto?> GetByIdAsync(string id);
        Task<string> CreateAsync(AdminUserCreateDto dto);
        Task UpdateAsync(AdminUserUpdateDto dto, string currentUserId);
        Task SetActiveStatusAsync(string id, bool isActive, string currentUserId);
    }
}
