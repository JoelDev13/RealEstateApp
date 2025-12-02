using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Exceptions;
using System.Net;

namespace RealEstateApp.Api.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            string exceptionTitle = "An unexpected error occurred";
            string details = exception.Message;
            int statusCode;

            switch (exception)
            {
                case ApiException apiException:
                    statusCode = apiException.StatusCode;
                    switch (statusCode)
                    {
                        case (int)HttpStatusCode.BadRequest:
                            exceptionTitle = "Bad Request";
                            break;
                        case (int)HttpStatusCode.NotFound:
                            exceptionTitle = "Not Found";
                            break;
                        default:
                            exceptionTitle = "Server Error";
                            break;
                    }
                    break;

                case KeyNotFoundException:
                    exceptionTitle = "Not Found";
                    statusCode = (int)HttpStatusCode.NotFound;
                    break;

                case ArgumentException:
                    exceptionTitle = "Bad Request";
                    statusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case RealEstateApp.Application.Exceptions.ValidationException validationException:
                    exceptionTitle = "Bad Request";
                    statusCode = (int)HttpStatusCode.BadRequest;
                    details = validationException.Errors.Aggregate((a, b) => a + "; " + b);
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Title = exceptionTitle,
                Status = statusCode,
                Detail = details,
                Instance = httpContext.Request.Path
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

            return true;
        }
    }
}
