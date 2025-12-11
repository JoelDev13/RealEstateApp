using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RealEstateApp.Application.Dtos.Auth;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public UserService(
            UserManager<AppUser> userManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserDto?> GetUserByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            return _mapper.Map<UserDto>(user);
        }

        public async Task<string> GetUserFullNameAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return $"Usuario desconocido";

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return $"Usuario {userId.Substring(0, Math.Min(8, userId.Length))}";

            var fullName = $"{user.FirstName} {user.LastName}".Trim();
            return string.IsNullOrWhiteSpace(fullName)
                ? user.UserName ?? $"Usuario {userId.Substring(0, 8)}"
                : fullName;
        }
    }
}