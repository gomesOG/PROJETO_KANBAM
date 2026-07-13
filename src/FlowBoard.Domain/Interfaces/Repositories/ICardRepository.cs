using FlowBoard.Domain.Entities;

namespace FlowBoard.Domain.Interfaces.Repositories;

public interface ICardRepository : IRepository<Card>
{
    Task<Card?> GetWithDetailsAsync(Guid cardId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Card>> GetByColumnIdAsync(Guid columnId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Card>> GetByAssigneeAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Card>> GetOverdueAsync(Guid boardId, CancellationToken cancellationToken = default);
}
