using FlowBoard.Domain.Entities;

namespace FlowBoard.Domain.Interfaces.Repositories;

public interface IProjectRepository : IRepository<Project>
{
    Task<IEnumerable<Project>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Project?> GetWithBoardsAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<bool> IsUserMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
}
