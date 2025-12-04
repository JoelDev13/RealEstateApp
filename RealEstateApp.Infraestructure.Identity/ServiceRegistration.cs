using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.Services;
using RealEstateApp.Domain.Settings;
using RealEstateApp.Infraestructure.Identity.Context;
using RealEstateApp.Infraestructure.Identity.Mappings;
using RealEstateApp.Infraestructure.Identity.Repositories;
using RealEstateApp.Infraestructure.Identity.Services;
using RealEstateApp.Infrastructure.Identity.Entities;
using RealEstateApp.Infrastructure.Identity.Repositories;
using RealEstateApp.Infrastructure.Identity.Services;

namespace RealEstateApp.Infrastructure.Identity
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            #region Database
            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("IdentityConnection"),
                    b => b.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)));
            #endregion

            #region Identity
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

            services.AddHttpContextAccessor();

            services.AddScoped<SignInManager<AppUser>>();

            services.AddAutoMapper(typeof(UserMappingProfile));
            services.AddScoped<IBaseAccountService, RealEstateApp.Infraestructure.Identity.Services.BaseAccountService>();
            services.AddScoped<IAccountServiceForWebApp, RealEstateApp.Infraestructure.Identity.Services.AccountServiceForWebApp>();
            services.AddScoped<IAdminUserRepository, AdminUserRepository>();
            services.AddScoped<IAccountApiService, AccountApiService>();
            services.AddScoped<IAgentAdminService, AgentAdminService>();
            services.AddScoped<IDeveloperUserService, DeveloperUserService>();
            services.AddScoped<IDeveloperUserRepository, DeveloperUserRepository>();

            #endregion

            #region Configurations
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            #endregion

            return services;
        }
    }
}
