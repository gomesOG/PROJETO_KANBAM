using FlowBoard.Application.Common.Exceptions;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Interfaces;
using FlowBoard.Domain.Interfaces.Repositories;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.ReorderColumns;

public class ReorderColumnsCommandHandler(
    IBoardRepository boardRepository,
    IProjectRepository projectRepository,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork
) : IRequestHandler<ReorderColumnsCommand>
{
    public async Task Handle(ReorderColumnsCommand request, CancellationToken cancellationToken)
    {
        var board = await boardRepository.GetWithColumnsAsync(request.BoardId, cancellationToken)
            ?? throw new NotFoundException(nameof(Board), request.BoardId);

        var isMember = await projectRepository.IsUserMemberAsync(board.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember) throw new ForbiddenException();

        board.ReorderColumns(request.OrderedColumnIds);
        boardRepository.Update(board);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
