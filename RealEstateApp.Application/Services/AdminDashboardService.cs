using RealEstateApp.Application.Dtos.Dashboard;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Application.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly ISaleTypeRepository _saleTypeRepository;
        private readonly IPropertyTypeRepository _propertyTypeRepository;
        private readonly IImprovementRepository _improvementRepository;

        public AdminDashboardService(
            ISaleTypeRepository saleTypeRepository,
            IPropertyTypeRepository propertyTypeRepository,
            IImprovementRepository improvementRepository)
        {
            _saleTypeRepository = saleTypeRepository;
            _propertyTypeRepository = propertyTypeRepository;
            _improvementRepository = improvementRepository;
        }

        public Task<AdminDashboardDto> GetDashboardAsync()
        {
            var saleTypesQuery = _saleTypeRepository.Query();
            var propertyTypesQuery = _propertyTypeRepository.Query();
            var improvementsQuery = _improvementRepository.Query();

            var dto = new AdminDashboardDto
            {
                TotalSaleTypes = saleTypesQuery.Count(),
                ActiveSaleTypes = saleTypesQuery.Count(x => x.IsActive),
                InactiveSaleTypes = saleTypesQuery.Count(x => !x.IsActive),

                TotalPropertyTypes = propertyTypesQuery.Count(),
                ActivePropertyTypes = propertyTypesQuery.Count(x => x.IsActive),
                InactivePropertyTypes = propertyTypesQuery.Count(x => !x.IsActive),

                TotalImprovements = improvementsQuery.Count(),
                ActiveImprovements = improvementsQuery.Count(x => x.IsActive),
                InactiveImprovements = improvementsQuery.Count(x => !x.IsActive),

                //Mientras creo modulos de Properties / Agents
                TotalProperties = 0,
                TotalAgents = 0
            };

            return Task.FromResult(dto);
        }
    }
}
