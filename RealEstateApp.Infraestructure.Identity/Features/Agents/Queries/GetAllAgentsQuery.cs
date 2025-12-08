using MediatR;
using RealEstateApp.Application.Dtos.Agents;

namespace RealEstateApp.Infraestructure.Identity.Features.Agents.Queries
{
    public record GetAllAgentsQuery : IRequest<List<AgentDto>>
    {
    }

    public record GetAgentByIdQuery : IRequest<AgentDto?>
    {
        public string Id { get; set; } = string.Empty;
    }

    public record GetAgentPropertiesQuery : IRequest<List<AgentPropertyDto>>
    {
        public string AgentId { get; set; } = string.Empty;
    }

    public record ChangeAgentStatusCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
