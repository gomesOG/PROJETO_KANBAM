using FlowBoard.Application.Common.Exceptions;
using FlowBoard.Domain.Entities;
using FlowBoard.Application.DTOs;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Interfaces;
using FlowBoard.Domain.Interfaces.Repositories;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.AddColumn;

public class AddColumnCommandHandler(
    IBoardRepository boardRepository,
    IProjectRepository projectRepository,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork
) : IRequestHandler<AddColumnCommand, ColumnDto>
{
    public async Task<ColumnDto> Handle(AddColumnCommand request, CancellationToken cancellationToken)
    {
        var board = await boardRepository.GetWithColumnsAsync(request.BoardId, cancellationToken)
            ?? throw new NotFoundException(nameof(Board), request.BoardId);

        var isMember = await projectRepository.IsUserMemberAsync(board.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember) throw new ForbiddenException();

        var column = board.AddColumn(request.Name, request.Color);

        if (request.CardLimit.HasValue)
            column.Update(column.Name, column.Color, request.CardLimit);

        boardRepository.Update(board);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ColumnDto(column.Id, column.Name, column.Color, column.BoardId, column.Position, column.CardLimit, []);
    }
}
