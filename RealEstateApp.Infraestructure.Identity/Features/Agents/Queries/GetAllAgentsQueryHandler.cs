using MediatR;
using Microsoft.AspNetCore.Identity;
using RealEstateApp.Application.Dtos.Agents;
using AutoMapper;
using RealEstateApp.Application.Interfaces.Repositories;
using AppUser = RealEstateApp.Infrastructure.Identity.Entities.AppUser;

namespace RealEstateApp.Infraestructure.Identity.Features.Agents.Queries
{
    public class GetAllAgentsQueryHandler : IRequestHandler<GetAllAgentsQuery, List<AgentDto>>,
                                           IRequestHandler<GetAgentByIdQuery, AgentDto?>,
                                           IRequestHandler<GetAgentPropertiesQuery, List<AgentPropertyDto>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;

        public GetAllAgentsQueryHandler(UserManager<AppUser> userManager, IPropertyRepository propertyRepository, IMapper mapper)
        {
            _userManager = userManager;
            _propertyRepository = propertyRepository;
            _mapper = mapper;
        }

        public async Task<List<AgentDto>> Handle(GetAllAgentsQuery request, CancellationToken cancellationToken)
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

        public async Task<AgentDto?> Handle(GetAgentByIdQuery request, CancellationToken cancellationToken)
        {
            var agent = await _userManager.FindByIdAsync(request.Id);
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

        public async Task<List<AgentPropertyDto>> Handle(GetAgentPropertiesQuery request, CancellationToken cancellationToken)
        {
            var properties = await _propertyRepository.GetByAgentAsync(request.AgentId);
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
    }

    public class ChangeAgentStatusCommandHandler : IRequestHandler<ChangeAgentStatusCommand, bool>
    {
        private readonly UserManager<AppUser> _userManager;

        public ChangeAgentStatusCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(ChangeAgentStatusCommand request, CancellationToken cancellationToken)
        {
            var agent = await _userManager.FindByIdAsync(request.Id);
            if (agent == null || !await _userManager.IsInRoleAsync(agent, "Agente"))
            {
                return false;
            }

            agent.IsActive = request.IsActive;
            var result = await _userManager.UpdateAsync(agent);
            return result.Succeeded;
        }
    }
}
