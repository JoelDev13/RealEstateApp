using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Infraestructure.Identity.Context;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Integration.Tests.Repositories
{
    public class AgentUserRepositoryTests : IDisposable
    {
        private ServiceProvider _serviceProvider;
        private IdentityContext _context;
        private UserManager<AppUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public AgentUserRepositoryTests()
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
            _roleManager.CreateAsync(new IdentityRole("Agente")).Wait();
            _roleManager.CreateAsync(new IdentityRole("Administrador")).Wait();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _serviceProvider.Dispose();
        }

        [Fact]
        public async Task GetUsersInRoleAsync_Agente_DebeRetornarSoloAgentes()
        {
            // Arrange
            var agentUser = new AppUser
            {
                UserName = "agent@test.com",
                Email = "agent@test.com",
                FirstName = "Agent",
                LastName = "Test",
                IsActive = true
            };
            
            var adminUser = new AppUser
            {
                UserName = "admin@test.com",
                Email = "admin@test.com",
                FirstName = "Admin",
                LastName = "Test",
                IsActive = true
            };

            await _userManager.CreateAsync(agentUser, "Password123!");
            await _userManager.CreateAsync(adminUser, "Password123!");
            await _userManager.AddToRoleAsync(agentUser, "Agente");
            await _userManager.AddToRoleAsync(adminUser, "Administrador");

            // Act
            var agents = await _userManager.GetUsersInRoleAsync("Agente");

            // Assert
            Assert.Single(agents);
            Assert.Equal("agent@test.com", agents[0].Email);
        }

        [Fact]
        public async Task CreateAgent_DebeCrearUsuarioConRolAgente()
        {
            // Arrange
            var agent = new AppUser
            {
                UserName = "newagent@test.com",
                Email = "newagent@test.com",
                FirstName = "New",
                LastName = "Agent",
                PhoneNumber = "8091234567",
                IsActive = true
            };

            // Act
            var result = await _userManager.CreateAsync(agent, "Password123!");
            await _userManager.AddToRoleAsync(agent, "Agente");

            // Assert
            Assert.True(result.Succeeded);
            
            var createdAgent = await _userManager.FindByEmailAsync("newagent@test.com");
            Assert.NotNull(createdAgent);
            
            var isInRole = await _userManager.IsInRoleAsync(createdAgent, "Agente");
            Assert.True(isInRole);
        }

        [Fact]
        public async Task UpdateAgentStatus_DebeCambiarEstadoActivo()
        {
            // Arrange
            var agent = new AppUser
            {
                UserName = "agent@test.com",
                Email = "agent@test.com",
                FirstName = "Agent",
                LastName = "Test",
                IsActive = true
            };
            
            await _userManager.CreateAsync(agent, "Password123!");
            await _userManager.AddToRoleAsync(agent, "Agente");

            // Act
            agent.IsActive = false;
            await _userManager.UpdateAsync(agent);

            // Assert
            var updatedAgent = await _userManager.FindByIdAsync(agent.Id);
            Assert.False(updatedAgent.IsActive);
        }

        [Fact]
        public async Task DeleteAgent_DebeEliminarUsuarioCorrectamente()
        {
            // Arrange
            var agent = new AppUser
            {
                UserName = "agent@test.com",
                Email = "agent@test.com",
                FirstName = "Agent",
                LastName = "Test",
                IsActive = true
            };
            
            await _userManager.CreateAsync(agent, "Password123!");
            await _userManager.AddToRoleAsync(agent, "Agente");

            // Act
            var deleteResult = await _userManager.DeleteAsync(agent);

            // Assert
            Assert.True(deleteResult.Succeeded);
            
            var deletedAgent = await _userManager.FindByIdAsync(agent.Id);
            Assert.Null(deletedAgent);
        }
    }
}
