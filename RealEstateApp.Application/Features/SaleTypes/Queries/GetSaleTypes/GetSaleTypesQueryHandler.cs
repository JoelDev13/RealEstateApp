using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Dtos.SaleTypes;
using RealEstateApp.Application.Interfaces;

namespace RealEstateApp.Application.Features.SaleTypes.Queries.GetSaleTypes
{
    public class GetSaleTypesQueryHandler
        : IRequestHandler<GetSaleTypesQuery, List<SaleTypeDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetSaleTypesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SaleTypeDto>> Handle(GetSaleTypesQuery request, CancellationToken cancellationToken)
        {
            var list = await _context.SaleTypes
                .OrderBy(st => st.Name)
                .Select(st => new SaleTypeDto
                {
                    Id = st.Id,
                    Name = st.Name,
                    Description = st.Description,
                    IsActive = st.IsActive
                })
                .ToListAsync(cancellationToken);

            return list;
        }
    }
}
