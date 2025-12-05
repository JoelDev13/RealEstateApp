using Microsoft.AspNetCore.Identity;
using RealEstateApp.Application.Dtos.AdminUsers;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infraestructure.Identity.Repositories
{
    public class AdminUserRepository : IAdminUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminUserRepository(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<AdminUserDto>> GetAllAdminsAsync()
        {
            var adminRoleName = Roles.Administrador.ToString();

            var admins = await _userManager.GetUsersInRoleAsync(adminRoleName);

            return admins.Select(u => new AdminUserDto
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

        public async Task<AdminUserDto?> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return null;

            return new AdminUserDto
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
        public async Task<string> CreateAdminAsync(AdminUserCreateDto dto)
        {
            var adminRoleName = Roles.Administrador.ToString();

            var user = new AppUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Cedula = dto.Cedula,
                Email = dto.Email,
                UserName = dto.UserName,
                IsActive = true,
                UserType = adminRoleName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Error creando administrador: {errors}");
            }

            var roleExists = await _roleManager.RoleExistsAsync(adminRoleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(adminRoleName));
            }

            await _userManager.AddToRoleAsync(user, adminRoleName);

            return user.Id;
        }

        public async Task UpdateAdminAsync(AdminUserUpdateDto dto)
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
                throw new Exception($"Error actualizando administrador: {errors}");
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
                throw new Exception($"Error cambiando estado del usuario: {errors}");
            }
        }
    }
}
