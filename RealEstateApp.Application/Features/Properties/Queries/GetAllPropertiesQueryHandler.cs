using MediatR;
using RealEstateApp.Application.Dtos.Property;
using RealEstateApp.Application.Interfaces.Repositories;

namespace RealEstateApp.Application.Features.Properties.Queries
{
    public class GetAllPropertiesQueryHandler : IRequestHandler<GetAllPropertiesQuery, List<PropertyDto>>
    {
        private readonly IPropertyRepository _propertyRepository;

        public GetAllPropertiesQueryHandler(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<List<PropertyDto>> Handle(GetAllPropertiesQuery request, CancellationToken cancellationToken)
        {
            var properties = await _propertyRepository.GetAllAsync();
            
            var propertyDtos = new List<PropertyDto>();
            
            foreach (var property in properties)
            {
                propertyDtos.Add(new PropertyDto
                {
                    Id = property.Id,
                    Code = property.Code,
                    PropertyType = property.PropertyType?.Name ?? "",
                    SaleType = property.SaleType?.Name ?? "",
                    Price = (decimal)property.Price,
                    Size = (decimal)property.SizeInSquareMeters,
                    Bedrooms = property.Bedrooms,
                    Bathrooms = property.Bathrooms,
                    Description = property.Description,
                    AgentId = property.AgentId,
                    Status = property.Status.ToString(),
                    IsSold = property.IsSold,
                    CreatedAt = property.CreatedAt
                });
            }

            return propertyDtos;
        }
    }
}
