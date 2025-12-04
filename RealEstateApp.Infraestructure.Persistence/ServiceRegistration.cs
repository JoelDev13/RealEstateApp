using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RealEstateApp.Infraestructure.Persistence
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddPersistenceLayerIoc(this IServiceCollection services, IConfiguration configuration)
        {
            
            return services;
        }
    }
}

