using MediatR;
using RealEstateApp.Application.Dtos.SaleTypes;

namespace RealEstateApp.Application.Features.SaleTypes.Queries.GetSaleTypes
{
    public class GetSaleTypesQuery : IRequest<List<SaleTypeDto>>
    {
    }
}
