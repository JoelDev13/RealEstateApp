using RealEstateApp.Application.Dtos.Auth;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IAccountServiceForWebApp : IBaseAccountService
    {
        Task<Result<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<bool> LogoutAsync();
        Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, string? origin);
    }
}

