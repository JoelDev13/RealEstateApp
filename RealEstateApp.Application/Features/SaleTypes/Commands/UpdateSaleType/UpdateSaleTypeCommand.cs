using MediatR;
using RealEstateApp.Application.Dtos.SaleTypes;

public class UpdateSaleTypeCommand : IRequest<SaleTypeDto>
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
