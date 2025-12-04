using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealEstateApp.Application;
using RealEstateApp.Application.Dtos.Auth;
using RealEstateApp.Application.Interfaces;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Domain.Settings;
using RealEstateApp.Infraestructure.Identity.Entities;

namespace RealEstateApp.Infraestructure.Identity.Services
{
    public class AccountServiceForWebApi : BaseAccountService, IAccountServiceForWebApi
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public AccountServiceForWebApi(IOptions<JwtSettings> jwtSettings, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IEmailService emailService, IMapper mapper) : base(userManager, signInManager, emailService, mapper)
        {
            _jwtSettings = jwtSettings.Value;
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
            _mapper = mapper;
        }

        public async Task<Result<string>> AuthenticateAsync(LoginDto loginDto)
        {
            // Busca el usuario por email o nombre de usuario
            var user = await _userManager.FindByEmailAsync(loginDto.EmailOrUserName)
                ?? await _userManager.FindByNameAsync(loginDto.EmailOrUserName);

            if (user == null)
            {
                return Result<string>.Fail($"No hay ninguna cuenta registrada con este usuario: {loginDto.EmailOrUserName}");
            }

            if (!user.EmailConfirmed)
            {
                return Result<string>.Fail($"Esta cuenta {loginDto.EmailOrUserName} no está activa, debes verificar tu correo electrónico");
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName ?? "", loginDto.Password, false, true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    string error =
                        $"Tu cuenta {loginDto.EmailOrUserName} ha sido bloqueada debido a múltiples intentos fallidos." +
                        $" Por favor intenta de nuevo en 10 minutos. Si no recuerdas tu contraseña, " +
                        $"puedes pasar por el proceso de recuperación de contraseñas";

                    return Result<string>.Fail(error);
                }

                return Result<string>.Fail($"Estas credenciales son inválidas para este usuario: {user.UserName}");
            }

            // En este sistema un usuario solamente puede tener un rol
            var role = (await _userManager.GetRolesAsync(user)).SingleOrDefault() ?? "";
            if (role != nameof(Roles.Administrador))
            {
                return Result<string>.Fail("Solo los Administradores pueden usar esta API");
            }

            string token = new JwtSecurityTokenHandler().WriteToken(await GenerateJwtToken(user));
            return Result<string>.Ok(token);
        }

        public override async Task<Result<UserDto>> RegisterUser(UserSaveDto saveDto, string? origin, bool? isApi = false)
        {
            return await base.RegisterUser(saveDto, null, isApi);
        }

        public override async Task<Result<UserDto>> EditUser(UserSaveDto saveDto, string? origin, bool? isApi = false)
        {
            return await base.EditUser(saveDto, null, isApi);
        }

        private async Task<JwtSecurityToken> GenerateJwtToken(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var rolesClaims = new List<Claim>();
            foreach (var role in roles)
            {
                rolesClaims.Add(new Claim("roles", role));
            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim("uid", user.Id ?? "")
            }.Union(userClaims).Union(rolesClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials
            );

            return jwtSecurityToken;
        }
    }
}
