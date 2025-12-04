using Microsoft.AspNetCore.Diagnostics;
using RealEstateApp.Application.Exceptions;

namespace RealEstateApp.WebApi.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            string exceptionTitle = "An unexpected error occurred";
            string details = exception.Message;

            switch (exception)
            {
                case ApiException apiException:
                    switch (apiException.StatusCode)
                    {
                        case 400:
                            exceptionTitle = "Bad Request";
                            httpContext.Response.StatusCode = 400;
                            break;
                        case 404:
                            exceptionTitle = "Not found";
                            httpContext.Response.StatusCode = 404;
                            break;
                        default:
                            httpContext.Response.StatusCode = 500;
                            break;
                    }
                    break;
                case KeyNotFoundException:
                    exceptionTitle = "Not found";
                    httpContext.Response.StatusCode = 404;
                    break;
                case ArgumentException:
                    exceptionTitle = "Bad Request";
                    httpContext.Response.StatusCode = 400;
                    break;
                case ValidationException valEx:
                    exceptionTitle = "Bad Request";
                    details = string.Join(", ", valEx.Errors);
                    httpContext.Response.StatusCode = 400;
                    break;
                default:
                    httpContext.Response.StatusCode = 500;
                    break;
            }

            var problemDetails = new
            {
                Title = exceptionTitle,
                Status = httpContext.Response.StatusCode,
                Detail = details,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

            return true;
        }
    }
}
