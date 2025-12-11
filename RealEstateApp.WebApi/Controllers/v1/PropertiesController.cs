using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Features.Properties.Queries;
using RealEstateApp.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateApp.WebApi.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class PropertiesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PropertiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = nameof(Roles.Administrador) + "," + nameof(Roles.Desarrollador))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(
            Summary = "Listar propiedades",
            Description = "Obtiene el listado completo de propiedades registradas."
        )]
        public async Task<IActionResult> List()
        {
            try
            {
                var properties = await _mediator.Send(new GetAllPropertiesQuery());

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

        [HttpGet("{id}")]
        [Authorize(Roles = nameof(Roles.Administrador) + "," + nameof(Roles.Desarrollador))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(
            Summary = "Obtener propiedad por Id",
            Description = "Devuelve la información de la propiedad correspondiente al Id especificado."
        )]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var property = await _mediator.Send(new GetPropertyByIdQuery { Id = id });

                if (property == null)
                {
                    return NoContent();
                }

                return Ok(property);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }

        [HttpGet("code/{code}")]
        [Authorize(Roles = nameof(Roles.Administrador) + "," + nameof(Roles.Desarrollador))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(
            Summary = "Obtener propiedad por código",
            Description = "Devuelve la información de la propiedad correspondiente al código especificado."
        )]
        public async Task<IActionResult> GetByCode(string code)
        {
            try
            {
                var property = await _mediator.Send(new GetPropertyByCodeQuery { Code = code });

                if (property == null)
                {
                    return NoContent();
                }

                return Ok(property);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }
    }
}
