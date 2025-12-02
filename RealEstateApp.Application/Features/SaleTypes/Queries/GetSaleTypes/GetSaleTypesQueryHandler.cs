using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Dtos.SaleTypes;
using RealEstateApp.Application.Interfaces.Repositories;

namespace RealEstateApp.Application.Features.SaleTypes.Queries.GetSaleTypes
{
    public class GetSaleTypesQueryHandler
        : IRequestHandler<GetSaleTypesQuery, List<SaleTypeDto>>
    {
        private readonly ISaleTypeRepository _saleTypeRepository;

        public GetSaleTypesQueryHandler(ISaleTypeRepository saleTypeRepository)
        {
            _saleTypeRepository = saleTypeRepository;
        }

        public async Task<List<SaleTypeDto>> Handle(GetSaleTypesQuery request, CancellationToken cancellationToken)
        {
            var list = await _saleTypeRepository
                .Query()
                .OrderBy(st => st.Name)
                .ToListAsync(cancellationToken);

            return list
                .Select(st => new SaleTypeDto
                {
                    Id = st.Id,
                    Name = st.Name,
                    Description = st.Description,
                    IsActive = st.IsActive
                })
                .ToList();
        }
    }
}
