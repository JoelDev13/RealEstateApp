using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Domain.Entities;

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

            builder.HasOne(p => p.PropertyType)
                .WithMany()
                .HasForeignKey(p => p.PropertyTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.SaleType)
                .WithMany()
                .HasForeignKey(p => p.SaleTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(p => p.Improvements)
                .WithMany();

            builder
                .HasMany(p => p.Images)
                .WithOne(i => i.Property)
                .HasForeignKey(i => i.PropertyId);

            builder.Property(p => p.IsActive)
                .HasDefaultValue(true);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
