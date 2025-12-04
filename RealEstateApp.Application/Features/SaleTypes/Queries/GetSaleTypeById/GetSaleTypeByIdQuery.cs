using MediatR;
using RealEstateApp.Application.Dtos.SaleTypes;

namespace RealEstateApp.Application.Features.SaleTypes.Queries.GetSaleTypeById
{
    public sealed record GetSaleTypeByIdQuery(int Id) : IRequest<SaleTypeDto>;
}
