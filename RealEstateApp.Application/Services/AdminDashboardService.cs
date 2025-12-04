using RealEstateApp.Application.Dtos.Dashboard;
using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Enums;

namespace RealEstateApp.Application.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly ISaleTypeRepository _saleTypeRepository;
        private readonly IPropertyTypeRepository _propertyTypeRepository;
        private readonly IImprovementRepository _improvementRepository;
        private readonly IBaseAccountService _accountService;

        public AdminDashboardService(
            ISaleTypeRepository saleTypeRepository,
            IPropertyTypeRepository propertyTypeRepository,
            IImprovementRepository improvementRepository,
            IBaseAccountService accountService)
        {
            _saleTypeRepository = saleTypeRepository;
            _propertyTypeRepository = propertyTypeRepository;
            _improvementRepository = improvementRepository;
            _accountService = accountService;
        }

        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            var saleTypesQuery = _saleTypeRepository.Query();
            var propertyTypesQuery = _propertyTypeRepository.Query();
            var improvementsQuery = _improvementRepository.Query();

            // Obtener conteos de usuarios por rol
            var totalAgents = await _accountService.CountUsers(Roles.Agente, null);
            var activeAgents = await _accountService.CountUsers(Roles.Agente, true);
            var inactiveAgents = await _accountService.CountUsers(Roles.Agente, false);

            var totalClients = await _accountService.CountUsers(Roles.Cliente, null);
            var activeClients = await _accountService.CountUsers(Roles.Cliente, true);
            var inactiveClients = await _accountService.CountUsers(Roles.Cliente, false);

            var totalDevelopers = await _accountService.CountUsers(Roles.Desarrollador, null);
            var activeDevelopers = await _accountService.CountUsers(Roles.Desarrollador, true);
            var inactiveDevelopers = await _accountService.CountUsers(Roles.Desarrollador, false);

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

                TotalAgents = totalAgents,
                ActiveAgents = activeAgents,
                InactiveAgents = inactiveAgents,

                TotalClients = totalClients,
                ActiveClients = activeClients,
                InactiveClients = inactiveClients,

                TotalDevelopers = totalDevelopers,
                ActiveDevelopers = activeDevelopers,
                InactiveDevelopers = inactiveDevelopers,

                TotalProperties = 0,
                AvailableProperties = 0,
                SoldProperties = 0
            };

            return dto;
        }
    }
}