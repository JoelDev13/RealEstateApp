using MediatR;
using RealEstateApp.Application.Interfaces;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Features.SaleTypes.Commands.CreateSaleType
{
    public class CreateSaleTypeCommandHandler
        : IRequestHandler<CreateSaleTypeCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateSaleTypeCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateSaleTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = new SaleType
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.SaleTypes.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
