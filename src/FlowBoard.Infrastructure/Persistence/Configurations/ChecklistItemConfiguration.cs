using FlowBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowBoard.Infrastructure.Persistence.Configurations;

public class ChecklistItemConfiguration : IEntityTypeConfiguration<ChecklistItem>
{
    public void Configure(EntityTypeBuilder<ChecklistItem> builder)
    {
        builder.ToTable("ChecklistItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.CardId).IsRequired();

        builder.Property(i => i.Text)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(i => i.IsCompleted).IsRequired().HasDefaultValue(false);
        builder.Property(i => i.Position).IsRequired();
        builder.Property(i => i.CompletedAt);
        builder.Property(i => i.CreatedAt).IsRequired();
        builder.Property(i => i.UpdatedAt);

        builder.HasIndex(i => i.CardId);
    }
}
