using FlowBoard.Application.DTOs;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Queries.GetBoardById;

public record GetBoardByIdQuery(Guid BoardId) : IRequest<BoardDetailDto>;
