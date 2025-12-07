using AutoMapper;
using RealEstateApp.Application.Dtos.Property;
using RealEstateApp.Domain.Entities;
using RealEstateApp.WebApp.Models.Property;

namespace RealEstateApp.WebApp.Mappings
{
    public class PropertyMappingProfile : Profile
    {
        public PropertyMappingProfile()
        {
            // ViewModel to DTO
            CreateMap<CreatePropertyViewModel, CreatePropertyDto>()
                .ForMember(dest => dest.ImprovementIds, opt => opt.MapFrom(src => src.SelectedImprovementIds));

            CreateMap<EditPropertyViewModel, UpdatePropertyDto>()
                .ForMember(dest => dest.ImprovementIds, opt => opt.MapFrom(src => src.SelectedImprovementIds));

            // Entity to ViewModel
            CreateMap<Property, PropertyDetailViewModel>()
                .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(src => src.PropertyType != null ? src.PropertyType.Name : ""))
                .ForMember(dest => dest.SaleType, opt => opt.MapFrom(src => src.SaleType != null ? src.SaleType.Name : ""))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images.Select(i => i.Url).ToList()))
                .ForMember(dest => dest.Improvements, opt => opt.MapFrom(src => src.Improvements.Select(i => i.Name).ToList()))
                .ForMember(dest => dest.AgentName, opt => opt.Ignore())
                .ForMember(dest => dest.AgentPhone, opt => opt.Ignore())
                .ForMember(dest => dest.AgentEmail, opt => opt.Ignore())
                .ForMember(dest => dest.AgentProfilePicture, opt => opt.Ignore());
        }
    }
}

