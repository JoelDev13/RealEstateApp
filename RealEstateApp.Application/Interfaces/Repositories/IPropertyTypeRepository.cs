using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Interfaces.Repositories
{
    public interface IPropertyTypeRepository : IRepository<PropertyType>
    {
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<bool> HasPropertiesAsync(int propertyTypeId);
        Task<int> GetPropertiesCountAsync(int propertyTypeId);
    }
}
