using MediatR;
using RealEstateApp.Application.Interfaces.Repositories;

namespace RealEstateApp.Application.Features.Improvements.Commands.UpdateImprovement
{
    public class UpdateImprovementCommandHandler
        : IRequestHandler<UpdateImprovementCommand, Unit>
    {
        private readonly IImprovementRepository _improvementRepository;

        public UpdateImprovementCommandHandler(IImprovementRepository improvementRepository)
        {
            _improvementRepository = improvementRepository;
        }

        public async Task<Unit> Handle(UpdateImprovementCommand request, CancellationToken cancellationToken)
        {
            var entity = await _improvementRepository.GetByIdAsync(request.Id);

            if (entity == null)
                throw new KeyNotFoundException($"Improvement con Id {request.Id} no existe.");

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _improvementRepository.UpdateAsync(entity);

            return Unit.Value;
        }
    }
}
