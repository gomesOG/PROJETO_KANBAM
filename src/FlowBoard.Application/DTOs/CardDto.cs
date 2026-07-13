namespace FlowBoard.Application.DTOs;

public record CardSummaryDto(
    Guid Id,
    string Title,
    Guid ColumnId,
    int Position,
    string Priority,
    DateTime? DueDate,
    bool IsOverdue,
    int ChecklistTotal,
    int ChecklistCompleted,
    IReadOnlyList<CardTagDto> Tags,
    IReadOnlyList<CardAssigneeDto> Assignees
);

public record CardDetailDto(
    Guid Id,
    string Title,
    string? Description,
    Guid ColumnId,
    Guid BoardId,
    int Position,
    string Priority,
    string Status,
    DateTime? DueDate,
    bool IsOverdue,
    bool IsArchived,
    Guid CreatedByUserId,
    IReadOnlyList<CardTagDto> Tags,
    IReadOnlyList<CardAssigneeDto> Assignees,
    IReadOnlyList<ChecklistItemDto> ChecklistItems,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CardTagDto(Guid Id, string Name, string Color);

public record CardAssigneeDto(Guid UserId, string UserName, string? AvatarUrl);

public record ChecklistItemDto(Guid Id, string Text, bool IsCompleted, int Position);
