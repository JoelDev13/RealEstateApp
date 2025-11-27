using Microsoft.AspNetCore.Identity;
using RealEstateApp.Application.Dtos.Auth;
using RealEstateApp.Application.Interfaces.Identity;
using RealEstateApp.Infraestructure.Identity.Entities;

namespace RealEstateApp.Infraestructure.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public IdentityService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var response = new AuthResponseDto();

            try
            {
                // Busca el usuario por email o nombre de usuario
                var user = await _userManager.FindByEmailAsync(loginDto.EmailOrUserName) 
                    ?? await _userManager.FindByNameAsync(loginDto.EmailOrUserName);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Usuario o contraseña incorrectos";
                    response.Errors = new List<string> { "Credenciales inválidas" };
                    return response;
                }

                // Verifica si el usuario esta activo
                if (!user.IsActive)
                {
                    response.Success = false;
                    response.Message = "Tu cuenta está inactiva. Por favor, contacta al administrador o activa tu cuenta mediante el correo electrónico.";
                    response.Errors = new List<string> { "Usuario inactivo" };
                    return response;
                }

                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName,
                    loginDto.Password,
                    loginDto.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    response.Success = true;
                    response.UserId = user.Id;
                    response.UserName = user.UserName;
                    response.Email = user.Email;
                    response.Role = roles.FirstOrDefault();
                    response.Message = "Inicio de sesión exitoso";
                }
                else if (result.IsLockedOut)
                {
                    response.Success = false;
                    response.Message = "Tu cuenta ha sido bloqueada temporalmente. Intenta más tarde.";
                    response.Errors = new List<string> { "Cuenta bloqueada" };
                }
                else if (result.IsNotAllowed)
                {
                    response.Success = false;
                    response.Message = "No tienes permiso para iniciar sesión. Verifica tu correo electrónico.";
                    response.Errors = new List<string> { "Acceso no permitido" };
                }
                else
                {
                    response.Success = false;
                    response.Message = "Usuario o contraseña incorrectos";
                    response.Errors = new List<string> { "Credenciales inválidas" };
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error al iniciar sesión";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var response = new AuthResponseDto();

            try
            {
                // Verifica si el usuario ya existe
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email)
                    ?? await _userManager.FindByNameAsync(registerDto.UserName);

                if (existingUser != null)
                {
                    response.Success = false;
                    response.Message = "El usuario o correo electrónico ya está registrado";
                    response.Errors = new List<string> { "Usuario ya existe" };
                    return response;
                }

                // Crea un nuevo usuario
                var user = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    PhoneNumber = registerDto.PhoneNumber,
                    UserType = registerDto.UserType,
                    IsActive = false, // Se crea inactivo
                    EmailConfirmed = false,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (result.Succeeded)
                {
                    // Asigna el rol segun el tipo de usuario
                    string roleName = registerDto.UserType switch
                    {
                        "Cliente" => "Cliente",
                        "Agente" => "Agente",
                        _ => "Cliente"
                    };

                    await _userManager.AddToRoleAsync(user, roleName);

                    response.Success = true;
                    response.UserId = user.Id;
                    response.UserName = user.UserName;
                    response.Email = user.Email;
                    response.Role = roleName;

                    if (registerDto.UserType == "Cliente")
                    {
                        response.Message = "Registro exitoso. Se ha enviado un correo de activación a tu email";
                    }
                    else if (registerDto.UserType == "Agente")
                    {
                        response.Message = "Registro exitoso. Tu cuenta está pendiente de activación por el administrador";
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "Error al registrar el usuario";
                    response.Errors = result.Errors.Select(e => e.Description).ToList();
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error al registrar el usuario";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user == null ? null : MapToUserDto(user);
        }

        public async Task<UserDto?> GetUserByEmailOrUserNameAsync(string emailOrUserName)
        {
            var user = await _userManager.FindByEmailAsync(emailOrUserName)
                ?? await _userManager.FindByNameAsync(emailOrUserName);
            return user == null ? null : MapToUserDto(user);
        }

        private UserDto MapToUserDto(AppUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ProfilePicture = user.ProfilePicture,
                UserType = user.UserType,
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<bool> IsUserActiveAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.IsActive ?? false;
        }

        public async Task<bool> ActivateUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                user.IsActive = true;
                user.EmailConfirmed = true;
                user.UpdatedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeactivateUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                return result.Succeeded;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null) return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return new List<string>();

                var roles = await _userManager.GetRolesAsync(user);
                return roles.ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        public async Task<bool> IsInRoleAsync(string userId, string role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                return await _userManager.IsInRoleAsync(user, role);
            }
            catch
            {
                return false;
            }
        }
    }
}

