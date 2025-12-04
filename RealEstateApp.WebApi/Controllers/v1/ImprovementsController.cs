using Asp.Versioning;
using InvestmentApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.Improvements;
using RealEstateApp.Application.Features.Improvements.Commands.CreateImprovement;
using RealEstateApp.Application.Features.Improvements.Commands.DeleteImprovement;
using RealEstateApp.Application.Features.Improvements.Commands.ToggleImprovementStatus;
using RealEstateApp.Application.Features.Improvements.Queries.GetImprovementById;
using RealEstateApp.Application.Features.Improvements.Queries.GetImprovements;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateApp.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class ImprovementsController : BaseApiController
    {

        [HttpGet]
        [Authorize(Roles = "Administrador,Desarrollador")]
        [ProducesResponseType(typeof(List<ImprovementDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(
            Summary = "Obtener lista de mejoras",
            Description = "Retorna una lista con todas las mejoras (amenities) disponibles."
        )]
        public async Task<ActionResult<List<ImprovementDto>>> GetAll()
        {
            var result = await Mediator.Send(new GetImprovementsQuery());
            if (result == null || result.Count == 0)
                return NoContent();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Administrador,Desarrollador")]
        [ProducesResponseType(typeof(ImprovementDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Obtener mejora por Id",
            Description = "Retorna la información de la mejora correspondiente al Id especificado."
        )]
        public async Task<ActionResult<ImprovementDto>> GetById(int id)
        {
            var result = await Mediator.Send(new GetImprovementByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "Crear mejora",
            Description = "Crea una nueva mejora (amenity). Solo permitido para administradores."
        )]
        public async Task<ActionResult<int>> Create([FromBody] CreateImprovementCommand command)
        {
            var newId = await Mediator.Send(command);

            return CreatedAtAction(
                nameof(GetById),
                new { id = newId, version = "1.0" },
                newId
            );
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ImprovementDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Actualizar mejora",
            Description = "Modifica los datos de una mejora existente."
        )]
        public async Task<ActionResult<ImprovementDto>> Update(
            int id,
            [FromBody] UpdateImprovementCommand command)
        {
            if (id != command.Id)
                return BadRequest("El Id de la URL no coincide con el Id del cuerpo de la petición.");

            var updated = await Mediator.Send(command);
            return Ok(updated);
        }

        [HttpPatch("{id:int}/toggle-status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Activar/Inactivar mejora",
            Description = "Cambia el estado activo de la mejora indicada."
        )]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var command = new ToggleImprovementStatusCommand { Id = id };
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Eliminar mejora",
            Description = "Elimina la mejora correspondiente al Id especificado."
        )]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteImprovementCommand { Id = id };
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
