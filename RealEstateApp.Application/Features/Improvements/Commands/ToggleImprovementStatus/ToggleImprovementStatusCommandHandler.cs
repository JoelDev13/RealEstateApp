using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Interfaces;

namespace RealEstateApp.Application.Features.Improvements.Commands.ToggleImprovementStatus
{
    public class ToggleImprovementStatusCommandHandler
        : IRequestHandler<ToggleImprovementStatusCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public ToggleImprovementStatusCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ToggleImprovementStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Improvements
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"Improvement con Id {request.Id} no existe.");

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
