using MediatR;
using RealEstateApp.Application.Dtos.PropertyTypes;

namespace RealEstateApp.Application.Features.PropertyTypes.Queries.GetPropertyTypes
{
    public class GetPropertyTypesQuery : IRequest<List<PropertyTypeDto>> { }

}
