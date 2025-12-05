using MediatR;
using RealEstateApp.Application.Interfaces.Repositories;

namespace RealEstateApp.Application.Features.SaleTypes.Commands.ToggleSaleTypeStatus
{
    public class ToggleSaleTypeStatusCommandHandler
        : IRequestHandler<ToggleSaleTypeStatusCommand, Unit>
    {
        private readonly ISaleTypeRepository _saleTypeRepository;

        public ToggleSaleTypeStatusCommandHandler(ISaleTypeRepository saleTypeRepository)
        {
            _saleTypeRepository = saleTypeRepository;
        }

        public async Task<Unit> Handle(ToggleSaleTypeStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _saleTypeRepository.GetByIdAsync(request.Id);

            if (entity == null)
                throw new KeyNotFoundException($"SaleType con Id {request.Id} no existe.");

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _saleTypeRepository.UpdateAsync(entity);

            return Unit.Value;
        }
    }
}
