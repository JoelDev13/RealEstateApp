using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Dtos.PropertyTypes;
using RealEstateApp.Application.Interfaces.Repositories;

namespace RealEstateApp.Application.Features.PropertyTypes.Queries.GetPropertyTypes
{
    public class GetPropertyTypesQueryHandler
        : IRequestHandler<GetPropertyTypesQuery, List<PropertyTypeDto>>
    {
        private readonly IPropertyTypeRepository _propertyTypeRepository;

        public GetPropertyTypesQueryHandler(IPropertyTypeRepository propertyTypeRepository)
        {
            _propertyTypeRepository = propertyTypeRepository;
        }

        public async Task<List<PropertyTypeDto>> Handle(GetPropertyTypesQuery request, CancellationToken cancellationToken)
        {
            var list = await _propertyTypeRepository
                .Query()
                .OrderBy(pt => pt.Name)
                .ToListAsync(cancellationToken);

            return list
                .Select(pt => new PropertyTypeDto
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    Description = pt.Description,
                    IsActive = pt.IsActive
                })
                .ToList();
        }
    }
}
