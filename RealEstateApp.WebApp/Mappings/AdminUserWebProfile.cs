using AutoMapper;
using RealEstateApp.Application.Dtos.AdminUsers;
using RealEstateApp.WebApp.Models.Admin.AdminUsers;

namespace RealEstateApp.WebApp.Mappings
{
    public class AdminUserWebProfile : Profile
    {
        public AdminUserWebProfile()
        {
            CreateMap<AdminUserDto, AdminUserViewModel>()
                .ReverseMap();

            CreateMap<AdminUserDto, AdminUserEditViewModel>();

            CreateMap<AdminUserCreateDto, AdminUserCreateViewModel>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ReverseMap()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));

            CreateMap<AdminUserUpdateDto, AdminUserEditViewModel>()
                .ReverseMap()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));
        }
    }
}

