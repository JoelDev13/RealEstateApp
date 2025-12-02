using MediatR;

namespace RealEstateApp.Application.Features.Improvements.Commands.ToggleImprovementStatus
{
    public class ToggleImprovementStatusCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
