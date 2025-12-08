using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.Dtos.Agents;
using RealEstateApp.Domain.Enums;

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
