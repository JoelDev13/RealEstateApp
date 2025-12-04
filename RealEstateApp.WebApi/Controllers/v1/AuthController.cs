using Asp.Versioning;
using InvestmentApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.Auth;
using RealEstateApp.Application.Features.Auth.Commands.Login;
using RealEstateApp.Application.Features.Auth.Commands.RegisterAdmin;
using RealEstateApp.Application.Features.Auth.Commands.RegisterDeveloper;

namespace RealEstateApp.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {

        /// <param name="command">LoginCommand con UserName y Password</param>
        /// <returns>AuthResponseDto con Token, Role y datos del usuario</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <param name="command">RegisterDeveloperCommand con datos del usuario</param>
        /// <returns>Id del usuario creado</returns>
        [HttpPost("register-developer")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<string>> RegisterDeveloper([FromBody] RegisterDeveloperCommand command)
        {
            var userId = await Mediator.Send(command);
            return Ok(userId);
        }

        /// <param name="command">RegisterAdminCommand con datos del usuario</param>
        /// <returns>Id del usuario creado</returns>
        [Authorize(Roles = "Administrador")]
        [HttpPost("register-admin")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<string>> RegisterAdmin([FromBody] RegisterAdminCommand command)
        {
            command.CurrentAdminId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            var userId = await Mediator.Send(command);
            return Ok(userId);
        }
    }
}
