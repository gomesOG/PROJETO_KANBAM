using FlowBoard.Domain.Entities;

namespace FlowBoard.Domain.Interfaces.Repositories;

public interface IBoardRepository : IRepository<Board>
{
    Task<Board?> GetWithColumnsAsync(Guid boardId, CancellationToken cancellationToken = default);
    Task<Board?> GetWithColumnsAndCardsAsync(Guid boardId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Board>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
}
