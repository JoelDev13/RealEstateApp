using Asp.Versioning;
using RealEstateApp.WebApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.Auth;
using RealEstateApp.Application.Features.Auth.Commands.Login;
using RealEstateApp.Application.Features.Auth.Commands.RegisterAdmin;
using RealEstateApp.Application.Features.Auth.Commands.RegisterDeveloper;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateApp.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerOperation(
            Summary = "Autenticar usuario",
            Description = "Valida las credenciales del usuario y devuelve un token JWT junto con la información básica del usuario y su rol."
        )]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("register-developer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerOperation(
            Summary = "Registrar desarrollador",
            Description = "Registra un nuevo usuario con rol Desarrollador en el sistema."
        )]
        public async Task<ActionResult<string>> RegisterDeveloper([FromBody] RegisterDeveloperCommand command)
        {
            var userId = await Mediator.Send(command);
            return Ok(userId);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("register-admin")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [SwaggerOperation(
            Summary = "Registrar administrador",
            Description = "Registra un nuevo usuario con rol Administrador. Solo puede ser ejecutado por un administrador autenticado."
        )]
        public async Task<ActionResult<string>> RegisterAdmin([FromBody] RegisterAdminCommand command)
        {
            command.CurrentAdminId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            var userId = await Mediator.Send(command);
            return Ok(userId);
        }
    }
}
