using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Interfaces;

namespace RealEstateApp.Application.Features.SaleTypes.Commands.UpdateSaleType
{
    public class UpdateSaleTypeCommandHandler
        : IRequestHandler<UpdateSaleTypeCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public UpdateSaleTypeCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateSaleTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.SaleTypes
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"SaleType con Id {request.Id} no existe.");

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
