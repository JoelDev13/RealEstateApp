using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Infraestructure.Persistence.Configurations
{
    public class PropertyImprovementConfiguration : IEntityTypeConfiguration<PropertyImprovement>
    {
        public void Configure(EntityTypeBuilder<PropertyImprovement> builder)
        {
            builder.ToTable("PropertyImprovements");

            // Llave compuesta
            builder.HasKey(pi => new { pi.PropertyId, pi.ImprovementId });

            builder.Property(pi => pi.PropertyId)
                .IsRequired();

            builder.Property(pi => pi.ImprovementId)
                .IsRequired();

            // Relaciones
            builder.HasOne(pi => pi.Property)
                .WithMany()
                .HasForeignKey(pi => pi.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pi => pi.Improvement)
                .WithMany()
                .HasForeignKey(pi => pi.ImprovementId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
