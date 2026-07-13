using FlowBoard.Domain.Common;
using FlowBoard.Domain.Enums;

namespace FlowBoard.Domain.Entities;

public class ProjectMember : Entity
{
    public Guid ProjectId { get; private init; }
    public Guid UserId { get; private init; }
    public ProjectRole Role { get; private set; }

    private ProjectMember() { }

    internal static ProjectMember Create(Guid projectId, Guid userId, ProjectRole role) =>
        new() { ProjectId = projectId, UserId = userId, Role = role };

    public void ChangeRole(ProjectRole role)
    {
        Role = role;
        UpdatedAt = DateTime.UtcNow;
    }
}
