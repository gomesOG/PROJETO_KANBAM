using FlowBoard.Domain.Common;

namespace FlowBoard.Domain.Entities;

public class ChecklistItem : Entity
{
    public Guid CardId { get; private init; }
    public string Text { get; private set; }
    public bool IsCompleted { get; private set; }
    public int Position { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private ChecklistItem() { }

    internal static ChecklistItem Create(Guid cardId, string text, int position)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Texto do item não pode ser vazio.", nameof(text));

        return new ChecklistItem
        {
            CardId = cardId,
            Text = text.Trim(),
            Position = position,
            IsCompleted = false
        };
    }

    internal void Toggle()
    {
        IsCompleted = !IsCompleted;
        CompletedAt = IsCompleted ? DateTime.UtcNow : null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Texto do item não pode ser vazio.", nameof(text));

        Text = text.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
