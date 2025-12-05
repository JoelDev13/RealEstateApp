using MediatR;
using RealEstateApp.Application.Interfaces.Repositories;

namespace RealEstateApp.Application.Features.Improvements.Commands.ToggleImprovementStatus
{
    public class ToggleImprovementStatusCommandHandler
        : IRequestHandler<ToggleImprovementStatusCommand, Unit>
    {
        private readonly IImprovementRepository _improvementRepository;

        public ToggleImprovementStatusCommandHandler(IImprovementRepository improvementRepository)
        {
            _improvementRepository = improvementRepository;
        }

        public async Task<Unit> Handle(ToggleImprovementStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _improvementRepository.GetByIdAsync(request.Id);

            if (entity == null)
                throw new KeyNotFoundException($"Improvement con Id {request.Id} no existe.");

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _improvementRepository.UpdateAsync(entity);

            return Unit.Value;
        }
    }
}
