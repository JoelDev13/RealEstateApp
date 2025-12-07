using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Interfaces.Repositories
{
    public interface IImprovementRepository : IRepository<Improvement>
    {
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<bool> HasPropertiesAsync(int improvementId);
        Task<int> GetPropertiesCountAsync(int improvementId);
        Task<List<Improvement>> GetByIdsAsync(List<string> ids);
    }
}
