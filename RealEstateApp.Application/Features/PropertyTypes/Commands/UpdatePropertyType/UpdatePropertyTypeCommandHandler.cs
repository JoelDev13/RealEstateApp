using MediatR;
using RealEstateApp.Application.Dtos.PropertyTypes;
using RealEstateApp.Application.Interfaces.Repositories;

public class UpdatePropertyTypeCommandHandler
    : IRequestHandler<UpdatePropertyTypeCommand, PropertyTypeDto>
{
    private readonly IPropertyTypeRepository _propertyTypeRepository;

    public UpdatePropertyTypeCommandHandler(IPropertyTypeRepository propertyTypeRepository)
    {
        _propertyTypeRepository = propertyTypeRepository;
    }

    public async Task<PropertyTypeDto> Handle(UpdatePropertyTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _propertyTypeRepository.GetByIdAsync(request.Id);

        if (entity == null)
            throw new KeyNotFoundException($"PropertyType con Id {request.Id} no existe.");

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await _propertyTypeRepository.UpdateAsync(entity);

        return new PropertyTypeDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive
        };
    }
}
