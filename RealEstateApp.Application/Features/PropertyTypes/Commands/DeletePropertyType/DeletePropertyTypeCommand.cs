using MediatR;

namespace RealEstateApp.Application.Features.PropertyTypes.Commands.DeletePropertyType
{
    public class DeletePropertyTypeCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
