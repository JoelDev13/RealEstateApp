using MediatR;

namespace RealEstateApp.Application.Features.Improvements.Commands.UpdateImprovement
{
    public class UpdateImprovementCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
