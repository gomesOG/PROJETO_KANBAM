using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.Infrastructure.Persistence.Repositories;

public class BoardRepository(AppDbContext context) : Repository<Board>(context), IBoardRepository
{
    public async Task<Board?> GetWithColumnsAsync(Guid boardId, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(b => b.Columns)
            .FirstOrDefaultAsync(b => b.Id == boardId, cancellationToken);

    public async Task<Board?> GetWithColumnsAndCardsAsync(Guid boardId, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(b => b.Columns)
                .ThenInclude(c => c.Cards)
                    .ThenInclude(card => card.Tags)
            .Include(b => b.Columns)
                .ThenInclude(c => c.Cards)
                    .ThenInclude(card => card.Assignees)
            .Include(b => b.Columns)
                .ThenInclude(c => c.Cards)
                    .ThenInclude(card => card.ChecklistItems)
            .FirstOrDefaultAsync(b => b.Id == boardId, cancellationToken);

    public async Task<IEnumerable<Board>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default) =>
        await DbSet
            .Where(b => b.ProjectId == projectId && !b.IsArchived)
            .OrderBy(b => b.Position)
            .ToListAsync(cancellationToken);
}
