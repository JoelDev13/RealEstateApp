using MediatR;

namespace RealEstateApp.Application.Features.SaleTypes.Commands.CreateSaleType
{
    public class CreateSaleTypeCommand : IRequest<int>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
