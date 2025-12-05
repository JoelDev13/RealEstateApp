using MediatR;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Features.Improvements.Commands.CreateImprovement
{
    public class CreateImprovementCommandHandler
        : IRequestHandler<CreateImprovementCommand, int>
    {
        private readonly IImprovementRepository _improvementRepository;

        public CreateImprovementCommandHandler(IImprovementRepository improvementRepository)
        {
            _improvementRepository = improvementRepository;
        }

        public async Task<int> Handle(CreateImprovementCommand request, CancellationToken cancellationToken)
        {
            var entity = new Improvement
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _improvementRepository.AddAsync(entity);

            return entity.Id;
        }
    }
}
