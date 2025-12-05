using MediatR;

namespace RealEstateApp.Application.Features.SaleTypes.Commands.DeleteSaleType
{
    public class DeleteSaleTypeCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
