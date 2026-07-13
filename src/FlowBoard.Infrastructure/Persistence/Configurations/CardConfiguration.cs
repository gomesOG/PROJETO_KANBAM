using FlowBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowBoard.Infrastructure.Persistence.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.ToTable("Cards");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(2000);

        builder.Property(c => c.ColumnId).IsRequired();
        builder.Property(c => c.BoardId).IsRequired();
        builder.Property(c => c.CreatedByUserId).IsRequired();
        builder.Property(c => c.Position).IsRequired();

        builder.Property(c => c.Priority)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.DueDate);
        builder.Property(c => c.IsArchived).IsRequired().HasDefaultValue(false);
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt);

        builder.HasMany(c => c.Tags)
            .WithOne()
            .HasForeignKey(t => t.CardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Assignees)
            .WithOne()
            .HasForeignKey(a => a.CardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.ChecklistItems)
            .WithOne()
            .HasForeignKey(i => i.CardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.ColumnId);
        builder.HasIndex(c => c.BoardId);
        builder.HasIndex(c => c.DueDate);
        builder.HasIndex(c => c.IsArchived);
    }
}
