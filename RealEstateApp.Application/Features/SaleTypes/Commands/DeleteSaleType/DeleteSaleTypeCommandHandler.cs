using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Interfaces;

namespace RealEstateApp.Application.Features.SaleTypes.Commands.DeleteSaleType
{
    public class DeleteSaleTypeCommandHandler
        : IRequestHandler<DeleteSaleTypeCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public DeleteSaleTypeCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteSaleTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.SaleTypes
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"SaleType con Id {request.Id} no existe.");

            _context.SaleTypes.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
