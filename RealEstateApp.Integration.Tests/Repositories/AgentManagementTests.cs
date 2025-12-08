using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Infraestructure.Identity.Context;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Integration.Tests.Repositories
{
    public class AgentManagementTests : IDisposable
    {
        private ServiceProvider _serviceProvider;
        private IdentityContext _context;
        private UserManager<AppUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public AgentManagementTests()
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
        public async Task GetActiveAgents_DebeRetornarSoloAgentesActivos()
        {
            // Arrange
            var activeAgent = new AppUser
            {
                UserName = "active@test.com",
                Email = "active@test.com",
                FirstName = "Active",
                LastName = "Agent",
                IsActive = true
            };
            
            var inactiveAgent = new AppUser
            {
                UserName = "inactive@test.com",
                Email = "inactive@test.com",
                FirstName = "Inactive",
                LastName = "Agent",
                IsActive = false
            };

            await _userManager.CreateAsync(activeAgent, "Password123!");
            await _userManager.CreateAsync(inactiveAgent, "Password123!");
            await _userManager.AddToRoleAsync(activeAgent, "Agente");
            await _userManager.AddToRoleAsync(inactiveAgent, "Agente");

            // Act
            var allAgents = await _userManager.GetUsersInRoleAsync("Agente");
            var activeAgents = allAgents.Where(a => a.IsActive).ToList();

            // Assert
            Assert.Equal(2, allAgents.Count);
            Assert.Single(activeAgents);
            Assert.Equal("active@test.com", activeAgents[0].Email);
        }

        [Fact]
        public async Task GetAgentByEmail_DebeRetornarAgenteExistente()
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
            var foundAgent = await _userManager.FindByEmailAsync("agent@test.com");

            // Assert
            Assert.NotNull(foundAgent);
            Assert.Equal("Agent", foundAgent.FirstName);
            Assert.Equal("Test", foundAgent.LastName);
            
            var isAgent = await _userManager.IsInRoleAsync(foundAgent, "Agente");
            Assert.True(isAgent);
        }

        [Fact]
        public async Task UpdateAgentPhone_DebeActualizarNumeroTelefono()
        {
            // Arrange
            var agent = new AppUser
            {
                UserName = "agent@test.com",
                Email = "agent@test.com",
                FirstName = "Agent",
                LastName = "Test",
                PhoneNumber = "8091234567",
                IsActive = true
            };
            
            await _userManager.CreateAsync(agent, "Password123!");
            await _userManager.AddToRoleAsync(agent, "Agente");

            // Act
            agent.PhoneNumber = "8099876543";
            await _userManager.UpdateAsync(agent);

            // Assert
            var updatedAgent = await _userManager.FindByIdAsync(agent.Id);
            Assert.Equal("8099876543", updatedAgent.PhoneNumber);
        }

        [Fact]
        public async Task CreateMultipleAgents_DebeCrearVariosAgentes()
        {
            // Arrange
            var agents = new List<AppUser>
            {
                new AppUser
                {
                    UserName = "agent1@test.com",
                    Email = "agent1@test.com",
                    FirstName = "Agent",
                    LastName = "One",
                    IsActive = true
                },
                new AppUser
                {
                    UserName = "agent2@test.com",
                    Email = "agent2@test.com",
                    FirstName = "Agent",
                    LastName = "Two",
                    IsActive = true
                },
                new AppUser
                {
                    UserName = "agent3@test.com",
                    Email = "agent3@test.com",
                    FirstName = "Agent",
                    LastName = "Three",
                    IsActive = true
                }
            };

            // Act
            foreach (var agent in agents)
            {
                await _userManager.CreateAsync(agent, "Password123!");
                await _userManager.AddToRoleAsync(agent, "Agente");
            }

            // Assert
            var allAgents = await _userManager.GetUsersInRoleAsync("Agente");
            Assert.Equal(3, allAgents.Count);
            
            foreach (var agent in allAgents)
            {
                var isInRole = await _userManager.IsInRoleAsync(agent, "Agente");
                Assert.True(isInRole);
            }
        }

        [Fact]
        public async Task GetAgentCount_DebeRetornarCantidadCorrecta()
        {
            // Arrange
            var agents = new List<AppUser>
            {
                new AppUser { UserName = "agent1@test.com", Email = "agent1@test.com", FirstName = "Agent", LastName = "One", IsActive = true },
                new AppUser { UserName = "agent2@test.com", Email = "agent2@test.com", FirstName = "Agent", LastName = "Two", IsActive = true },
                new AppUser { UserName = "admin@test.com", Email = "admin@test.com", FirstName = "Admin", LastName = "Test", IsActive = true }
            };

            // Act
            foreach (var agent in agents.Take(2))
            {
                await _userManager.CreateAsync(agent, "Password123!");
                await _userManager.AddToRoleAsync(agent, "Agente");
            }
            
            await _userManager.CreateAsync(agents.Last(), "Password123!");
            await _userManager.AddToRoleAsync(agents.Last(), "Administrador");

            // Assert
            var agentCount = (await _userManager.GetUsersInRoleAsync("Agente")).Count;
            Assert.Equal(2, agentCount);
        }

        [Fact]
        public async Task GetInactiveAgents_DebeRetornarSoloAgentesInactivos()
        {
            // Arrange
            var activeAgent = new AppUser
            {
                UserName = "active@test.com",
                Email = "active@test.com",
                FirstName = "Active",
                LastName = "Agent",
                IsActive = true
            };
            
            var inactiveAgent = new AppUser
            {
                UserName = "inactive@test.com",
                Email = "inactive@test.com",
                FirstName = "Inactive",
                LastName = "Agent",
                IsActive = false
            };

            await _userManager.CreateAsync(activeAgent, "Password123!");
            await _userManager.CreateAsync(inactiveAgent, "Password123!");
            await _userManager.AddToRoleAsync(activeAgent, "Agente");
            await _userManager.AddToRoleAsync(inactiveAgent, "Agente");

            // Act
            var allAgents = await _userManager.GetUsersInRoleAsync("Agente");
            var inactiveAgents = allAgents.Where(a => !a.IsActive).ToList();

            // Assert
            Assert.Equal(2, allAgents.Count);
            Assert.Single(inactiveAgents);
            Assert.Equal("inactive@test.com", inactiveAgents[0].Email);
        }
    }
}
