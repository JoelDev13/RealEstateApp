using MediatR;
using RealEstateApp.Application.Interfaces.Repositories;

namespace RealEstateApp.Application.Features.PropertyTypes.Commands.TogglePropertyTypeStatus
{
    public class TogglePropertyTypeStatusCommandHandler
        : IRequestHandler<TogglePropertyTypeStatusCommand, Unit>
    {
        private readonly IPropertyTypeRepository _propertyTypeRepository;

        public TogglePropertyTypeStatusCommandHandler(IPropertyTypeRepository propertyTypeRepository)
        {
            _propertyTypeRepository = propertyTypeRepository;
        }

        public async Task<Unit> Handle(TogglePropertyTypeStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _propertyTypeRepository.GetByIdAsync(request.Id);

            if (entity == null)
                throw new KeyNotFoundException($"PropertyType con Id {request.Id} no existe.");

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _propertyTypeRepository.UpdateAsync(entity);

            return Unit.Value;
        }
    }
}
