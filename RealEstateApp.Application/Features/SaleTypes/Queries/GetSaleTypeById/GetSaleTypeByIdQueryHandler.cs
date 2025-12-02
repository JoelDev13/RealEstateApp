using MediatR;
using RealEstateApp.Application.Dtos.SaleTypes;
using RealEstateApp.Application.Interfaces.Repositories;

namespace RealEstateApp.Application.Features.SaleTypes.Queries.GetSaleTypeById
{
    public class GetSaleTypeByIdQueryHandler
        : IRequestHandler<GetSaleTypeByIdQuery, SaleTypeDto>
    {
        private readonly ISaleTypeRepository _saleTypeRepository;

        public GetSaleTypeByIdQueryHandler(ISaleTypeRepository saleTypeRepository)
        {
            _saleTypeRepository = saleTypeRepository;
        }

        public async Task<SaleTypeDto> Handle(GetSaleTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _saleTypeRepository.GetByIdAsync(request.Id);

            if (entity == null)
                throw new KeyNotFoundException($"SaleType con Id {request.Id} no existe.");

            return new SaleTypeDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive
            };
        }
    }
}
