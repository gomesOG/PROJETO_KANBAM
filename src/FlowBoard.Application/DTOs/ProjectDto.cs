namespace FlowBoard.Application.DTOs;

public record ProjectDto(
    Guid Id,
    string Name,
    string? Description,
    string? Color,
    bool IsArchived,
    Guid OwnerId,
    int MemberCount,
    int BoardCount,
    DateTime CreatedAt
);

public record ProjectMemberDto(
    Guid UserId,
    string UserName,
    string? AvatarUrl,
    string Role
);
