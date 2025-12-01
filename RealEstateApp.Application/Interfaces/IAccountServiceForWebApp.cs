using RealEstateApp.Application.Dtos.Auth;
using RealEstateApp.Application;

namespace RealEstateApp.Application.Interfaces
{
    public interface IAccountServiceForWebApp : IBaseAccountService
    {
        Task<Result<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<bool> LogoutAsync();
        Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, string? origin);
    }
}

