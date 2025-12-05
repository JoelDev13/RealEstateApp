using Microsoft.AspNetCore.Identity;
using RealEstateApp.Application.Dtos.DeveloperUsers;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;


namespace RealEstateApp.Infrastructure.Identity.Repositories
{

    public class DeveloperUserRepository : IDeveloperUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private const string DeveloperRoleName = nameof(Roles.Desarrollador);
        public DeveloperUserRepository(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<DeveloperUserDto>> GetAllDevelopersAsync()
        {
            var devs = await _userManager.GetUsersInRoleAsync(DeveloperRoleName);

            return devs.Select(u => new DeveloperUserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName!,
                Cedula = u.Cedula,
                Email = u.Email!,
                IsActive = u.IsActive
            }).ToList();
        }

        public async Task<DeveloperUserDto?> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return null;

            return new DeveloperUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName!,
                Cedula = user.Cedula,
                Email = user.Email!,
                IsActive = user.IsActive
            };
        }

        public async Task<string> CreateDeveloperAsync(DeveloperUserCreateDto dto)
        {
            var user = new AppUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Cedula = dto.Cedula,
                Email = dto.Email,
                UserName = dto.UserName,
                IsActive = true,
                UserType = DeveloperRoleName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Error creando desarrollador: {errors}");
            }

            var roleExists = await _roleManager.RoleExistsAsync(DeveloperRoleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(DeveloperRoleName));
            }

            await _userManager.AddToRoleAsync(user, DeveloperRoleName);

            return user.Id;
        }
        public async Task UpdateDeveloperAsync(DeveloperUserUpdateDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Id)
                       ?? throw new KeyNotFoundException("Usuario no encontrado");

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Cedula = dto.Cedula;
            user.Email = dto.Email;
            user.UserName = dto.UserName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Error actualizando desarrollador: {errors}");
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, dto.Password);

                if (!passResult.Succeeded)
                {
                    var errors = string.Join(", ", passResult.Errors.Select(e => e.Description));
                    throw new Exception($"Error cambiando contraseña: {errors}");
                }
            }
        }

        public async Task SetActiveStatusAsync(string id, bool isActive)
        {
            var user = await _userManager.FindByIdAsync(id)
                       ?? throw new KeyNotFoundException("Usuario no encontrado");

            user.IsActive = isActive;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Error cambiando estado del desarrollador: {errors}");
            }
        }
    }
}
