using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Interfaces;

namespace RealEstateApp.Application.Features.PropertyTypes.Commands.TogglePropertyTypeStatus
{
    public class TogglePropertyTypeStatusCommandHandler
        : IRequestHandler<TogglePropertyTypeStatusCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public TogglePropertyTypeStatusCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Unit> Handle(TogglePropertyTypeStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.PropertyTypes
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"PropertyType con Id {request.Id} no existe.");

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

    }
}
