using RealEstateApp.Application.Dtos.Auth;
using RealEstateApp.Domain.Enums;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IBaseAccountService
    {
        Task<Result<UserDto>> RegisterUser(UserSaveDto saveDto, string? origin, bool? isApi = false);
        Task<Result<UserDto>> EditUser(UserSaveDto saveDto, string? origin, bool? isApi = false);
        Task<Result<UserDto>> LoginAsync(LoginRequestDto request);
        Task<Result> ForgotPasswordAsync(ForgotPasswordRequestDto request, bool? isApi = false);
        Task<Result> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<Result> DeleteAsync(string id);
        Task<UserDto?> GetUserByEmail(string email);
        Task<UserDto?> GetUserById(string id);
        Task<List<UserDto>> GetUsersByIds(IEnumerable<string> ids);
        Task<UserDto?> GetUserByUserName(string userName);
        Task<List<UserDto>> GetAllUser(bool? isActive = true);
        Task<List<UserDto>> GetAllUserOfRole(Roles role, bool isActive = true);
        Task<List<string>> GetAllUserIdsOfRole(Roles role, bool isActive = true);
        Task<int> CountUsers(Roles? role, bool? onlyActive = null);
        Task<List<string>> GetAllUsersIds(bool isActive = true);
        Task<Result> ConfirmAccountAsync(string userId, string token);
        Task<Result> SetStateOnUser(string userId, bool state);
        Task<bool> ThisEmailExists(string email, string? id = null);
        Task<bool> ThisUsernameExists(string userName, string? id = null);
    }
}

