using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Dtos.Auth;
using RealEstateApp.Application.Dtos.Email;
using RealEstateApp.Application.Interfaces;
using RealEstateApp.Application;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Infraestructure.Identity.Entities;

namespace RealEstateApp.Infraestructure.Identity.Services
{
    public class BaseAccountService : IBaseAccountService
    {
        protected readonly UserManager<AppUser> _userManager;
        protected readonly SignInManager<AppUser> _signInManager;
        protected readonly IMapper _mapper;
        protected readonly IEmailService _emailService;

        public BaseAccountService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IEmailService emailService,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _mapper = mapper;
        }

        public virtual async Task<Result<UserDto>> RegisterUser(UserSaveDto saveDto, string? origin, bool? isApi = false)
        {
            // Valida que la cédula tenga exactamente 11 digitos
            if (string.IsNullOrWhiteSpace(saveDto.Cedula))
            {
                return Result<UserDto>.Fail("La cédula es requerida.");
            }

            // Remueve los guiones o espacios si los tiene
            string cedulaClean = saveDto.Cedula.Replace("-", "").Replace(" ", "").Trim();
            
            if (cedulaClean.Length != 11 || !cedulaClean.All(char.IsDigit))
            {
                return Result<UserDto>.Fail("La cédula debe tener exactamente 11 dígitos numéricos");
            }

            // Verifica si ya existe un usuario con esta cedula
            var userWithSameCedula = await _userManager.Users.FirstOrDefaultAsync(u => u.Cedula == cedulaClean);
            if (userWithSameCedula != null)
            {
                return Result<UserDto>.Fail($"Ya existe un usuario registrado con la cédula: {cedulaClean}.");
            }

            var userWithSameUserName = await _userManager.FindByNameAsync(saveDto.UserName);
            if (userWithSameUserName != null)
            {
                return Result<UserDto>.Fail($"El nombre de usuario: {saveDto.UserName} ya está en uso");
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(saveDto.Email);
            if (userWithSameEmail != null)
            {
                return Result<UserDto>.Fail($"El correo electrónico: {saveDto.Email} ya está en uso");
            }

            // limpieza de cedula
            saveDto.Cedula = cedulaClean;

            // Clientes y agentes se crean inactivos. Los clientes reciben correos, los agentes no
            // Los administradores se crean activos, no reciben correos
            var user = _mapper.Map<AppUser>(saveDto);
            user.ProfilePicture ??= string.Empty;
            user.IsActive = saveDto.UserType == nameof(Roles.Administrador);
            user.EmailConfirmed = saveDto.UserType == nameof(Roles.Administrador);
            user.CreatedAt = DateTime.UtcNow;

            var result = await _userManager.CreateAsync(user, saveDto.Password!);
            if (!result.Succeeded)
            {
                return Result<UserDto>.Fail(result.Errors.Select(s => s.Description).ToList());
            }

            // Asigna el rol segun UserType
            string roleName = saveDto.UserType switch
            {
                "Cliente" => nameof(Roles.Cliente),
                "Agente" => nameof(Roles.Agente),
                "Administrador" => nameof(Roles.Administrador),
                _ => nameof(Roles.Cliente)
            };

            await _userManager.AddToRoleAsync(user, roleName);

            // El unico rol que se verifica por correo es Cliente
            if (saveDto.UserType == nameof(Roles.Cliente))
            {
                string verificationUri = await GetVerificationEmailUri(user, origin ?? "");
                string htmlBody = $@"
                <p>Hola {saveDto.FirstName},</p>
                <p>¡Bienvenido a RealEstateApp!</p>
                <p>Por favor confirma tu correo haciendo clic aquí: <a href='{verificationUri}'>Confirmar correo</a></p>
                ";
                await _emailService.SendAsync(new EmailRequestDto()
                {
                    To = saveDto.Email,
                    HtmlBody = htmlBody,
                    Subject = "Confirma tu cuenta en RealEstateApp"
                });
            }

            var rolesList = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Role = rolesList.FirstOrDefault();

            return Result<UserDto>.Ok(userDto);
        }

        public virtual async Task<Result<UserDto>> EditUser(UserSaveDto saveDto, string? origin, bool? isApi = false)
        {
            var userWithSameUserName = await _userManager.Users
                .FirstOrDefaultAsync(w => w.UserName == saveDto.UserName && w.Id != saveDto.Id);
            if (userWithSameUserName != null)
            {
                return Result<UserDto>.Fail($"El nombre de usuario: {saveDto.UserName} ya está en uso");
            }

            var userWithSameEmail = await _userManager.Users
                .FirstOrDefaultAsync(w => w.Email == saveDto.Email && w.Id != saveDto.Id);
            if (userWithSameEmail != null)
            {
                return Result<UserDto>.Fail($"El correo electrónico: {saveDto.Email} ya está en uso");
            }

            var user = await _userManager.FindByIdAsync(saveDto.Id!);
            if (user == null)
            {
                return Result<UserDto>.Fail("No existe una cuenta registrada con este usuario");
            }

            // Validar cédula si se está actualizando
            if (!string.IsNullOrWhiteSpace(saveDto.Cedula))
            {
                string cedulaClean = saveDto.Cedula.Replace("-", "").Replace(" ", "").Trim();
                
                if (cedulaClean.Length != 11 || !cedulaClean.All(char.IsDigit))
                {
                    return Result<UserDto>.Fail("La cédula debe tener exactamente 11 dígitos numéricos.");
                }

                // Verificar si otra cédula ya existe (excluyendo el usuario actual)
                var userWithSameCedula = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.Cedula == cedulaClean && u.Id != saveDto.Id);
                if (userWithSameCedula != null)
                {
                    return Result<UserDto>.Fail($"Ya existe un usuario registrado con la cédula: {cedulaClean}.");
                }

                user.Cedula = cedulaClean;
            }

            user.FirstName = saveDto.FirstName ?? user.FirstName;
            user.LastName = saveDto.LastName ?? user.LastName;
            user.ProfilePicture = string.IsNullOrWhiteSpace(saveDto.ProfilePicture) ? user.ProfilePicture : saveDto.ProfilePicture;
            user.PhoneNumber = string.IsNullOrWhiteSpace(saveDto.PhoneNumber) ? user.PhoneNumber : saveDto.PhoneNumber;
            user.UserName = saveDto.UserName;
            user.EmailConfirmed = user.EmailConfirmed && user.Email == saveDto.Email;
            user.Email = saveDto.Email;
            user.UserType = saveDto.UserType;
            user.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(saveDto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resultChange = await _userManager.ResetPasswordAsync(user, token, saveDto.Password);

                if (!resultChange.Succeeded)
                {
                    return Result<UserDto>.Fail(resultChange.Errors.Select(s => s.Description).ToList());
                }
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return Result<UserDto>.Fail(result.Errors.Select(s => s.Description).ToList());
            }

            var rolesList = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, rolesList.ToList());

            string roleName = saveDto.UserType switch
            {
                "Cliente" => nameof(Roles.Cliente),
                "Agente" => nameof(Roles.Agente),
                "Administrador" => nameof(Roles.Administrador),
                _ => nameof(Roles.Cliente)
            };

            await _userManager.AddToRoleAsync(user, roleName);

            var updatedRolesList = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Role = updatedRolesList.FirstOrDefault();

            return Result<UserDto>.Ok(userDto);
        }

        public virtual async Task<Result<UserDto>> LoginAsync(LoginRequestDto request)
        {
            // Busca el usuario por username o email
            var user = await _userManager.FindByNameAsync(request.UserName) ?? 
                      await _userManager.FindByEmailAsync(request.UserName);

            if (user == null)
            {
                return Result<UserDto>.Fail("Usuario o contraseña incorrectos");
            }

            // Verifica si el usuario esta activo
            if (!user.IsActive)
            {
                return Result<UserDto>.Fail("Tu cuenta no está activa. Por favor verifica tu correo electrónico o contacta al administrador");
            }

            // Intenta el login
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName, 
                request.Password, 
                isPersistent: false, 
                lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    return Result<UserDto>.Fail("Cuenta bloqueada por múltiples intentos fallidos. Por favor intenta más tarde");
                }
                else if (result.IsNotAllowed)
                {
                    return Result<UserDto>.Fail("No se permite el acceso a esta cuenta. Por favor verifica tu correo electrónico");
                }
                else
                {
                    return Result<UserDto>.Fail("Usuario o contraseña incorrectos");
                }
            }

            // Obtiene los roles del usuario
            var rolesList = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Role = rolesList.FirstOrDefault();

            return Result<UserDto>.Ok(userDto);
        }

        public virtual async Task<Result> ForgotPasswordAsync(ForgotPasswordRequestDto request, bool? isApi = false)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return Result.Fail($"No existe una cuenta registrada con este nombre de usuario: {request.UserName}");
            }

            user.EmailConfirmed = false;
            await _userManager.UpdateAsync(user);

            if (isApi != null && !isApi.Value)
            {
                var resetUri = await GetResetPasswordUri(user, request.Origin ?? "");
                await _emailService.SendAsync(new EmailRequestDto()
                {
                    To = user.Email!,
                    HtmlBody = $"Por favor restablece tu contraseña visitando esta URL: {resetUri}",
                    Subject = "Restablecer contraseña"
                });
            }
            else
            {
                string? resetToken = await GetResetPasswordToken(user);
                await _emailService.SendAsync(new EmailRequestDto()
                {
                    To = user.Email!,
                    HtmlBody = $"Por favor restablece tu contraseña usando este token: {resetToken}",
                    Subject = "Restablecer contraseña"
                });
            }

            return Result.Ok();
        }

        public virtual async Task<Result> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return Result.Fail("No existe una cuenta registrada con este usuario");
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var result = await _userManager.ResetPasswordAsync(user, token, request.Password);
            if (!result.Succeeded)
            {
                return Result.Fail(result.Errors.Select(s => s.Description).ToList());
            }

            user.EmailConfirmed = true;
            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            return Result.Ok();
        }

        public virtual async Task<Result> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Result.Fail("No existe una cuenta registrada con este usuario");
            }

            await _userManager.DeleteAsync(user);
            return Result.Ok();
        }

        public virtual async Task<UserDto?> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            var rolesList = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Role = rolesList.FirstOrDefault();
            return userDto;
        }

        public virtual async Task<UserDto?> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            var rolesList = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Role = rolesList.FirstOrDefault();
            return userDto;
        }

        public virtual async Task<List<UserDto>> GetUsersByIds(IEnumerable<string> ids)
        {
            var users = await _userManager.Users.Where(u => ids.Contains(u.Id)).ToListAsync();

            var usersDtos = new List<UserDto>();
            foreach (var user in users)
            {
                var rolesList = await _userManager.GetRolesAsync(user);
                var userDto = _mapper.Map<UserDto>(user);
                userDto.Role = rolesList.FirstOrDefault();
                usersDtos.Add(userDto);
            }

            return usersDtos;
        }

        public virtual async Task<UserDto?> GetUserByUserName(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return null;
            }

            var rolesList = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Role = rolesList.FirstOrDefault();
            return userDto;
        }

        public virtual async Task<List<UserDto>> GetAllUser(bool? isActive = true)
        {
            List<UserDto> listUsersDtos = new();

            var users = _userManager.Users;

            if (isActive != null && isActive == true)
            {
                users = users.Where(w => w.IsActive);
            }

            var listUser = await users.ToListAsync();

            foreach (var user in listUser)
            {
                var roleList = await _userManager.GetRolesAsync(user);
                var userDto = _mapper.Map<UserDto>(user);
                userDto.Role = roleList.FirstOrDefault();
                listUsersDtos.Add(userDto);
            }

            return listUsersDtos;
        }

        public async Task<List<UserDto>> GetAllUserOfRole(Roles role, bool isActive = true)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.ToString());
            if (isActive)
            {
                usersInRole = usersInRole.Where(u => u.IsActive).ToList();
            }

            List<UserDto> listUsersDtos = new();
            foreach (var user in usersInRole)
            {
                var userDto = _mapper.Map<UserDto>(user);
                userDto.Role = role.ToString();
                listUsersDtos.Add(userDto);
            }

            return listUsersDtos;
        }

        public async Task<List<string>> GetAllUserIdsOfRole(Roles role, bool isActive = true)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.ToString());
            if (isActive)
            {
                usersInRole = usersInRole.Where(u => u.IsActive).ToList();
            }

            return usersInRole.Select(u => u.Id).ToList();
        }

        public async Task<int> CountUsers(Roles? role, bool? onlyActive = null)
        {
            List<AppUser> users;

            if (role != null)
                users = (await _userManager.GetUsersInRoleAsync(role.ToString())).ToList();
            else
                users = await _userManager.Users.ToListAsync();

            if (onlyActive == true)
                return users.Count(u => u.IsActive);

            if (onlyActive == false)
                return users.Count(u => !u.IsActive);

            return users.Count;
        }

        public async Task<List<string>> GetAllUsersIds(bool isActive = true)
        {
            var users = _userManager.Users;
            if (isActive)
            {
                users = users.Where(u => u.IsActive);
            }

            var usersIds = await users.Select(u => u.Id).ToListAsync();
            return usersIds;
        }

        public virtual async Task<Result> ConfirmAccountAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Fail("No existe una cuenta registrada con este usuario");
            }

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return Result.Fail($"Ocurrió un error al confirmar el correo electrónico {user.Email}");
            }

            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            return Result.Ok();
        }

        public async Task<Result> SetStateOnUser(string userId, bool state)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Fail($"No existe una cuenta registrada con este usuario: {userId}");

            user.IsActive = state;
            user.EmailConfirmed = state;
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return Result.Fail(updateResult.Errors.Select(s => s.Description).ToList());
            }

            return Result.Ok();
        }

        public async Task<bool> ThisEmailExists(string email, string? id = null)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                return await _userManager.Users.AnyAsync(u => u.Email == email && u.Id != id);
            }

            return await _userManager.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ThisUsernameExists(string userName, string? id = null)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                return await _userManager.Users.AnyAsync(u => u.UserName == userName && u.Id != id);
            }

            return await _userManager.Users.AnyAsync(u => u.UserName == userName);
        }

        #region "Protected methods"

        protected async Task<string> GetVerificationEmailUri(AppUser user, string origin)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var route = "Account/ConfirmEmail";
            var completeUrl = new Uri(string.Concat(origin, "/", route));
            var verificationUri = QueryHelpers.AddQueryString(completeUrl.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "token", token);

            return verificationUri;
        }

        protected async Task<string?> GetVerificationEmailToken(AppUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            return token;
        }

        protected async Task<string> GetResetPasswordUri(AppUser user, string origin)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var route = "Account/ResetPassword";
            var completeUrl = new Uri(string.Concat(origin, "/", route));
            var resetUri = QueryHelpers.AddQueryString(completeUrl.ToString(), "userId", user.Id);
            resetUri = QueryHelpers.AddQueryString(resetUri, "token", token);

            return resetUri;
        }

        protected async Task<string?> GetResetPasswordToken(AppUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            return token;
        }

        #endregion
    }
}

