using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;

namespace RealEstateApp.Infraestructure.Persistence.Configurations
{
    public class OfferConfiguration : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            builder.ToTable("Offers");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.PropertyId)
                .IsRequired();

            builder.Property(o => o.ClientId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(o => o.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.OfferDate)
                .IsRequired();

            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<int>();

            #region Relationship
            builder.HasOne(o => o.Property)
                .WithMany(p => p.Offers)
                .HasForeignKey(o => o.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Indexes
            builder.HasIndex(o => o.PropertyId);
            builder.HasIndex(o => o.ClientId);
            builder.HasIndex(o => o.Status);
            builder.HasIndex(o => new { o.PropertyId, o.Status });
            #endregion
        }
    }
}
