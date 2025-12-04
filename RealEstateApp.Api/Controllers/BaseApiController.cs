using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application;

namespace RealEstateApp.Api.Controllers
{
    [Route("api/v{version}/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult BadRequest400WithErrorMessagesFromResult(Result result)
        {
            return BadRequest(new
            {
                errors = result.Errors ?? new List<string> { result.Message ?? "Error en la solicitud" }
            });
        }

        protected IActionResult BadRequest400WithErrorMessagesFromResult<T>(Result<T> result)
        {
            return BadRequest(new
            {
                errors = result.Errors ?? new List<string> { result.Message ?? "Error en la solicitud" }
            });
        }
    }
}
