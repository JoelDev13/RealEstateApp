using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Dtos.SaleTypes;
using RealEstateApp.Application.Interfaces;

namespace RealEstateApp.Application.Features.SaleTypes.Queries.GetSaleTypeById
{
    public class GetSaleTypeByIdQueryHandler
        : IRequestHandler<GetSaleTypeByIdQuery, SaleTypeDto>
    {
        private readonly IApplicationDbContext _context;

        public GetSaleTypeByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SaleTypeDto> Handle(GetSaleTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.SaleTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"SaleType con Id {request.Id} no existe.");

            return new SaleTypeDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive
            };
        }
    }
}
