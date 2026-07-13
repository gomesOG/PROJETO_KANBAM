using FlowBoard.Domain.Common;
using FlowBoard.Domain.Enums;

namespace FlowBoard.Domain.Entities;

public class Project : AggregateRoot
{
    private readonly List<ProjectMember> _members = [];
    private readonly List<Board> _boards = [];

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? Color { get; private set; }
    public bool IsArchived { get; private set; }
    public Guid OwnerId { get; private init; }

    public IReadOnlyCollection<ProjectMember> Members => _members.AsReadOnly();
    public IReadOnlyCollection<Board> Boards => _boards.AsReadOnly();

    private Project() { Name = null!; }

    public static Project Create(string name, Guid ownerId, string? description = null, string? color = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome do projeto não pode ser vazio.", nameof(name));

        var project = new Project
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            Color = color,
            OwnerId = ownerId,
            IsArchived = false
        };

        project._members.Add(ProjectMember.Create(project.Id, ownerId, ProjectRole.Owner));
        return project;
    }

    public void Update(string name, string? description, string? color)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome do projeto não pode ser vazio.", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        Color = color;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddMember(Guid userId, ProjectRole role)
    {
        if (_members.Any(m => m.UserId == userId))
            throw new InvalidOperationException("Usuário já é membro do projeto.");

        _members.Add(ProjectMember.Create(Id, userId, role));
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveMember(Guid userId)
    {
        if (userId == OwnerId)
            throw new InvalidOperationException("Não é possível remover o proprietário do projeto.");

        var member = _members.FirstOrDefault(m => m.UserId == userId)
            ?? throw new InvalidOperationException("Usuário não é membro do projeto.");

        _members.Remove(member);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        IsArchived = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
