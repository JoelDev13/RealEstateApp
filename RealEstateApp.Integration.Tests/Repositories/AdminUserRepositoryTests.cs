using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Infraestructure.Identity.Context;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Integration.Tests.Repositories
{
    public class AdminUserRepositoryTests : IDisposable
    {
        private ServiceProvider _serviceProvider;
        private IdentityContext _context;
        private UserManager<AppUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public AdminUserRepositoryTests()
        {
            Setup();
        }

        private void Setup()
        {
            var services = new ServiceCollection();
            
            services.AddDbContext<IdentityContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));
            
            services.AddLogging();
            
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<IdentityContext>();

            _serviceProvider = services.BuildServiceProvider();
            _context = _serviceProvider.GetRequiredService<IdentityContext>();
            _userManager = _serviceProvider.GetRequiredService<UserManager<AppUser>>();
            _roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Crea los roles
            _roleManager.CreateAsync(new IdentityRole("Administrador")).Wait();
            _roleManager.CreateAsync(new IdentityRole("Agente")).Wait();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _serviceProvider.Dispose();
        }

        [Fact]
        public async Task GetUsersInRoleAsync_Administrador_DebeRetornarSoloAdministradores()
        {
            // Arrange
            var adminUser = new AppUser
            {
                UserName = "admin@test.com",
                Email = "admin@test.com",
                FirstName = "Admin",
                LastName = "Test",
                IsActive = true
            };
            
            var agentUser = new AppUser
            {
                UserName = "agent@test.com",
                Email = "agent@test.com",
                FirstName = "Agent",
                LastName = "Test",
                IsActive = true
            };

            await _userManager.CreateAsync(adminUser, "Password123!");
            await _userManager.CreateAsync(agentUser, "Password123!");
            await _userManager.AddToRoleAsync(adminUser, "Administrador");
            await _userManager.AddToRoleAsync(agentUser, "Agente");

            // Act
            var admins = await _userManager.GetUsersInRoleAsync("Administrador");

            // Assert
            Assert.Single(admins);
            Assert.Equal("admin@test.com", admins[0].Email);
        }

        [Fact]
        public async Task CreateAdmin_DebeCrearUsuarioConRolAdministrador()
        {
            // Arrange
            var admin = new AppUser
            {
                UserName = "newadmin@test.com",
                Email = "newadmin@test.com",
                FirstName = "New",
                LastName = "Admin",
                PhoneNumber = "8091234567",
                IsActive = true
            };

            // Act
            var result = await _userManager.CreateAsync(admin, "Password123!");
            await _userManager.AddToRoleAsync(admin, "Administrador");

            // Assert
            Assert.True(result.Succeeded);
            
            var createdAdmin = await _userManager.FindByEmailAsync("newadmin@test.com");
            Assert.NotNull(createdAdmin);
            
            var isInRole = await _userManager.IsInRoleAsync(createdAdmin, "Administrador");
            Assert.True(isInRole);
        }

        [Fact]
        public async Task UpdateAdminStatus_DebeCambiarEstadoActivo()
        {
            // Arrange
            var admin = new AppUser
            {
                UserName = "admin@test.com",
                Email = "admin@test.com",
                FirstName = "Admin",
                LastName = "Test",
                IsActive = true
            };
            
            await _userManager.CreateAsync(admin, "Password123!");
            await _userManager.AddToRoleAsync(admin, "Administrador");

            // Act
            admin.IsActive = false;
            await _userManager.UpdateAsync(admin);

            // Assert
            var updatedAdmin = await _userManager.FindByIdAsync(admin.Id);
            Assert.False(updatedAdmin.IsActive);
        }

        [Fact]
        public async Task DeleteAdmin_DebeEliminarUsuarioCorrectamente()
        {
            // Arrange
            var admin = new AppUser
            {
                UserName = "admin@test.com",
                Email = "admin@test.com",
                FirstName = "Admin",
                LastName = "Test",
                IsActive = true
            };
            
            await _userManager.CreateAsync(admin, "Password123!");
            await _userManager.AddToRoleAsync(admin, "Administrador");

            // Act
            var deleteResult = await _userManager.DeleteAsync(admin);

            // Assert
            Assert.True(deleteResult.Succeeded);
            
            var deletedAdmin = await _userManager.FindByIdAsync(admin.Id);
            Assert.Null(deletedAdmin);
        }
    }
}
