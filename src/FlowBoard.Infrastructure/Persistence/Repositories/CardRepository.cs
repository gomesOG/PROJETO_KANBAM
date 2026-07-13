using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.Infrastructure.Persistence.Repositories;

public class CardRepository(AppDbContext context) : Repository<Card>(context), ICardRepository
{
    public async Task<Card?> GetWithDetailsAsync(Guid cardId, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(c => c.Tags)
            .Include(c => c.Assignees)
            .Include(c => c.ChecklistItems)
            .FirstOrDefaultAsync(c => c.Id == cardId, cancellationToken);

    public async Task<IEnumerable<Card>> GetByColumnIdAsync(Guid columnId, CancellationToken cancellationToken = default) =>
        await DbSet
            .Where(c => c.ColumnId == columnId && !c.IsArchived)
            .OrderBy(c => c.Position)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Card>> GetByAssigneeAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(c => c.Assignees)
            .Include(c => c.Tags)
            .Where(c => c.Assignees.Any(a => a.UserId == userId) && !c.IsArchived)
            .OrderBy(c => c.DueDate)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Card>> GetOverdueAsync(Guid boardId, CancellationToken cancellationToken = default) =>
        await DbSet
            .Where(c => c.BoardId == boardId
                     && !c.IsArchived
                     && c.DueDate.HasValue
                     && c.DueDate.Value < DateTime.UtcNow)
            .OrderBy(c => c.DueDate)
            .ToListAsync(cancellationToken);
}
