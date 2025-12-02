using AutoMapper;
using RealEstateApp.Application.Dtos.Improvements;
using RealEstateApp.Application.Dtos.PropertyTypes;
using RealEstateApp.Application.Dtos.SaleTypes;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PropertyType, PropertyTypeDto>();
            CreateMap<Improvement, ImprovementDto>();
            CreateMap<SaleType, SaleTypeDto>();
        }
    }
}
