using Lastlink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lastlink.Infrastructure.Persistence.Configurations;

public sealed class ProductEventConfiguration : IEntityTypeConfiguration<ProductEvent>
{
    public void Configure(EntityTypeBuilder<ProductEvent> builder)
    {
        builder.ToTable("ProductEvents");

        builder.HasKey(productEvent => productEvent.Id);

        builder.Property(productEvent => productEvent.EventType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(productEvent => productEvent.Payload)
            .IsRequired();

        builder.Property(productEvent => productEvent.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("NOW()")
            .IsRequired();

        builder
            .HasIndex(productEvent => productEvent.ProductId);
    }
}
