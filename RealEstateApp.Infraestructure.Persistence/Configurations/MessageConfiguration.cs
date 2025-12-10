using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Content)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(m => m.SentDate)
                .IsRequired();

            builder.Property(m => m.PropertyId)
                .IsRequired();

            builder.Property(m => m.SenderId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(m => m.ReceiverId)
                .IsRequired()
                .HasMaxLength(450);

            // Relationships
            builder.HasOne(m => m.Property)
                .WithMany(p => p.Messages)
                .HasForeignKey(m => m.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            builder.HasIndex(m => new { m.PropertyId, m.SentDate });
            builder.HasIndex(m => new { m.SenderId, m.PropertyId });
            builder.HasIndex(m => new { m.ReceiverId, m.PropertyId });
        }
    }
}
