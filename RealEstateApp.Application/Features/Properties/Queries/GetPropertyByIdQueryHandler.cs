using MediatR;
using RealEstateApp.Application.Dtos.Property;
using RealEstateApp.Application.Interfaces.Repositories;

namespace RealEstateApp.Application.Features.Properties.Queries
{
    public class GetPropertyByIdQueryHandler : IRequestHandler<GetPropertyByIdQuery, PropertyDto?>
    {
        private readonly IPropertyRepository _propertyRepository;

        public GetPropertyByIdQueryHandler(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<PropertyDto?> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
        {
            var property = await _propertyRepository.GetByIdAsync(request.Id);
            
            if (property == null)
                return null;

            return new PropertyDto
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
            };
        }
    }
}
