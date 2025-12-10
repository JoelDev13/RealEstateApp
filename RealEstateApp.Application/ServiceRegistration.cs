using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Application.Behaviors;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.Services;
using RealEstateApp.Application.Interfaces.Repositories;
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

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            
            // Services (Los repositorios se registran en Infrastructure.Persistence)
            services.AddScoped<ISaleTypeService, SaleTypeService>();
            services.AddScoped<IPropertyTypeService, PropertyTypeService>();
            services.AddScoped<IImprovementService, ImprovementService>();
            services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<IDeveloperUserService, DeveloperUserService>();
            
            // New services for client and agent functionality
            services.AddScoped<IFavoriteService, FavoriteService>();
            services.AddScoped<IOfferService, OfferService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IAgentProfileService, AgentProfileService>();

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
