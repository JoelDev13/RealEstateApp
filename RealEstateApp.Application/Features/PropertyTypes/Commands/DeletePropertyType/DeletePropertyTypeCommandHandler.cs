using MediatR;
using RealEstateApp.Application.Interfaces.Repositories;

namespace RealEstateApp.Application.Features.PropertyTypes.Commands.DeletePropertyType
{
    public class DeletePropertyTypeCommandHandler
        : IRequestHandler<DeletePropertyTypeCommand, Unit>
    {
        private readonly IPropertyTypeRepository _propertyTypeRepository;

        public DeletePropertyTypeCommandHandler(IPropertyTypeRepository propertyTypeRepository)
        {
            _propertyTypeRepository = propertyTypeRepository;
        }

        public async Task<Unit> Handle(DeletePropertyTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _propertyTypeRepository.GetByIdAsync(request.Id);

            if (entity == null)
                throw new KeyNotFoundException($"PropertyType con Id {request.Id} no existe.");

            await _propertyTypeRepository.DeleteAsync(entity);

            return Unit.Value;
        }
    }
}
