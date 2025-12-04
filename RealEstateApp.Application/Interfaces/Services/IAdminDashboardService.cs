using RealEstateApp.Application.Dtos.Dashboard;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardDto> GetDashboardAsync();
    }
}
