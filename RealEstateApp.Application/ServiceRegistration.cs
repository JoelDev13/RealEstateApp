using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Application.Behaviors;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.Services;
using System.Reflection;

namespace RealEstateApp.Application
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServicesIoC(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            //Behaviors and Validators
            services.AddValidatorsFromAssembly(typeof(ServiceRegistration).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
            });

            services.AddValidatorsFromAssembly(assembly);

            //Services
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<ISaleTypeService, SaleTypeService>();
            services.AddScoped<IPropertyTypeService, PropertyTypeService>();
            services.AddScoped<IImprovementService, ImprovementService>();
            services.AddScoped<IAdminUserService, AdminUserService>();

            return services;
        }
    }
}
