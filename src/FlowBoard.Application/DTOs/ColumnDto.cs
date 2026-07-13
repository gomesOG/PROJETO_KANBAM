namespace FlowBoard.Application.DTOs;

public record ColumnDto(
    Guid Id,
    string Name,
    string? Color,
    Guid BoardId,
    int Position,
    int? CardLimit,
    IReadOnlyList<CardSummaryDto> Cards
);
