using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Dtos.Improvements;
using RealEstateApp.Application.Interfaces.Repositories;

namespace RealEstateApp.Application.Features.Improvements.Queries.GetImprovements
{
    public class GetImprovementsQueryHandler
        : IRequestHandler<GetImprovementsQuery, List<ImprovementDto>>
    {
        private readonly IImprovementRepository _improvementRepository;

        public GetImprovementsQueryHandler(IImprovementRepository improvementRepository)
        {
            _improvementRepository = improvementRepository;
        }

        public async Task<List<ImprovementDto>> Handle(GetImprovementsQuery request, CancellationToken cancellationToken)
        {
            var list = await _improvementRepository
                .Query()
                .OrderBy(i => i.Name)
                .ToListAsync(cancellationToken);

            return list
                .Select(i => new ImprovementDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    IsActive = i.IsActive
                })
                .ToList();
        }
    }
}
