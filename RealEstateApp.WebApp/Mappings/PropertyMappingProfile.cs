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
            CreateMap<CreatePropertyViewModel, CreatePropertyDto>()
                .ForMember(dest => dest.ImprovementIds, opt => opt.MapFrom(src => src.SelectedImprovementIds));

            CreateMap<EditPropertyViewModel, UpdatePropertyDto>()
                .ForMember(dest => dest.ImprovementIds, opt => opt.MapFrom(src => src.SelectedImprovementIds));

            CreateMap<Property, PropertyDetailViewModel>()
                .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(src => src.PropertyType != null ? src.PropertyType.Name : ""))
                .ForMember(dest => dest.SaleType, opt => opt.MapFrom(src => src.SaleType != null ? src.SaleType.Name : ""))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images.Select(i => i.Url).ToList()))
                .ForMember(dest => dest.Improvements, opt => opt.MapFrom(src => src.Improvements.Select(i => i.Name).ToList()))
                .ForMember(dest => dest.AgentName, opt => opt.Ignore())
                .ForMember(dest => dest.AgentPhone, opt => opt.Ignore())
                .ForMember(dest => dest.AgentEmail, opt => opt.Ignore())
                .ForMember(dest => dest.AgentProfilePicture, opt => opt.Ignore());

            CreateMap<Property, PropertyDto>()
                .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(src => src.PropertyType != null ? src.PropertyType.Name : ""))
                .ForMember(dest => dest.SaleType, opt => opt.MapFrom(src => src.SaleType != null ? src.SaleType.Name : ""))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => (decimal)src.SizeInSquareMeters))
                .ForMember(dest => dest.Improvements, opt => opt.MapFrom(src => src.Improvements.Select(i => i.Name).ToList()))
                .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src => GetMainImageUrl(src)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.AgentName, opt => opt.Ignore());

            CreateMap<Property, EditPropertyViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.PropertyTypeId, opt => opt.MapFrom(src => src.PropertyTypeId.ToString()))
                .ForMember(dest => dest.SaleTypeId, opt => opt.MapFrom(src => src.SaleTypeId.ToString()))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => (decimal)src.SizeInSquareMeters))
                .ForMember(dest => dest.SelectedImprovementIds, opt => opt.MapFrom(src => src.Improvements.Select(i => i.Id.ToString()).ToList()))
                .ForMember(dest => dest.ExistingImages, opt => opt.MapFrom(src => src.Images.Select(i => i.Url).ToList()))
                .ForMember(dest => dest.PropertyTypes, opt => opt.Ignore())
                .ForMember(dest => dest.SaleTypes, opt => opt.Ignore())
                .ForMember(dest => dest.Improvements, opt => opt.Ignore())
                .ForMember(dest => dest.NewImages, opt => opt.Ignore());
        }

        // Método helper para obtener la imagen principal
        private static string GetMainImageUrl(Property property)
        {
            if (property.Images == null || !property.Images.Any())
                return string.Empty;

            var primaryImage = property.Images.FirstOrDefault(img => img.IsPrimary);
            if (primaryImage != null)
                return primaryImage.Url;

            // Si no hay imagen principal, tomar la primera
            var firstImage = property.Images.FirstOrDefault();
            return firstImage?.Url ?? string.Empty;
        }
    }
}