using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Infraestructure.Persistence.Configurations
{
    public class PropertyTypeConfiguration : IEntityTypeConfiguration<PropertyType>
    {
        public void Configure(EntityTypeBuilder<PropertyType> builder)
        {
            builder.ToTable("PropertyTypes");

            builder.HasKey(pt => pt.Id);

            builder.Property(pt => pt.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(pt => pt.Description)
                   .HasMaxLength(500);

            builder.Property(pt => pt.IsActive)
                   .HasDefaultValue(true);

            builder.Property(pt => pt.CreatedAt)
                   .IsRequired();

            builder.Property(pt => pt.UpdatedAt)
                   .IsRequired(false);
        }
    }
}
