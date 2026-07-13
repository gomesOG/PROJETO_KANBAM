using FlowBoard.Domain.Common;
using FlowBoard.Domain.Enums;

namespace FlowBoard.Domain.Entities;

public class Card : AggregateRoot
{
    private readonly List<CardTag> _tags = [];
    private readonly List<CardAssignee> _assignees = [];
    private readonly List<ChecklistItem> _checklistItems = [];

    public string Title { get; private set; }
    public string? Description { get; private set; }
    public Guid ColumnId { get; private set; }
    public Guid BoardId { get; private init; }
    public Guid CreatedByUserId { get; private init; }
    public int Position { get; private set; }
    public Priority Priority { get; private set; }
    public CardStatus Status { get; private set; }
    public DateTime? DueDate { get; private set; }
    public bool IsArchived { get; private set; }

    public IReadOnlyCollection<CardTag> Tags => _tags.AsReadOnly();
    public IReadOnlyCollection<CardAssignee> Assignees => _assignees.AsReadOnly();
    public IReadOnlyCollection<ChecklistItem> ChecklistItems => _checklistItems.AsReadOnly();

    private Card() { Title = null!; }

    internal static Card Create(
        string title,
        Guid columnId,
        Guid boardId,
        Guid createdByUserId,
        string? description,
        int position)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Título do card não pode ser vazio.", nameof(title));

        return new Card
        {
            Title = title.Trim(),
            Description = description?.Trim(),
            ColumnId = columnId,
            BoardId = boardId,
            CreatedByUserId = createdByUserId,
            Position = position,
            Priority = Priority.Medium,
            Status = CardStatus.Active,
            IsArchived = false
        };
    }

    public void Update(string title, string? description, Priority priority, DateTime? dueDate)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Título do card não pode ser vazio.", nameof(title));

        Title = title.Trim();
        Description = description?.Trim();
        Priority = priority;
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MoveToColumn(Guid columnId, int position)
    {
        ColumnId = columnId;
        Position = position;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddTag(string name, string color)
    {
        if (_tags.Any(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            return;

        _tags.Add(CardTag.Create(Id, name, color));
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveTag(Guid tagId)
    {
        var tag = _tags.FirstOrDefault(t => t.Id == tagId);
        if (tag is not null)
        {
            _tags.Remove(tag);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void AssignUser(Guid userId)
    {
        if (_assignees.Any(a => a.UserId == userId))
            return;

        _assignees.Add(CardAssignee.Create(Id, userId));
        UpdatedAt = DateTime.UtcNow;
    }

    public void UnassignUser(Guid userId)
    {
        var assignee = _assignees.FirstOrDefault(a => a.UserId == userId);
        if (assignee is not null)
        {
            _assignees.Remove(assignee);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public ChecklistItem AddChecklistItem(string text)
    {
        var item = ChecklistItem.Create(Id, text, _checklistItems.Count);
        _checklistItems.Add(item);
        UpdatedAt = DateTime.UtcNow;
        return item;
    }

    public void ToggleChecklistItem(Guid itemId)
    {
        var item = _checklistItems.FirstOrDefault(i => i.Id == itemId)
            ?? throw new InvalidOperationException("Item de checklist não encontrado.");

        item.Toggle();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        IsArchived = true;
        Status = CardStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPosition(int position)
    {
        Position = position;
        UpdatedAt = DateTime.UtcNow;
    }
}
