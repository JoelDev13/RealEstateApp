using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace RealEstateApp.Application
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registra el AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            
            return services;
        }
    }
}
