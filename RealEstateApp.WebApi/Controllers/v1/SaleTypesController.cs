using Asp.Versioning;
using RealEstateApp.WebApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Dtos.SaleTypes;
using RealEstateApp.Application.Features.SaleTypes.Commands.CreateSaleType;
using RealEstateApp.Application.Features.SaleTypes.Commands.DeleteSaleType;
using RealEstateApp.Application.Features.SaleTypes.Commands.ToggleSaleTypeStatus;
using RealEstateApp.Application.Features.SaleTypes.Queries.GetSaleTypeById;
using RealEstateApp.Application.Features.SaleTypes.Queries.GetSaleTypes;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateApp.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class SaleTypesController : BaseApiController
    {
        [HttpGet]
        [Authorize(Roles = "Administrador,Desarrollador")]
        [ProducesResponseType(typeof(List<SaleTypeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(
            Summary = "Obtener lista de tipos de venta",
            Description = "Retorna una lista con todos los tipos de venta disponibles."
        )]
        public async Task<ActionResult<List<SaleTypeDto>>> GetAll()
        {
            var result = await Mediator.Send(new GetSaleTypesQuery());
            if (result == null || result.Count == 0)
                return NoContent();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Administrador,Desarrollador")]
        [ProducesResponseType(typeof(SaleTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Obtener tipo de venta por Id",
            Description = "Retorna la información del tipo de venta correspondiente al Id especificado."
        )]
        public async Task<ActionResult<SaleTypeDto>> GetById(int id)
        {
            var result = await Mediator.Send(new GetSaleTypeByIdQuery(id));
            return Ok(result);
        }


        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "Crear tipo de venta",
            Description = "Crea un nuevo tipo de venta. Solo permitido para administradores."
        )]
        public async Task<ActionResult<int>> Create([FromBody] CreateSaleTypeCommand command)
        {
            var newId = await Mediator.Send(command);

            return CreatedAtAction(
                nameof(GetById),
                new { id = newId, version = "1.0" },
                newId
            );
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(SaleTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Actualizar tipo de venta",
            Description = "Modifica los datos de un tipo de venta existente."
        )]
        public async Task<ActionResult<SaleTypeDto>> Update(
            int id,
            [FromBody] UpdateSaleTypeCommand command)
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
            Summary = "Activar/Inactivar tipo de venta",
            Description = "Cambia el estado activo del tipo de venta indicado."
        )]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var command = new ToggleSaleTypeStatusCommand { Id = id };
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Eliminar tipo de venta",
            Description = "Elimina el tipo de venta correspondiente al Id especificado."
        )]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteSaleTypeCommand { Id = id };
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
