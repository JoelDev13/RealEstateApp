using MediatR;
using RealEstateApp.Application.Interfaces;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Features.PropertyTypes.Commands.CreatePropertyType
{
    public class CreatePropertyTypeCommandHandler
        : IRequestHandler<CreatePropertyTypeCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreatePropertyTypeCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreatePropertyTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = new PropertyType
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.PropertyTypes.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
