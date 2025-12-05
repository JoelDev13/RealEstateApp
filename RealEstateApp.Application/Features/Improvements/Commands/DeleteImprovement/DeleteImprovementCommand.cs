using MediatR;

namespace RealEstateApp.Application.Features.Improvements.Commands.DeleteImprovement
{
    public class DeleteImprovementCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
