using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace RealEstateApp.Application
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationLayerIoc(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
            });

            services.AddValidatorsFromAssembly(assembly);

            services.AddAutoMapper(assembly);

            return services;
        }
    }
}
