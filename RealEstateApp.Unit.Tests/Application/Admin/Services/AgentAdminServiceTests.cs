using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Infraestructure.Identity.Services;
using RealEstateApp.Infrastructure.Identity.Entities;
using RealEstateApp.Infrastructure.Persistence;

namespace RealEstateApp.Unit.Tests.Services.Admin
{
    public class AgentAdminServiceTests
    {
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly ApplicationDbContext _dbContext;
        private readonly AgentAdminService _service;

        public AgentAdminServiceTests()
        {
            // DbContext InMemory
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"AgentAdminServiceTests_{Guid.NewGuid()}")
                .Options;

            _dbContext = new ApplicationDbContext(options);

            // UserManager mock
            var userStoreMock = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(
                userStoreMock.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            _service = new AgentAdminService(_userManagerMock.Object, _dbContext);
        }

        #region GetAgentsAsync

        [Fact]
        public async Task GetAgentsAsync_ReturnsOnlyAgents_WithCorrectPropertyCounts()
        {
            // Arrange
            var agent1 = new AppUser
            {
                Id = "agent1",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "111-111",
                UserType = nameof(Roles.Agente),
                IsActive = true
            };

            var agent2 = new AppUser
            {
                Id = "agent2",
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                PhoneNumber = "222-222",
                UserType = nameof(Roles.Agente),
                IsActive = false
            };

            var client = new AppUser
            {
                Id = "client1",
                FirstName = "Client",
                LastName = "User",
                Email = "client@example.com",
                PhoneNumber = "333-333",
                UserType = nameof(Roles.Cliente),
                IsActive = true
            };

            var allUsers = new List<AppUser> { agent1, agent2, client };

            _userManagerMock
                .SetupGet(um => um.Users)
                .Returns(allUsers.AsQueryable());

            _dbContext.Properties.AddRange(
                new Property { Id = 1, AgentId = "agent1", Code = "P1", Price = 100, Description = "Prop 1" },
                new Property { Id = 2, AgentId = "agent1", Code = "P2", Price = 200, Description = "Prop 2" },
                new Property { Id = 3, AgentId = "agent1", Code = "P3", Price = 300, Description = "Prop 3" },
                new Property { Id = 4, AgentId = "agent2", Code = "P4", Price = 400, Description = "Prop 4" },
                new Property { Id = 5, AgentId = "other", Code = "P5", Price = 500, Description = "Prop 5" }
            );
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetAgentsAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            // Solo los 2 agentes, no el cliente
            Assert.Equal(2, result.Data.Count);

            var agent1Dto = result.Data.Single(a => a.Id == "agent1");
            var agent2Dto = result.Data.Single(a => a.Id == "agent2");

            Assert.Equal("John", agent1Dto.FirstName);
            Assert.Equal("Doe", agent1Dto.LastName);
            Assert.Equal("john@example.com", agent1Dto.Email);
            Assert.Equal("111-111", agent1Dto.PhoneNumber);
            Assert.Equal(3, agent1Dto.PropertyCount);
            Assert.True(agent1Dto.IsActive);

            Assert.Equal("Jane", agent2Dto.FirstName);
            Assert.Equal("Smith", agent2Dto.LastName);
            Assert.Equal("jane@example.com", agent2Dto.Email);
            Assert.Equal("222-222", agent2Dto.PhoneNumber);
            Assert.Equal(1, agent2Dto.PropertyCount);
            Assert.False(agent2Dto.IsActive);
        }

        #endregion

        #region ToggleStatusAsync

        [Fact]
        public async Task ToggleStatusAsync_ValidAgent_TogglesStatusAndReturnsSuccess()
        {
            // Arrange
            var agent = new AppUser
            {
                Id = "agent1",
                UserType = nameof(Roles.Agente),
                IsActive = true
            };

            _userManagerMock
                .Setup(um => um.FindByIdAsync("agent1"))
                .ReturnsAsync(agent);

            _userManagerMock
                .Setup(um => um.UpdateAsync(agent))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _service.ToggleStatusAsync("agent1");

            // Assert
            Assert.True(result.Succeeded);
            Assert.False(agent.IsActive);

            _userManagerMock.Verify(um => um.UpdateAsync(agent), Times.Once);
        }

        [Fact]
        public async Task ToggleStatusAsync_UserNotFoundOrNotAgent_ReturnsFail()
        {
            // Caso 1: usuario null
            _userManagerMock
                .Setup(um => um.FindByIdAsync("missing"))
                .ReturnsAsync((AppUser?)null);

            var result1 = await _service.ToggleStatusAsync("missing");
            Assert.False(result1.Succeeded);

            // Caso 2: usuario no es agente
            var client = new AppUser
            {
                Id = "client1",
                UserType = nameof(Roles.Cliente),
                IsActive = true
            };

            _userManagerMock
                .Setup(um => um.FindByIdAsync("client1"))
                .ReturnsAsync(client);

            var result2 = await _service.ToggleStatusAsync("client1");
            Assert.False(result2.Succeeded);
        }

        [Fact]
        public async Task ToggleStatusAsync_UpdateFails_ReturnsFail()
        {
            // Arrange
            var agent = new AppUser
            {
                Id = "agent1",
                UserType = nameof(Roles.Agente),
                IsActive = true
            };

            _userManagerMock
                .Setup(um => um.FindByIdAsync("agent1"))
                .ReturnsAsync(agent);

            var identityError = new IdentityError { Description = "Update error" };

            _userManagerMock
                .Setup(um => um.UpdateAsync(agent))
                .ReturnsAsync(IdentityResult.Failed(identityError));

            // Act
            var result = await _service.ToggleStatusAsync("agent1");

            // Assert
            Assert.False(result.Succeeded);
            _userManagerMock.Verify(um => um.UpdateAsync(agent), Times.Once);
        }

        #endregion

        #region DeleteAgentWithPropertiesAsync

        [Fact]
        public async Task DeleteAgentWithPropertiesAsync_ValidAgent_DeletesPropertiesAndAgent()
        {
            // Arrange
            var agent = new AppUser
            {
                Id = "agent1",
                UserType = nameof(Roles.Agente),
                IsActive = true
            };

            _userManagerMock
                .Setup(um => um.FindByIdAsync("agent1"))
                .ReturnsAsync(agent);

            _userManagerMock
                .Setup(um => um.DeleteAsync(agent))
                .ReturnsAsync(IdentityResult.Success);

            _dbContext.Properties.AddRange(
                new Property { Id = 1, AgentId = "agent1", Code = "P1", Price = 100, Description = "Prop 1" },
                new Property { Id = 2, AgentId = "agent1", Code = "P2", Price = 200, Description = "Prop 2" },
                new Property { Id = 3, AgentId = "other", Code = "P3", Price = 300, Description = "Prop 3" }
            );
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.DeleteAgentWithPropertiesAsync("agent1");

            // Assert
            Assert.True(result.Succeeded);

            var agentProps = await _dbContext.Properties
                .Where(p => p.AgentId == "agent1")
                .ToListAsync();

            var otherProps = await _dbContext.Properties
                .Where(p => p.AgentId == "other")
                .ToListAsync();

            Assert.Empty(agentProps);           // todas las del agente se eliminaron
            Assert.Single(otherProps);          // las de otros agentes siguen

            _userManagerMock.Verify(um => um.DeleteAsync(agent), Times.Once);
        }

        [Fact]
        public async Task DeleteAgentWithPropertiesAsync_UserNotFoundOrNotAgent_ReturnsFail()
        {
            // Caso 1: usuario null
            _userManagerMock
                .Setup(um => um.FindByIdAsync("missing"))
                .ReturnsAsync((AppUser?)null);

            var result1 = await _service.DeleteAgentWithPropertiesAsync("missing");
            Assert.False(result1.Succeeded);

            // Caso 2: usuario no es agente
            var client = new AppUser
            {
                Id = "client1",
                UserType = nameof(Roles.Cliente),
                IsActive = true
            };

            _userManagerMock
                .Setup(um => um.FindByIdAsync("client1"))
                .ReturnsAsync(client);

            var result2 = await _service.DeleteAgentWithPropertiesAsync("client1");
            Assert.False(result2.Succeeded);
        }

        [Fact]
        public async Task DeleteAgentWithPropertiesAsync_DeleteFails_ReturnsFail_AndPropertiesAreStillRemoved()
        {
            // Arrange
            var agent = new AppUser
            {
                Id = "agent1",
                UserType = nameof(Roles.Agente),
                IsActive = true
            };

            _userManagerMock
                .Setup(um => um.FindByIdAsync("agent1"))
                .ReturnsAsync(agent);

            var identityError = new IdentityError { Description = "Delete error" };

            _userManagerMock
                .Setup(um => um.DeleteAsync(agent))
                .ReturnsAsync(IdentityResult.Failed(identityError));

            _dbContext.Properties.AddRange(
                new Property { Id = 1, AgentId = "agent1", Code = "P1", Price = 100, Description = "Prop 1" },
                new Property { Id = 2, AgentId = "agent1", Code = "P2", Price = 200, Description = "Prop 2" },
                new Property { Id = 3, AgentId = "other", Code = "P3", Price = 300, Description = "Prop 3" }
            );
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.DeleteAgentWithPropertiesAsync("agent1");

            // Assert
            Assert.False(result.Succeeded);

            // Aunque falle la eliminación del usuario, el servicio ya borro las propiedades del agente
            var agentProps = await _dbContext.Properties
                .Where(p => p.AgentId == "agent1")
                .ToListAsync();
            var otherProps = await _dbContext.Properties
                .Where(p => p.AgentId == "other")
                .ToListAsync();

            Assert.Empty(agentProps);
            Assert.Single(otherProps);

            _userManagerMock.Verify(um => um.DeleteAsync(agent), Times.Once);
        }

        #endregion
    }
}
