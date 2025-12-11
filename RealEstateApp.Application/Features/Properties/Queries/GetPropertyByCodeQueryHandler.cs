using MediatR;
using RealEstateApp.Application.Dtos.Property;
using RealEstateApp.Application.Interfaces.Repositories;

namespace RealEstateApp.Application.Features.Properties.Queries
{
    public class GetPropertyByCodeQueryHandler : IRequestHandler<GetPropertyByCodeQuery, PropertyDto?>
    {
        private readonly IPropertyRepository _propertyRepository;

        public GetPropertyByCodeQueryHandler(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<PropertyDto?> Handle(GetPropertyByCodeQuery request, CancellationToken cancellationToken)
        {
            var property = await _propertyRepository.GetByCodeAsync(request.Code);
            
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
