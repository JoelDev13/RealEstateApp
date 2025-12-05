using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealEstateApp.Application.Dtos.Auth;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Domain.Settings;
using RealEstateApp.Infrastructure.Identity.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealEstateApp.Infrastructure.Identity.Services
{
    public class AccountApiService : IAccountApiService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtSettings _jwtSettings;

        public AccountApiService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResponseDto> LoginAsync(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName)
                       ?? throw new Exception("Usuario o contraseña incorrectos.");

            var result = await _signInManager.PasswordSignInAsync(user.UserName ?? "", password, false, true);
            if (!result.Succeeded)
            {
                var msg = result.IsLockedOut
                    ? "Cuenta bloqueada por intentos fallidos. Intenta de nuevo en 10 minutos."
                    : "Credenciales inválidas.";

                throw new Exception(msg);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);

            return new AuthResponseDto
            {
                Success = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? Roles.Cliente.ToString(),
                Message = "Login exitoso"
            };
        }

        public async Task<string> RegisterDeveloperAsync(RegisterDto dto)
        {
            var user = new AppUser
            {
                UserName = dto.UserName ?? string.Empty,
                Email = dto.Email ?? string.Empty,
                FirstName = dto.FirstName ?? string.Empty,
                LastName = dto.LastName ?? string.Empty,
                Cedula = dto.Cedula ?? string.Empty,
                PhoneNumber = dto.PhoneNumber,
                UserType = Roles.Desarrollador.ToString(),
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password ?? string.Empty);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            if (!await _roleManager.RoleExistsAsync(Roles.Desarrollador.ToString()))
                await _roleManager.CreateAsync(new IdentityRole(Roles.Desarrollador.ToString()));

            await _userManager.AddToRoleAsync(user, Roles.Desarrollador.ToString());

            return user.Id ?? string.Empty;
        }

        public async Task<string> RegisterAdminAsync(RegisterDto dto, string currentAdminId)
        {
            var currentAdmin = await _userManager.FindByIdAsync(currentAdminId)
                               ?? throw new Exception("Usuario administrador no encontrado.");

            var roles = await _userManager.GetRolesAsync(currentAdmin);
            if (!roles.Contains(Roles.Administrador.ToString()))
                throw new Exception("No tienes permisos para crear administradores.");

            var user = new AppUser
            {
                UserName = dto.UserName ?? string.Empty,
                Email = dto.Email ?? string.Empty,
                FirstName = dto.FirstName ?? string.Empty,
                LastName = dto.LastName ?? string.Empty,
                Cedula = dto.Cedula ?? string.Empty,
                PhoneNumber = dto.PhoneNumber,
                UserType = Roles.Administrador.ToString(),
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password ?? string.Empty);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            if (!await _roleManager.RoleExistsAsync(Roles.Administrador.ToString()))
                await _roleManager.CreateAsync(new IdentityRole(Roles.Administrador.ToString()));

            await _userManager.AddToRoleAsync(user, Roles.Administrador.ToString());

            return user.Id ?? string.Empty;
        }

        private JwtSecurityToken GenerateJwtToken(AppUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim("uid", user.Id ?? string.Empty)
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: creds
            );

            return token;
        }
    }
}
