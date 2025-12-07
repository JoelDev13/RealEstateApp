using AutoMapper;
using RealEstateApp.Application.Dtos.Improvements;
using RealEstateApp.Web.Models.Admin.Improvements;

namespace RealEstateApp.WebApp.Mappings
{
    public class ImprovementsWebProfile : Profile
    {
        public ImprovementsWebProfile()
        {
            CreateMap<ImprovementDto, ImprovementViewModel>()
                .ReverseMap();

            CreateMap<ImprovementDto, ImprovementCreateEditViewModel>()
                .ReverseMap();
        }
    }
}

