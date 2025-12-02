using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Interfaces;

namespace RealEstateApp.Application.Features.PropertyTypes.Commands.DeletePropertyType
{
    public class DeletePropertyTypeCommandHandler
        : IRequestHandler<DeletePropertyTypeCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public DeletePropertyTypeCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeletePropertyTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.PropertyTypes
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"PropertyType con Id {request.Id} no existe.");

            _context.PropertyTypes.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
