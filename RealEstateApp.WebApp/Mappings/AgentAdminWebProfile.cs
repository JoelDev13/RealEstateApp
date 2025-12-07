using AutoMapper;
using RealEstateApp.Application.Dtos.Agents;
using RealEstateApp.WebApp.Models.Agents;

namespace RealEstateApp.WebApp.Mappings
{
    public class AgentAdminWebProfile : Profile
    {
        public AgentAdminWebProfile()
        {
            CreateMap<AgentAdminDto, AgentListItemViewModel>();
        }
    }
}

