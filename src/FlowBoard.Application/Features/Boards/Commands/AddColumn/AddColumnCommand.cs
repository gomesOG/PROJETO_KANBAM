using FlowBoard.Application.DTOs;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.AddColumn;

public record AddColumnCommand(
    Guid BoardId,
    string Name,
    string? Color,
    int? CardLimit
) : IRequest<ColumnDto>;
