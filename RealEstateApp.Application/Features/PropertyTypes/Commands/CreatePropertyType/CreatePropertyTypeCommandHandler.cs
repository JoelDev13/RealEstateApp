using MediatR;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Features.PropertyTypes.Commands.CreatePropertyType
{
    public class CreatePropertyTypeCommandHandler
        : IRequestHandler<CreatePropertyTypeCommand, int>
    {
        private readonly IPropertyTypeRepository _propertyTypeRepository;

        public CreatePropertyTypeCommandHandler(IPropertyTypeRepository propertyTypeRepository)
        {
            _propertyTypeRepository = propertyTypeRepository;
        }

        public async Task<int> Handle(CreatePropertyTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = new PropertyType
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _propertyTypeRepository.AddAsync(entity);

            return entity.Id;
        }
    }
}
