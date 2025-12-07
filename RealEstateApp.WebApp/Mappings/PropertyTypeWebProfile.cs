using AutoMapper;
using RealEstateApp.Application.Dtos.PropertyTypes;
using RealEstateApp.Web.Models.Admin.PropertyTypes;

namespace RealEstateApp.WebApp.Mappings
{
    public class PropertyTypesWebProfile : Profile
    {
        public PropertyTypesWebProfile()
        {
            CreateMap<PropertyTypeDto, PropertyTypeViewModel>()
                .ReverseMap();

            CreateMap<PropertyTypeDto, PropertyTypeCreateEditViewModel>()
                .ReverseMap();
        }
    }
}

