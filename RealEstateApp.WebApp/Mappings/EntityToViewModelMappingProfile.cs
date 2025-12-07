using AutoMapper;
using RealEstateApp.Application.Dtos.Property;
using RealEstateApp.Domain.Entities;
using RealEstateApp.WebApp.Models.Property;

namespace RealEstateApp.WebApp.Mappings
{
    public class EntityToViewModelMappingProfile : Profile
    {
        public EntityToViewModelMappingProfile()
        {
            // Entity to ViewModel
            CreateMap<Property, CreatePropertyViewModel>()
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.PropertyTypes, opt => opt.Ignore())
                .ForMember(dest => dest.SaleTypes, opt => opt.Ignore())
                .ForMember(dest => dest.Improvements, opt => opt.Ignore());

            CreateMap<Property, EditPropertyViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.PropertyTypeId, opt => opt.MapFrom(src => src.PropertyTypeId.ToString()))
                .ForMember(dest => dest.SaleTypeId, opt => opt.MapFrom(src => src.SaleTypeId.ToString()))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => (decimal)src.SizeInSquareMeters))
                .ForMember(dest => dest.NewImages, opt => opt.Ignore())
                .ForMember(dest => dest.PropertyTypes, opt => opt.Ignore())
                .ForMember(dest => dest.SaleTypes, opt => opt.Ignore())
                .ForMember(dest => dest.Improvements, opt => opt.Ignore())
                .ForMember(dest => dest.SelectedImprovementIds, opt => opt.MapFrom(src => src.Improvements.Select(i => i.Id.ToString()).ToList()));
        }
    }
}

