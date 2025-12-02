using AutoMapper;
using RealEstateApp.Application.Dtos.Improvements;
using RealEstateApp.Application.Dtos.PropertyTypes;
using RealEstateApp.Application.Dtos.SaleTypes;
using RealEstateApp.Application.Features.Improvements.Commands.CreateImprovement;
using RealEstateApp.Application.Features.Improvements.Commands.UpdateImprovement;
using RealEstateApp.Application.Features.PropertyTypes.Commands.CreatePropertyType;
using RealEstateApp.Application.Features.PropertyTypes.Commands.UpdatePropertyType;
using RealEstateApp.Application.Features.SaleTypes.Commands.CreateSaleType;
using RealEstateApp.Application.Features.SaleTypes.Commands.UpdateSaleType;
using RealEstateApp.Web.Models.Admin.Improvements;
using RealEstateApp.Web.Models.Admin.PropertyTypes;
using RealEstateApp.Web.Models.Admin.SaleTypes;

namespace RealEstateApp.Web.Mappings
{
    public class WebMappingProfile : Profile
    {
        public WebMappingProfile()
        {
            // PropertyTypes
            CreateMap<PropertyTypeDto, PropertyTypeViewModel>();
            CreateMap<PropertyTypeDto, PropertyTypeCreateEditViewModel>();
            CreateMap<PropertyTypeCreateEditViewModel, CreatePropertyTypeCommand>();
            CreateMap<PropertyTypeCreateEditViewModel, UpdatePropertyTypeCommand>();


            // SaleTypes
            CreateMap<SaleTypeDto, SaleTypeViewModel>();
            CreateMap<SaleTypeDto, SaleTypeCreateEditViewModel>();
            CreateMap<SaleTypeCreateEditViewModel, CreateSaleTypeCommand>();
            CreateMap<SaleTypeCreateEditViewModel, UpdateSaleTypeCommand>();

            //Improvements
            CreateMap<ImprovementDto, ImprovementViewModel>();
            CreateMap<ImprovementDto, ImprovementCreateEditViewModel>();
            CreateMap<ImprovementCreateEditViewModel, CreateImprovementCommand>();
            CreateMap<ImprovementCreateEditViewModel, UpdateImprovementCommand>();
        }
    }
}
