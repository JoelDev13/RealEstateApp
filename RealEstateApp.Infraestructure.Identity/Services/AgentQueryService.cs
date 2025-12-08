using Microsoft.AspNetCore.Identity;
using RealEstateApp.Application.Dtos.Agents;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Infrastructure.Identity.Entities;
using AutoMapper;

namespace RealEstateApp.Infraestructure.Identity.Services
{
    public class AgentQueryService : IAgentQueryService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;

        public AgentQueryService(UserManager<AppUser> userManager, IPropertyRepository propertyRepository, IMapper mapper)
        {
            _userManager = userManager;
            _propertyRepository = propertyRepository;
            _mapper = mapper;
        }

        public async Task<List<AgentDto>> GetAllAgentsAsync()
        {
            var agents = await _userManager.GetUsersInRoleAsync("Agente");
            var agentDtos = new List<AgentDto>();

            foreach (var agent in agents)
            {
                var properties = await _propertyRepository.GetByAgentAsync(agent.Id);
                var agentDto = new AgentDto
                {
                    Id = agent.Id,
                    Nombre = agent.FirstName ?? string.Empty,
                    Apellido = agent.LastName ?? string.Empty,
                    Email = agent.Email ?? string.Empty,
                    Phone = agent.PhoneNumber ?? string.Empty,
                    PropertyCount = properties?.Count ?? 0
                };
                agentDtos.Add(agentDto);
            }

            return agentDtos;
        }

        public async Task<AgentDto?> GetAgentByIdAsync(string id)
        {
            var agent = await _userManager.FindByIdAsync(id);
            if (agent == null || !await _userManager.IsInRoleAsync(agent, "Agente"))
            {
                return null;
            }

            var properties = await _propertyRepository.GetByAgentAsync(agent.Id);
            return new AgentDto
            {
                Id = agent.Id,
                Nombre = agent.FirstName ?? string.Empty,
                Apellido = agent.LastName ?? string.Empty,
                Email = agent.Email ?? string.Empty,
                Phone = agent.PhoneNumber ?? string.Empty,
                PropertyCount = properties?.Count ?? 0
            };
        }

        public async Task<List<AgentPropertyDto>> GetAgentPropertiesAsync(string agentId)
        {
            var properties = await _propertyRepository.GetByAgentAsync(agentId);
            if (properties == null || !properties.Any())
            {
                return new List<AgentPropertyDto>();
            }

            return properties.Select(p => new AgentPropertyDto
            {
                Id = p.Id.ToString(),
                Code = p.Code,
                PropertyType = p.PropertyType?.Name ?? string.Empty,
                SaleType = p.SaleType?.Name ?? string.Empty,
                Price = p.Price,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                Size = (decimal)p.SizeInSquareMeters,
                Description = p.Description,
                IsSold = p.IsSold,
                CreatedAt = p.CreatedAt
            }).ToList();
        }

        public async Task<bool> ChangeAgentStatusAsync(string id, bool isActive)
        {
            var agent = await _userManager.FindByIdAsync(id);
            if (agent == null || !await _userManager.IsInRoleAsync(agent, "Agente"))
            {
                return false;
            }

            agent.IsActive = isActive;
            var result = await _userManager.UpdateAsync(agent);
            return result.Succeeded;
        }
    }
}
