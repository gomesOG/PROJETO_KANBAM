using FlowBoard.Domain.Common;
using FlowBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FlowBoard.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<Column> Columns => Set<Column>();
    public DbSet<Card> Cards => Set<Card>();
    public DbSet<CardTag> CardTags => Set<CardTag>();
    public DbSet<CardAssignee> CardAssignees => Set<CardAssignee>();
    public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        DispatchDomainEvents();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void DispatchDomainEvents()
    {
        var aggregates = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        foreach (var aggregate in aggregates)
            aggregate.ClearDomainEvents();
    }
}
