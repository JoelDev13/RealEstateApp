using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Interfaces;

namespace RealEstateApp.Application.Features.Improvements.Commands.UpdateImprovement
{
    public class UpdateImprovementCommandHandler
        : IRequestHandler<UpdateImprovementCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public UpdateImprovementCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateImprovementCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Improvements
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"Improvement con Id {request.Id} no existe.");

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
