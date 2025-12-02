using MediatR;
using RealEstateApp.Application.Dtos.Improvements;
using RealEstateApp.Application.Interfaces.Repositories;

namespace RealEstateApp.Application.Features.Improvements.Queries.GetImprovementById
{
    public class GetImprovementByIdQueryHandler
        : IRequestHandler<GetImprovementByIdQuery, ImprovementDto>
    {
        private readonly IImprovementRepository _improvementRepository;

        public GetImprovementByIdQueryHandler(IImprovementRepository improvementRepository)
        {
            _improvementRepository = improvementRepository;
        }

        public async Task<ImprovementDto> Handle(GetImprovementByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _improvementRepository.GetByIdAsync(request.Id);

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
