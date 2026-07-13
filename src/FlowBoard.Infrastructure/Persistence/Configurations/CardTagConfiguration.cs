using FlowBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowBoard.Infrastructure.Persistence.Configurations;

public class CardTagConfiguration : IEntityTypeConfiguration<CardTag>
{
    public void Configure(EntityTypeBuilder<CardTag> builder)
    {
        builder.ToTable("CardTags");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.CardId).IsRequired();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Color)
            .IsRequired()
            .HasMaxLength(7);

        builder.Property(t => t.CreatedAt).IsRequired();
        builder.Property(t => t.UpdatedAt);
    }
}
