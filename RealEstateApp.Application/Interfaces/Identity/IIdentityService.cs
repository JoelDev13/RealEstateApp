using RealEstateApp.Application.Dtos.Auth;

namespace RealEstateApp.Application.Interfaces.Identity
{
    public interface IIdentityService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<bool> LogoutAsync();
        Task<UserDto?> GetUserByIdAsync(string userId);
        Task<UserDto?> GetUserByEmailOrUserNameAsync(string emailOrUserName);
        Task<bool> IsUserActiveAsync(string userId);
        Task<bool> ActivateUserAsync(string userId);
        Task<bool> DeactivateUserAsync(string userId);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(string email);
        Task<List<string>> GetUserRolesAsync(string userId);
        Task<bool> IsInRoleAsync(string userId, string role);
    }
}

