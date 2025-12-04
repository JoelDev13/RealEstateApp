using MediatR;
using RealEstateApp.Application.Dtos.PropertyTypes;
using RealEstateApp.Application.Features.PropertyTypes.Queries.GetPropertyTypeById;
using RealEstateApp.Application.Interfaces.Repositories;

public class GetPropertyTypeByIdQueryHandler
    : IRequestHandler<GetPropertyTypeByIdQuery, PropertyTypeDto>
{
    private readonly IPropertyTypeRepository _propertyTypeRepository;

    public GetPropertyTypeByIdQueryHandler(IPropertyTypeRepository propertyTypeRepository)
    {
        _propertyTypeRepository = propertyTypeRepository;
    }

    public async Task<PropertyTypeDto> Handle(GetPropertyTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _propertyTypeRepository.GetByIdAsync(request.Id);

        if (entity == null)
            throw new KeyNotFoundException($"PropertyType con Id {request.Id} no existe.");

        return new PropertyTypeDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive
        };
    }
}
