using MediatR;

namespace RealEstateApp.Application.Features.SaleTypes.Commands.UpdateSaleType
{
    public class UpdateSaleTypeCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
