using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Dtos.Improvements;
using RealEstateApp.Application.Interfaces;

namespace RealEstateApp.Application.Features.Improvements.Queries.GetImprovementById
{
    public class GetImprovementByIdQueryHandler
        : IRequestHandler<GetImprovementByIdQuery, ImprovementDto>
    {
        private readonly IApplicationDbContext _context;

        public GetImprovementByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ImprovementDto> Handle(GetImprovementByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.Improvements
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"Improvement con Id {request.Id} no existe.");

            return new ImprovementDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive
            };
        }
    }
}
