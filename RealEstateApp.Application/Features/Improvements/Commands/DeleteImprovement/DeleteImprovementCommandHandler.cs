using MediatR;
using RealEstateApp.Application.Interfaces.Repositories;

namespace RealEstateApp.Application.Features.Improvements.Commands.DeleteImprovement
{
    public class DeleteImprovementCommandHandler
        : IRequestHandler<DeleteImprovementCommand, Unit>
    {
        private readonly IImprovementRepository _improvementRepository;

        public DeleteImprovementCommandHandler(IImprovementRepository improvementRepository)
        {
            _improvementRepository = improvementRepository;
        }

        public async Task<Unit> Handle(DeleteImprovementCommand request, CancellationToken cancellationToken)
        {
            var entity = await _improvementRepository.GetByIdAsync(request.Id);

            if (entity == null)
                throw new KeyNotFoundException($"Improvement con Id {request.Id} no existe.");

            await _improvementRepository.DeleteAsync(entity);

            return Unit.Value;
        }
    }
}
