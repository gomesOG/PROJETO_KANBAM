using FlowBoard.Application.Common.Exceptions;
using FlowBoard.Application.DTOs;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Interfaces;
using FlowBoard.Domain.Interfaces.Repositories;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.CreateBoard;

public class CreateBoardCommandHandler(
    IBoardRepository boardRepository,
    IProjectRepository projectRepository,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateBoardCommand, BoardDto>
{
    public async Task<BoardDto> Handle(CreateBoardCommand request, CancellationToken cancellationToken)
    {
        var isMember = await projectRepository.IsUserMemberAsync(request.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember) throw new ForbiddenException();

        var board = Board.Create(request.Name, request.ProjectId, request.Description);

        await boardRepository.AddAsync(board, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new BoardDto(board.Id, board.Name, board.Description, board.ProjectId, board.IsArchived, board.Position, board.CreatedAt);
    }
}
