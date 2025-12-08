using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Infraestructure.Persistence.Configurations
{
    public class FavoritePropertyConfiguration : IEntityTypeConfiguration<FavoriteProperty>
    {
        public void Configure(EntityTypeBuilder<FavoriteProperty> builder)
        {
            builder.ToTable("FavoriteProperties");

            builder.HasKey(fp => fp.Id);

            builder.Property(fp => fp.ClientId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(fp => fp.PropertyId)
                .IsRequired();

            builder.Property(fp => fp.AddedDate)
                .IsRequired();

            #region Relationship
            builder.HasOne(fp => fp.Property)
                .WithMany(p => p.FavoriteProperties)
                .HasForeignKey(fp => fp.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Unique constraint
            builder.HasIndex(fp => new { fp.ClientId, fp.PropertyId })
                .IsUnique();
            #endregion
        }
    }
}
