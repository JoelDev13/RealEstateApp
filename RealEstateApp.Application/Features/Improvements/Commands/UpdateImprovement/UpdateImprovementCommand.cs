using MediatR;
using RealEstateApp.Application.Dtos.Improvements;

public class UpdateImprovementCommand : IRequest<ImprovementDto>
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
