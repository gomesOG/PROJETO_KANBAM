using FlowBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowBoard.Infrastructure.Persistence.Configurations;

public class CardAssigneeConfiguration : IEntityTypeConfiguration<CardAssignee>
{
    public void Configure(EntityTypeBuilder<CardAssignee> builder)
    {
        builder.ToTable("CardAssignees");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.CardId).IsRequired();
        builder.Property(a => a.UserId).IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.UpdatedAt);

        builder.HasIndex(a => new { a.CardId, a.UserId }).IsUnique();
        builder.HasIndex(a => a.UserId);
    }
}
