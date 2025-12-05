using MediatR;

namespace RealEstateApp.Application.Features.PropertyTypes.Commands.TogglePropertyTypeStatus
{
    public class TogglePropertyTypeStatusCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
