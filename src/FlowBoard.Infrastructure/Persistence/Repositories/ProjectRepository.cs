using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.Infrastructure.Persistence.Repositories;

public class ProjectRepository(AppDbContext context) : Repository<Project>(context), IProjectRepository
{
    public async Task<IEnumerable<Project>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(p => p.Members)
            .Include(p => p.Boards)
            .Where(p => p.Members.Any(m => m.UserId == userId))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<Project?> GetWithBoardsAsync(Guid projectId, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(p => p.Members)
            .Include(p => p.Boards)
            .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

    public async Task<bool> IsUserMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default) =>
        await context.ProjectMembers
            .AnyAsync(m => m.ProjectId == projectId && m.UserId == userId, cancellationToken);
}
