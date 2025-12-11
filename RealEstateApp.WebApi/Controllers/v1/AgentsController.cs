using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateApp.WebApi.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class AgentsController : ControllerBase
    {
        private readonly IAgentQueryService _agentQueryService;

        public AgentsController(IAgentQueryService agentQueryService)
        {
            _agentQueryService = agentQueryService;
        }

        [HttpGet]
        [Authorize(Roles = nameof(Roles.Administrador) + "," + nameof(Roles.Desarrollador))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(
            Summary = "Listar agentes",
            Description = "Obtiene el listado completo de agentes registrados en el sistema."
        )]
        public async Task<IActionResult> List()
        {
            try
            {
                var agents = await _agentQueryService.GetAllAgentsAsync();

                if (!agents.Any())
                {
                    return NoContent();
                }

                return Ok(agents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = nameof(Roles.Administrador) + "," + nameof(Roles.Desarrollador))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(
            Summary = "Obtener agente por Id",
            Description = "Devuelve la información del agente correspondiente al Id especificado."
        )]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var agent = await _agentQueryService.GetAgentByIdAsync(id);

                if (agent == null)
                {
                    return NoContent();
                }

                return Ok(agent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }

        [HttpGet("{id}/properties")]
        [Authorize(Roles = nameof(Roles.Administrador) + "," + nameof(Roles.Desarrollador))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(
            Summary = "Obtener propiedades de un agente",
            Description = "Devuelve el listado de propiedades asignadas al agente con el Id especificado."
        )]
        public async Task<IActionResult> GetAgentProperties(string id)
        {
            try
            {
                var properties = await _agentQueryService.GetAgentPropertiesAsync(id);

                if (!properties.Any())
                {
                    return NoContent();
                }

                return Ok(properties);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = nameof(Roles.Administrador))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "Cambiar estado de un agente",
            Description = "Modifica el estado activo/inactivo del agente identificado por el Id especificado."
        )]
        public async Task<IActionResult> ChangeStatus(string id, [FromBody] bool isActive)
        {
            try
            {
                var result = await _agentQueryService.ChangeAgentStatusAsync(id, isActive);

                if (!result)
                {
                    return BadRequest(new { error = "No se pudo cambiar el estado del agente" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }
    }
}
