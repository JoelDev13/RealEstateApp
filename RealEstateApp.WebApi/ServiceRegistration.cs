using Microsoft.OpenApi.Models;
using RealEstateApp.Api.Handlers;

namespace RealEstateApp.Api
{
    public static class ServiceRegistration
    {
        public static void AddApiServices(this IServiceCollection services)
        {
            services.AddControllers()
                    .ConfigureApiBehaviorOptions(options =>
                    {
                        options.SuppressModelStateInvalidFilter = true;
                    });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "RealEstateApp API",
                    Version = "v1",
                    Description = "REST API for RealEstateApp with CQRS, JWT and ProblemDetails"
                });
            });
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                        builder.AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowAnyMethod());
            });
        }
    }
}
