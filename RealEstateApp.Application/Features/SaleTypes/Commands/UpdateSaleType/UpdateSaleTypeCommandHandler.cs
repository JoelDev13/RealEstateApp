using MediatR;
using RealEstateApp.Application.Interfaces.Repositories;

namespace RealEstateApp.Application.Features.SaleTypes.Commands.UpdateSaleType
{
    public class UpdateSaleTypeCommandHandler
        : IRequestHandler<UpdateSaleTypeCommand, Unit>
    {
        private readonly ISaleTypeRepository _saleTypeRepository;

        public UpdateSaleTypeCommandHandler(ISaleTypeRepository saleTypeRepository)
        {
            _saleTypeRepository = saleTypeRepository;
        }

        public async Task<Unit> Handle(UpdateSaleTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _saleTypeRepository.GetByIdAsync(request.Id);

            if (entity == null)
                throw new KeyNotFoundException($"SaleType con Id {request.Id} no existe.");

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _saleTypeRepository.UpdateAsync(entity);

            return Unit.Value;
        }
    }
}
