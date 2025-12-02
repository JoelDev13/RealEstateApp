using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Dtos.PropertyTypes;
using RealEstateApp.Application.Features.PropertyTypes.Queries.GetPropertyTypeById;
using RealEstateApp.Application.Interfaces;

public class GetPropertyTypeByIdQueryHandler
    : IRequestHandler<GetPropertyTypeByIdQuery, PropertyTypeDto>
{
    private readonly IApplicationDbContext _context;

    public GetPropertyTypeByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PropertyTypeDto> Handle(GetPropertyTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.PropertyTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

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
