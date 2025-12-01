using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RealEstateApp.Application;
using RealEstateApp.Application.Dtos.Auth;
using RealEstateApp.Application.Interfaces;
using RealEstateApp.Infraestructure.Identity.Entities;

namespace RealEstateApp.Infraestructure.Identity.Services
{
    public class AccountServiceForWebApp : BaseAccountService, IAccountServiceForWebApp
    {
        public AccountServiceForWebApp(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IEmailService emailService,
            IMapper mapper)
            : base(userManager, signInManager, emailService, mapper)
        {
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var response = new AuthResponseDto();

            try
            {
                // Busca al usuario por email o nombre de usuario
                var user = await _userManager.FindByEmailAsync(loginDto.EmailOrUserName)
                    ?? await _userManager.FindByNameAsync(loginDto.EmailOrUserName);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Usuario o contraseña incorrectos";
                    response.Errors = new List<string> { "Credenciales inválidas" };
                    return Result<AuthResponseDto>.Fail("Usuario o contraseña incorrectos");
                }

                // Verifica si el usuario esta activo
                if (!user.IsActive)
                {
                    response.Success = false;
                    response.Message = "Tu cuenta está inactiva. Por favor, contacta al administrador o activa tu cuenta mediante el correo electrónico";
                    response.Errors = new List<string> { "Usuario inactivo" };
                    return Result<AuthResponseDto>.Fail("Tu cuenta está inactiva. Por favor, contacta al administrador o activa tu cuenta mediante el correo electrónico");
                }

                // Intenta iniciar sesión...
                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName!,
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
                    return Result<AuthResponseDto>.Ok(response);
                }
                else if (result.IsLockedOut)
                {
                    response.Success = false;
                    response.Message = "Tu cuenta ha sido bloqueada temporalmente. Intenta más tarde";
                    response.Errors = new List<string> { "Cuenta bloqueada" };
                    return Result<AuthResponseDto>.Fail("Tu cuenta ha sido bloqueada temporalmente. Intenta más tarde");
                }
                else if (result.IsNotAllowed)
                {
                    response.Success = false;
                    response.Message = "No tienes permiso para iniciar sesión. Verifica tu correo electrónico.";
                    response.Errors = new List<string> { "Acceso no permitido" };
                    return Result<AuthResponseDto>.Fail("No tienes permiso para iniciar sesión. Verifica tu correo electrónico");
                }
                else
                {
                    response.Success = false;
                    response.Message = "Usuario o contraseña incorrectos";
                    response.Errors = new List<string> { "Credenciales inválidas" };
                    return Result<AuthResponseDto>.Fail("Usuario o contraseña incorrectos");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error al iniciar sesión";
                response.Errors = new List<string> { ex.Message };
                return Result<AuthResponseDto>.Fail($"Error al iniciar sesión: {ex.Message}");
            }
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

        public async Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, string? origin)
        {
            try
            {
                // Converti RegisterDto a UserSaveDto
                var userSaveDto = new UserSaveDto
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email,
                    Password = registerDto.Password,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    PhoneNumber = registerDto.PhoneNumber,
                    UserType = registerDto.UserType
                };

                // Usa el metodo del BaseAccountService
                var result = await RegisterUser(userSaveDto, origin, false);

                if (result.Succeeded)
                {
                    return Result<UserDto>.Ok(result.Data!);
                }
                else
                {
                    return Result<UserDto>.Fail(result.Errors ?? new List<string> { "Error al registrar el usuario" });
                }
            }
            catch (Exception ex)
            {
                return Result<UserDto>.Fail($"Error al registrar el usuario: {ex.Message}");
            }
        }
    }
}
