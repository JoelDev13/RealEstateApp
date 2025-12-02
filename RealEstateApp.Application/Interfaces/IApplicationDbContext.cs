using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<PropertyType> PropertyTypes { get; }
        DbSet<SaleType> SaleTypes { get; }
        DbSet<Improvement> Improvements { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
