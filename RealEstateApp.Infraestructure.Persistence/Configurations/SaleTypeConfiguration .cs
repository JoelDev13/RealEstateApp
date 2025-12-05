using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Infraestructure.Persistence.Configurations
{
    public class SaleTypeConfiguration : IEntityTypeConfiguration<SaleType>
    {
        public void Configure(EntityTypeBuilder<SaleType> builder)
        {
            builder.ToTable("SaleTypes", "RealEstate");

            builder.HasKey(st => st.Id);

            builder.Property(st => st.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(st => st.Description)
                   .HasMaxLength(500);

            builder.Property(st => st.IsActive)
                   .HasDefaultValue(true);
        }
    }
}
