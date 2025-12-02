using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Interfaces;

namespace RealEstateApp.Application.Features.SaleTypes.Commands.ToggleSaleTypeStatus
{
    public class ToggleSaleTypeStatusCommandHandler
        : IRequestHandler<ToggleSaleTypeStatusCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public ToggleSaleTypeStatusCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ToggleSaleTypeStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.SaleTypes
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"SaleType con Id {request.Id} no existe.");

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
