using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Interfaces;

namespace RealEstateApp.Application.Features.Improvements.Commands.DeleteImprovement
{
    public class DeleteImprovementCommandHandler
        : IRequestHandler<DeleteImprovementCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public DeleteImprovementCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteImprovementCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Improvements
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"Improvement con Id {request.Id} no existe.");

            _context.Improvements.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
