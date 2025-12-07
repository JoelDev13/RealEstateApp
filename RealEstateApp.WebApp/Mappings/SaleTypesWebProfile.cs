using AutoMapper;
using RealEstateApp.Application.Dtos.SaleTypes;
using RealEstateApp.Web.Models.Admin.SaleTypes;

namespace RealEstateApp.WebApp.Mappings
{
    public class SaleTypesWebProfile : Profile
    {
        public SaleTypesWebProfile()
        {
            CreateMap<SaleTypeDto, SaleTypeViewModel>()
                .ReverseMap();

            CreateMap<SaleTypeDto, SaleTypeCreateEditViewModel>()
                .ReverseMap();
        }
    }
}

