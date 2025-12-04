using MediatR;
using RealEstateApp.Application.Dtos.PropertyTypes;

namespace RealEstateApp.Application.Features.PropertyTypes.Queries.GetPropertyTypeById
{
    public sealed record GetPropertyTypeByIdQuery(int Id) : IRequest<PropertyTypeDto>;
}
