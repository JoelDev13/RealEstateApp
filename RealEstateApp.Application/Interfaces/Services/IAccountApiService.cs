using RealEstateApp.Application.Dtos.Auth;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IAccountApiService
    {
        Task<AuthResponseDto> LoginAsync(string userName, string password);
        Task<string> RegisterDeveloperAsync(RegisterDto dto);
        Task<string> RegisterAdminAsync(RegisterDto dto, string currentAdminId);
    }
}
