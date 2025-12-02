using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Interfaces;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Infraestructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PropertyType> PropertyTypes => Set<PropertyType>();
        public DbSet<SaleType> SaleTypes => Set<SaleType>();
        public DbSet<Improvement> Improvements => Set<Improvement>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
