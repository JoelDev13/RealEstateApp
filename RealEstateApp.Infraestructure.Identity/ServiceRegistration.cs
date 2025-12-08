using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RealEstateApp.Domain.Settings;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Infraestructure.Identity.Context;
using RealEstateApp.Infraestructure.Identity.Mappings;
using RealEstateApp.Infraestructure.Identity.Repositories;
using RealEstateApp.Infraestructure.Identity.Services;
using RealEstateApp.Infrastructure.Identity.Entities;
using RealEstateApp.Infrastructure.Identity.Repositories;
using RealEstateApp.Infrastructure.Identity.Services;
using RealEstateApp.Application.Services;

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
            services.AddScoped<IAgentService, AgentService>();
            services.AddScoped<IDeveloperUserService, DeveloperUserService>();
            services.AddScoped<IDeveloperUserRepository, DeveloperUserRepository>();

            #endregion

            #region Configurations
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            #endregion

            return services;
        }

        public static IServiceCollection AddIdentityLayerIocForWebApi(this IServiceCollection services, IConfiguration configuration)
        {
            #region Database
            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("IdentityConnection"),
                    b => b.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)));
            #endregion

            #region Identity
            services.AddIdentityCore<AppUser>(options =>
            {
                // Configuracion de contraseñas
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                // Configuracion de bloqueo de usuario
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Configuracion de usuario
                options.User.RequireUniqueEmail = true;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            services.AddHttpContextAccessor();
            services.AddScoped<SignInManager<AppUser>>();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();
            services.AddAutoMapper(typeof(UserMappingProfile));
            #endregion

            #region JWT Settings
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtSettings?.Issuer,
                    ValidAudience = jwtSettings?.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.SecretKey ?? ""))
                };
            });
            #endregion

            #region Services
            services.AddScoped<Application.Interfaces.Services.IBaseAccountService, RealEstateApp.Infraestructure.Identity.Services.BaseAccountService>();
            services.AddScoped<Application.Interfaces.IAccountServiceForWebApi, RealEstateApp.Infraestructure.Identity.Services.AccountServiceForWebApi>();
            services.AddScoped<IAdminUserService, AdminUserService>();
            services.AddScoped<IAdminUserRepository, AdminUserRepository>();
            services.AddScoped<IDeveloperUserRepository, DeveloperUserRepository>();
            #endregion

            #region Exception Handler
            services.AddProblemDetails();
            #endregion

            return services;
        }
    }
}
