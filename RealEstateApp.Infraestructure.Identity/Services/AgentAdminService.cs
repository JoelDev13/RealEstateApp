using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application;
using RealEstateApp.Application.Dtos.Agents;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;
using RealEstateApp.Infrastructure.Persistence;

namespace RealEstateApp.Infraestructure.Identity.Services
{
    public class AgentAdminService : IAgentAdminService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public AgentAdminService(
            UserManager<AppUser> userManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<Result<List<AgentAdminDto>>> GetAgentsAsync()
        {
            var agents = await _userManager.Users
                .Where(u => u.UserType == nameof(Roles.Agente))
                .ToListAsync();

            var agentIds = agents.Select(a => a.Id).ToList();

            var propertyCounts = await _dbContext.Properties
                .Where(p => agentIds.Contains(p.AgentId))
                .GroupBy(p => p.AgentId)
                .Select(g => new { AgentId = g.Key, Count = g.Count() })
                .ToListAsync();

            var countsDict = propertyCounts.ToDictionary(x => x.AgentId, x => x.Count);

            var list = agents.Select(a => new AgentAdminDto
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email ?? string.Empty,
                PhoneNumber = a.PhoneNumber,
                PropertyCount = countsDict.TryGetValue(a.Id, out var c) ? c : 0,
                IsActive = a.IsActive
            }).ToList();

            return Result<List<AgentAdminDto>>.Ok(list);
        }

        public async Task<Result> ToggleStatusAsync(string agentId)
        {
            var user = await _userManager.FindByIdAsync(agentId);

            if (user == null || user.UserType != nameof(Roles.Agente))
            {
                return Result.Fail("Agente no encontrado.");
            }

            user.IsActive = !user.IsActive;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return Result.Fail(updateResult.Errors.Select(e => e.Description).ToList());
            }

            return Result.Ok();
        }

        public async Task<Result> DeleteAgentWithPropertiesAsync(string agentId)
        {
            var user = await _userManager.FindByIdAsync(agentId);

            if (user == null || user.UserType != nameof(Roles.Agente))
            {
                return Result.Fail("Agente no encontrado.");
            }

            var properties = await _dbContext.Properties
                .Where(p => p.AgentId == agentId)
                .ToListAsync();

            _dbContext.Properties.RemoveRange(properties);
            await _dbContext.SaveChangesAsync();

            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return Result.Fail(deleteResult.Errors.Select(e => e.Description).ToList());
            }

            return Result.Ok();
        }
    }
}
