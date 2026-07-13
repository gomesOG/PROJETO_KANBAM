using FlowBoard.Domain.Common;

namespace FlowBoard.Domain.Entities;

public class CardAssignee : Entity
{
    public Guid CardId { get; private init; }
    public Guid UserId { get; private init; }

    private CardAssignee() { }

    internal static CardAssignee Create(Guid cardId, Guid userId) =>
        new() { CardId = cardId, UserId = userId };
}
