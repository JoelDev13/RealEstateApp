using AutoMapper;
using RealEstateApp.Application.Dtos.DeveloperUsers;
using RealEstateApp.WebApp.Models.Admin.DeveloperUser;

namespace RealEstateApp.WebApp.Mappings
{
    public class DeveloperUserMappingProfile : Profile
    {
        public DeveloperUserMappingProfile()
        {
            CreateMap<DeveloperUserDto, DevUserViewModel>().ReverseMap();
            CreateMap<DeveloperUserCreateDto, DevUserCreateViewModel>().ReverseMap();
            CreateMap<DeveloperUserUpdateDto, DevUserEditViewModel>().ReverseMap();
            CreateMap<DeveloperUserDto, DevUserEditViewModel>();
        }
    }
}

