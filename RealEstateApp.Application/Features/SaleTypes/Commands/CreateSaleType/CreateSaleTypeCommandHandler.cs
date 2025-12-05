using MediatR;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Features.SaleTypes.Commands.CreateSaleType
{
    public class CreateSaleTypeCommandHandler
        : IRequestHandler<CreateSaleTypeCommand, int>
    {
        private readonly ISaleTypeRepository _saleTypeRepository;

        public CreateSaleTypeCommandHandler(ISaleTypeRepository saleTypeRepository)
        {
            _saleTypeRepository = saleTypeRepository;
        }

        public async Task<int> Handle(
            CreateSaleTypeCommand request,
            CancellationToken cancellationToken)
        {
            var entity = new SaleType
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _saleTypeRepository.AddAsync(entity);

            return entity.Id;
        }
    }
}
