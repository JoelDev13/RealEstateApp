using MediatR;
using RealEstateApp.Application.Interfaces.Repositories;

namespace RealEstateApp.Application.Features.PropertyTypes.Commands.UpdatePropertyType
{
    public class UpdatePropertyTypeCommandHandler
        : IRequestHandler<UpdatePropertyTypeCommand, Unit>
    {
        private readonly IPropertyTypeRepository _propertyTypeRepository;

        public UpdatePropertyTypeCommandHandler(IPropertyTypeRepository propertyTypeRepository)
        {
            _propertyTypeRepository = propertyTypeRepository;
        }

        public async Task<Unit> Handle(UpdatePropertyTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _propertyTypeRepository.GetByIdAsync(request.Id);

            if (entity == null)
                throw new KeyNotFoundException($"PropertyType con Id {request.Id} no existe.");

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _propertyTypeRepository.UpdateAsync(entity);

            return Unit.Value;
        }
    }
}
