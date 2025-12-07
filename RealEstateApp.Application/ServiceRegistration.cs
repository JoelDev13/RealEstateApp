using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Application.Behaviors;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.Services;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Mappings;
using System.Reflection;
using RealEstateApp.Domain.Entities;

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

            //Services - Solo mapeos Entity <-> DTO (los mapeos DTO <-> ViewModel están en WebApp)
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            
            // Services (Los repositorios se registran en Infrastructure.Persistence)
            services.AddScoped<ISaleTypeService, SaleTypeService>();
            services.AddScoped<IPropertyTypeService, PropertyTypeService>();
            services.AddScoped<IImprovementService, ImprovementService>();
            services.AddScoped<IAdminUserService, AdminUserService>();
            services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            services.AddScoped<IPropertyService, PropertyService>();

            return services;
        }

        public static IServiceCollection AddApplicationLayerIoc(this IServiceCollection services, IConfiguration configuration)
        {
            // Registra el AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            
            return services;
        }
    }
}
