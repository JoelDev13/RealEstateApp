using RealEstateApp.Application.Dtos.DeveloperUsers;
using RealEstateApp.Application.Exceptions;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Application.Services
{
    public class DeveloperUserService : IDeveloperUserService
    {
        private readonly IDeveloperUserRepository _developerRepo;

        public DeveloperUserService(IDeveloperUserRepository developerRepo)
        {
            _developerRepo = developerRepo;
        }

        public Task<List<DeveloperUserDto>> GetAllAsync()
            => _developerRepo.GetAllDevelopersAsync();

        public Task<DeveloperUserDto?> GetByIdAsync(string id)
            => _developerRepo.GetByIdAsync(id);

        public Task<string> CreateAsync(DeveloperUserCreateDto dto)
            => _developerRepo.CreateDeveloperAsync(dto);

        public async Task UpdateAsync(DeveloperUserUpdateDto dto, string currentUserId)
        {
            if (dto.Id == currentUserId)
                throw new ApiException("No puedes editar tu propio usuario desarrollador.", 400);

            await _developerRepo.UpdateDeveloperAsync(dto);
        }

        public async Task SetActiveStatusAsync(string id, bool isActive, string currentUserId)
        {
            if (id == currentUserId)
                throw new ApiException("No puedes cambiar el estado de tu propio usuario desarrollador.", 400);

            await _developerRepo.SetActiveStatusAsync(id, isActive);
        }
    }
}
