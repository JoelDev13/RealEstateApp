using RealEstateApp.Application.Dtos.DeveloperUsers;

public interface IDeveloperUserRepository
{
    Task<List<DeveloperUserDto>> GetAllDevelopersAsync();
    Task<DeveloperUserDto?> GetByIdAsync(string id);
    Task<string> CreateDeveloperAsync(DeveloperUserCreateDto dto);
    Task UpdateDeveloperAsync(DeveloperUserUpdateDto dto);
    Task SetActiveStatusAsync(string id, bool isActive);
}
