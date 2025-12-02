using MediatR;
using RealEstateApp.Application.Dtos.PropertyTypes;

namespace RealEstateApp.Application.Features.PropertyTypes.Queries.GetPropertyTypeById
{
    public class GetPropertyTypeByIdQuery : IRequest<PropertyTypeDto>
    {
        public int Id { get; set; }
    }
}
