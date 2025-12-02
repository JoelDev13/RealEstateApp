using MediatR;
using RealEstateApp.Application.Interfaces;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Features.Improvements.Commands.CreateImprovement
{
    public class CreateImprovementCommandHandler
        : IRequestHandler<CreateImprovementCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateImprovementCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateImprovementCommand request, CancellationToken cancellationToken)
        {
            var entity = new Improvement
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Improvements.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
