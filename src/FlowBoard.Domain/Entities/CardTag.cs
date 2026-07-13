using FlowBoard.Domain.Common;

namespace FlowBoard.Domain.Entities;

public class CardTag : Entity
{
    public Guid CardId { get; private init; }
    public string Name { get; private set; }
    public string Color { get; private set; }

    private CardTag() { Name = null!; Color = null!; }

    internal static CardTag Create(Guid cardId, string name, string color) =>
        new() { CardId = cardId, Name = name.Trim(), Color = color };
}
