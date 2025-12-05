using MediatR;

namespace RealEstateApp.Application.Features.SaleTypes.Commands.ToggleSaleTypeStatus
{
    public class ToggleSaleTypeStatusCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
