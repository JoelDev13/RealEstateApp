using Asp.Versioning;
using InvestmentApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.PropertyTypes;
using RealEstateApp.Application.Features.PropertyTypes.Commands.CreatePropertyType;
using RealEstateApp.Application.Features.PropertyTypes.Commands.DeletePropertyType;
using RealEstateApp.Application.Features.PropertyTypes.Commands.TogglePropertyTypeStatus;
using RealEstateApp.Application.Features.PropertyTypes.Queries.GetPropertyTypeById;
using RealEstateApp.Application.Features.PropertyTypes.Queries.GetPropertyTypes;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateApp.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class PropertyTypesController : BaseApiController
    {
        [HttpGet]
        [Authorize(Roles = "Administrador,Desarrollador")]
        [ProducesResponseType(typeof(List<PropertyTypeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Listar tipos de propiedad",
            Description = "Obtiene la lista de todos los tipos de propiedad registrados en el sistema."
        )]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new GetPropertyTypesQuery());

            if (result == null || result.Count == 0)
                return NoContent();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Administrador,Desarrollador")]
        [ProducesResponseType(typeof(PropertyTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Obtener tipo de propiedad por Id",
            Description = "Devuelve la información de un tipo de propiedad específico."
        )]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetPropertyTypeByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Crear tipo de propiedad",
            Description = "Crea un nuevo tipo de propiedad. Solo disponible para administradores."
        )]
        public async Task<IActionResult> Create([FromBody] CreatePropertyTypeCommand command)
        {
            var newId = await Mediator.Send(command);

            if (newId == 0)
                return StatusCode(StatusCodes.Status500InternalServerError, "No se pudo crear el tipo de propiedad.");

            return CreatedAtAction(
                nameof(GetById),
                new { id = newId, version = "1.0" },
                newId
            );
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(PropertyTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Actualizar tipo de propiedad",
            Description = "Actualiza los datos de un tipo de propiedad existente. Solo disponible para administradores."
        )]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdatePropertyTypeCommand command)
        {
            if (id != command.Id)
                return BadRequest("El Id de la URL no coincide con el Id del cuerpo de la petición.");

            var updated = await Mediator.Send(command);
            return Ok(updated);
        }

        [HttpPatch("{id:int}/toggle-status")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Cambiar estado de tipo de propiedad",
            Description = "Activa o inactiva un tipo de propiedad existente. Solo disponible para administradores."
        )]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var command = new TogglePropertyTypeStatusCommand { Id = id };
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Eliminar tipo de propiedad",
            Description = "Elimina un tipo de propiedad por Id. Solo disponible para administradores."
        )]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeletePropertyTypeCommand { Id = id };
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
