using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;

namespace RealEstateApp.Infrastructure.Persistence.Configurations
{
    public class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(p => p.SizeInSquareMeters)
                .HasColumnType("float")
                .IsRequired();

            builder.Property(p => p.Bedrooms)
                .IsRequired();

            builder.Property(p => p.Bathrooms)
                .IsRequired();

            builder.Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(6);

            builder.Property(p => p.AgentId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(PropertyStatus.Disponible);

            builder.Property(p => p.IsActive)
                .HasDefaultValue(true);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(p => p.PropertyType)
                .WithMany()
                .HasForeignKey(p => p.PropertyTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.SaleType)
                .WithMany()
                .HasForeignKey(p => p.SaleTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-many relationship with improvements
            builder.HasMany(p => p.Improvements)
                .WithMany(i => i.Properties)
                .UsingEntity<PropertyImprovement>(
                    j => j.HasOne(pi => pi.Improvement)
                        .WithMany()
                        .HasForeignKey(pi => pi.ImprovementId),
                    j => j.HasOne(pi => pi.Property)
                        .WithMany()
                        .HasForeignKey(pi => pi.PropertyId),
                    j => j.HasKey(pi => new { pi.PropertyId, pi.ImprovementId }));

            builder.HasMany(p => p.Images)
                .WithOne(i => i.Property)
                .HasForeignKey(i => i.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.FavoriteProperties)
                .WithOne(fp => fp.Property)
                .HasForeignKey(fp => fp.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Offers)
                .WithOne(o => o.Property)
                .HasForeignKey(o => o.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(p => p.Code).IsUnique();
            builder.HasIndex(p => p.AgentId);
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.PropertyTypeId);
            builder.HasIndex(p => p.SaleTypeId);
        }
    }
}
