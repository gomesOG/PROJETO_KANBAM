using FlowBoard.Domain.Common;

namespace FlowBoard.Domain.Entities;

public class Column : Entity
{
    private readonly List<Card> _cards = [];

    public string Name { get; private set; }
    public string? Color { get; private set; }
    public Guid BoardId { get; private init; }
    public int Position { get; private set; }
    public int? CardLimit { get; private set; }

    public IReadOnlyCollection<Card> Cards => _cards.AsReadOnly();

    private Column() { }

    internal static Column Create(string name, Guid boardId, int position, string? color = null) =>
        new()
        {
            Name = name.Trim(),
            BoardId = boardId,
            Position = position,
            Color = color
        };

    public Card AddCard(string title, Guid createdByUserId, string? description = null)
    {
        if (CardLimit.HasValue && _cards.Count(c => !c.IsArchived) >= CardLimit.Value)
            throw new InvalidOperationException($"Limite de {CardLimit} cards atingido nesta coluna.");

        var position = _cards.Count > 0 ? _cards.Max(c => c.Position) + 1 : 0;
        var card = Card.Create(title, Id, BoardId, createdByUserId, description, position);
        _cards.Add(card);
        UpdatedAt = DateTime.UtcNow;
        return card;
    }

    public void Update(string name, string? color, int? cardLimit)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome da coluna não pode ser vazio.", nameof(name));

        Name = name.Trim();
        Color = color;
        CardLimit = cardLimit;
        UpdatedAt = DateTime.UtcNow;
    }

    internal void SetPosition(int position)
    {
        Position = position;
        UpdatedAt = DateTime.UtcNow;
    }
}
