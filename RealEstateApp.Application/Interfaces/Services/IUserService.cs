using RealEstateApp.Application.Dtos.Auth;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(string userId);
        Task<string> GetUserFullNameAsync(string userId);
    }
}