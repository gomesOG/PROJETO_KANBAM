using FlowBoard.Application.DTOs;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.CreateBoard;

public record CreateBoardCommand(
    string Name,
    Guid ProjectId,
    string? Description
) : IRequest<BoardDto>;
