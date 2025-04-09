using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RapidPay.Domain.Models;

namespace RapidPay.Infrastructure.Persistence.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.Property(c => c.CardNumber).HasMaxLength(15).IsRequired();
        builder.Property(c => c.Balance).HasColumnType("decimal(18,2)").HasDefaultValue(0).IsRequired();

        // Index for CardNumber to ensure uniqueness
        builder.HasIndex(c => c.CardNumber).IsUnique();
    }
}