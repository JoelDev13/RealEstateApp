using MediatR;
using RealEstateApp.Application.Dtos.Property;

namespace RealEstateApp.Application.Features.Properties.Queries
{
    public record GetAllPropertiesQuery : IRequest<List<PropertyDto>>
    {
    }

    public record GetPropertyByIdQuery : IRequest<PropertyDto?>
    {
        public int Id { get; set; }
    }

    public record GetPropertyByCodeQuery : IRequest<PropertyDto?>
    {
        public string Code { get; set; } = string.Empty;
    }
}

