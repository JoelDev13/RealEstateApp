using AutoMapper;
using RealEstateApp.Application.Dtos.Agent;
using RealEstateApp.WebApp.Models.Agents;

namespace RealEstateApp.WebApp.Mappings
{
    public class AgentProfileMappingProfile : Profile
    {
        public AgentProfileMappingProfile()
        {
            CreateMap<AgentProfileDto, AgentProfileViewModel>()
                .ReverseMap();
        }
    }
}

