using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Api.Handlers;
using RealEstateApp.Application;
using RealEstateApp.Application.Dtos.Auth;
using RealEstateApp.Application.Interfaces;
using RealEstateApp.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateApp.Api.Controllers.v1
{
    [ApiVersion("1.0")]
    [SwaggerTag("Endpoints para el registro, autenticación y recuperación de cuentas")]
    public class AccountController : BaseApiController
    {
        private readonly IAccountServiceForWebApi _accountServiceForWebApi;

        public AccountController(IAccountServiceForWebApi accountServiceForWebApi)
        {
            _accountServiceForWebApi = accountServiceForWebApi;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Autentica un usuario",
            Description = "Valida las credenciales recibidas y devuelve un JWT en caso de que sean correctas"
        )]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _accountServiceForWebApi.AuthenticateAsync(dto);

            if (!result.Succeeded)
            {
                return BadRequest400WithErrorMessagesFromResult(result);
            }

            return Ok(new { token = result.Data });
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("register-admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Registra un nuevo administrador en el sistema",
            Description = "Crea un nuevo administrador en el sistema. Se puede enviar una imagen para este usuario"
        )]
        public async Task<IActionResult> RegisterAdmin([FromForm] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var save = new UserSaveDto
            {
                Id = "",
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.UserName,
                Password = dto.Password,
                Cedula = dto.Cedula,
                PhoneNumber = dto.Phone,
                UserType = nameof(Roles.Administrador),
            };

            var result = await _accountServiceForWebApi.RegisterUser(save, null, true);

            if (!result.Succeeded)
            {
                return BadRequest400WithErrorMessagesFromResult(result);
            }

            save.Id = result.Data!.Id;
            save.ProfilePicture = FileHandler.Upload(dto.ProfileImage, save.Id, "users");

            var resultEdit = await _accountServiceForWebApi.EditUser(save, null, true);

            if (!resultEdit.Succeeded)
            {
                return BadRequest400WithErrorMessagesFromResult(resultEdit);
            }

            return StatusCode(StatusCodes.Status201Created);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("register-agent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Registra un nuevo agente",
            Description = "Crea un nuevo agente en el sistema. Se puede enviar una imagen para el usuario"
        )]
        public async Task<IActionResult> RegisterAgent([FromForm] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var save = new UserSaveDto
            {
                Id = "",
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.UserName,
                Password = dto.Password,
                Cedula = dto.Cedula,
                PhoneNumber = dto.Phone,
                UserType = nameof(Roles.Agente),
            };

            var result = await _accountServiceForWebApi.RegisterUser(save, null, true);

            if (!result.Succeeded)
            {
                return BadRequest400WithErrorMessagesFromResult(result);
            }

            save.Id = result.Data!.Id;
            save.ProfilePicture = FileHandler.Upload(dto.ProfileImage, save.Id, "users");

            var resultEdit = await _accountServiceForWebApi.EditUser(save, null, true);

            if (!resultEdit.Succeeded)
            {
                return BadRequest400WithErrorMessagesFromResult(resultEdit);
            }

            return StatusCode(StatusCodes.Status201Created);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("get-reset-token")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Genera un nuevo token con el que resetear la contraseña",
            Description = "Genera un nuevo token para cambiar la contraseña de una cuenta. Esta se envía por Email"
        )]
        public async Task<IActionResult> GetResetToken([FromBody] ForgotPasswordApiRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _accountServiceForWebApi.ForgotPasswordAsync(
                new ForgotPasswordRequestDto
                {
                    UserName = dto.UserName,
                    Origin = ""
                }, true);

            if (!result.Succeeded)
            {
                return BadRequest400WithErrorMessagesFromResult(result);
            }

            return NoContent();
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Resetea la contraseña del usuario",
            Description = "Resetea la contraseña del usuario con el token enviado"
        )]
        public async Task<IActionResult> ChangePassword([FromBody] ResetPasswordRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _accountServiceForWebApi.ResetPasswordAsync(dto);

            if (!result.Succeeded)
            {
                return BadRequest400WithErrorMessagesFromResult(result);
            }

            return NoContent();
        }
    }
}

