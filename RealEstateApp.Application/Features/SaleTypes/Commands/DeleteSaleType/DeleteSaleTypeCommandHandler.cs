using MediatR;
using RealEstateApp.Application.Interfaces.Repositories;

namespace RealEstateApp.Application.Features.SaleTypes.Commands.DeleteSaleType
{
    public class DeleteSaleTypeCommandHandler
        : IRequestHandler<DeleteSaleTypeCommand, Unit>
    {
        private readonly ISaleTypeRepository _saleTypeRepository;

        public DeleteSaleTypeCommandHandler(ISaleTypeRepository saleTypeRepository)
        {
            _saleTypeRepository = saleTypeRepository;
        }

        public async Task<Unit> Handle(DeleteSaleTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _saleTypeRepository.GetByIdAsync(request.Id);

            if (entity == null)
                throw new KeyNotFoundException($"SaleType con Id {request.Id} no existe.");

            await _saleTypeRepository.DeleteAsync(entity);
            return Unit.Value;
        }
    }
}
