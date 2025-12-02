using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PropertyType> PropertyTypes => Set<PropertyType>();
        public DbSet<SaleType> SaleTypes => Set<SaleType>();
        public DbSet<Improvement> Improvements => Set<Improvement>();
        public DbSet<Property> Properties => Set<Property>();
        public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly);
        }
    }
}
