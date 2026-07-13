using FlowBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowBoard.Infrastructure.Persistence.Configurations;

public class ColumnConfiguration : IEntityTypeConfiguration<Column>
{
    public void Configure(EntityTypeBuilder<Column> builder)
    {
        builder.ToTable("Columns");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(c => c.Color)
            .HasMaxLength(7);

        builder.Property(c => c.BoardId).IsRequired();
        builder.Property(c => c.Position).IsRequired();
        builder.Property(c => c.CardLimit);
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt);

        builder.HasMany(c => c.Cards)
            .WithOne()
            .HasForeignKey(card => card.ColumnId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.BoardId, c.Position });
    }
}
