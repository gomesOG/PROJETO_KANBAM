namespace FlowBoard.Application.DTOs;

public record BoardDto(
    Guid Id,
    string Name,
    string? Description,
    Guid ProjectId,
    bool IsArchived,
    int Position,
    DateTime CreatedAt
);

public record BoardDetailDto(
    Guid Id,
    string Name,
    string? Description,
    Guid ProjectId,
    bool IsArchived,
    IReadOnlyList<ColumnDto> Columns,
    DateTime CreatedAt
);
