using MediatR;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application;

namespace RealEstateApp.WebApi.Controllers
{

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        private IMediator? _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext!.RequestServices.GetService<IMediator>()!;
        protected IActionResult BadRequest400WithErrorMessagesFromResult(Result result)
        {
            var errors = result.Errors ?? new List<string>();
            if (!string.IsNullOrEmpty(result.Message))
            {
                errors.Add(result.Message);
            }
            return BadRequest(new { errors });
        }

        protected IActionResult BadRequest400WithErrorMessagesFromResult<T>(Result<T> result)
        {
            var errors = result.Errors ?? new List<string>();
            if (!string.IsNullOrEmpty(result.Message))
            {
                errors.Add(result.Message);
            }
            return BadRequest(new { errors });
        }
    }
}
