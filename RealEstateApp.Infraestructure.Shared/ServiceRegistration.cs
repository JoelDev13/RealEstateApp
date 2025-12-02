using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Settings;
using RealEstateApp.Infraestructure.Shared.Service;

namespace RealEstateApp.Infraestructure.Shared
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
