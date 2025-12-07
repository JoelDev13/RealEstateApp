using RealEstateApp.Application.Interfaces.Repositories;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Infraestructure.Persistence.Repositories
{
    public interface IImprovementRepository : IRepository<Improvement>
    {
        Task<List<Improvement>> GetByIdsAsync(List<string> ids);
    }
}
