using FlowBoard.Domain.Common;

namespace FlowBoard.Domain.Entities;

public class Board : AggregateRoot
{
    private readonly List<Column> _columns = [];

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public Guid ProjectId { get; private init; }
    public bool IsArchived { get; private set; }
    public int Position { get; private set; }

    public IReadOnlyCollection<Column> Columns => _columns.AsReadOnly();

    private Board() { Name = null!; }

    public static Board Create(string name, Guid projectId, string? description = null, int position = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome do board não pode ser vazio.", nameof(name));

        return new Board
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            ProjectId = projectId,
            Position = position,
            IsArchived = false
        };
    }

    public Column AddColumn(string name, string? color = null)
    {
        var position = _columns.Count > 0 ? _columns.Max(c => c.Position) + 1 : 0;
        var column = Column.Create(name, Id, position, color);
        _columns.Add(column);
        UpdatedAt = DateTime.UtcNow;
        return column;
    }

    public void ReorderColumns(IEnumerable<Guid> orderedColumnIds)
    {
        var ids = orderedColumnIds.ToList();
        foreach (var (id, index) in ids.Select((id, i) => (id, i)))
        {
            var column = _columns.FirstOrDefault(c => c.Id == id)
                ?? throw new InvalidOperationException($"Coluna {id} não pertence a este board.");
            column.SetPosition(index);
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome do board não pode ser vazio.", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        IsArchived = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
