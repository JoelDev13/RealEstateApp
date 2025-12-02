using RealEstateApp.Application.Dtos.AdminUsers;

namespace RealEstateApp.Application.Interfaces.Repositories
{
    public interface IAdminUserRepository
    {
        Task<List<AdminUserDto>> GetAllAdminsAsync();
        Task<AdminUserDto?> GetByIdAsync(string id);

        Task<string> CreateAdminAsync(AdminUserCreateDto dto);
        Task UpdateAdminAsync(AdminUserUpdateDto dto);

        Task SetActiveStatusAsync(string id, bool isActive);
    }
}
