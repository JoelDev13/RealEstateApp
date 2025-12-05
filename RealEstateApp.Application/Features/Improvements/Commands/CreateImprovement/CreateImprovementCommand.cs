using MediatR;

namespace RealEstateApp.Application.Features.Improvements.Commands.CreateImprovement
{
    public class CreateImprovementCommand : IRequest<int>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
