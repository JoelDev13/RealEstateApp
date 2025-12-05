using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Interfaces.Repositories
{
    public interface ISaleTypeRepository : IRepository<SaleType>
    {
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<bool> HasPropertiesAsync(int saleTypeId);
        Task<int> GetPropertiesCountAsync(int saleTypeId);
    }
}
