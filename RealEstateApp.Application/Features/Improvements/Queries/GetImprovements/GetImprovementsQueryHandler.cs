using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Dtos.Improvements;
using RealEstateApp.Application.Interfaces;

namespace RealEstateApp.Application.Features.Improvements.Queries.GetImprovements
{
    public class GetImprovementsQueryHandler
        : IRequestHandler<GetImprovementsQuery, List<ImprovementDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetImprovementsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ImprovementDto>> Handle(GetImprovementsQuery request, CancellationToken cancellationToken)
        {
            var list = await _context.Improvements
                .OrderBy(i => i.Name)
                .Select(i => new ImprovementDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    IsActive = i.IsActive
                })
                .ToListAsync(cancellationToken);

            return list;
        }
    }
}
