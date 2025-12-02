using MediatR;
using RealEstateApp.Application.Dtos.SaleTypes;

namespace RealEstateApp.Application.Features.SaleTypes.Queries.GetSaleTypeById
{
    public class GetSaleTypeByIdQuery : IRequest<SaleTypeDto>
    {
        public int Id { get; set; }
    }
}
