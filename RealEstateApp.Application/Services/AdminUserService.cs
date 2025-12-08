using RealEstateApp.Application.Dtos.AdminUsers;
using RealEstateApp.Application.Exceptions;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Application.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IAdminUserRepository _adminUserRepository;

        public AdminUserService(IAdminUserRepository adminUserRepository)
        {
            _adminUserRepository = adminUserRepository;
        }

        public Task<List<AdminUserDto>> GetAllAsync()
        {
            return _adminUserRepository.GetAllAdminsAsync();
        }

        public Task<AdminUserDto?> GetByIdAsync(string id)
        {
            return _adminUserRepository.GetByIdAsync(id);
        }

        public Task<string> CreateAsync(AdminUserCreateDto dto)
        {
            return _adminUserRepository.CreateAdminAsync(dto);
        }

        public async Task UpdateAsync(AdminUserUpdateDto dto, string currentUserId)
        {
            if (dto.Id == currentUserId)
            {
                throw new ApiException("No puede editar su propio usuario administrador.", 400);
            }

            await _adminUserRepository.UpdateAdminAsync(dto);
        }

        public async Task SetActiveStatusAsync(string id, bool isActive, string currentUserId)
        {
            if (id == currentUserId)
            {
                throw new ApiException("No puede cambiar el estado de su propio usuario administrador.", 400);
            }

            await _adminUserRepository.SetActiveStatusAsync(id, isActive);
        }
    }
}
