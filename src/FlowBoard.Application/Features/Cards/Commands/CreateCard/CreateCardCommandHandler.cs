using FlowBoard.Application.Common.Exceptions;
using FlowBoard.Application.DTOs;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Interfaces;
using FlowBoard.Domain.Interfaces.Repositories;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.CreateCard;

public class CreateCardCommandHandler(
    IBoardRepository boardRepository,
    IProjectRepository projectRepository,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateCardCommand, CardSummaryDto>
{
    public async Task<CardSummaryDto> Handle(CreateCardCommand request, CancellationToken cancellationToken)
    {
        var board = await boardRepository.GetWithColumnsAsync(request.ColumnId, cancellationToken)
            ?? throw new NotFoundException("Coluna não encontrada.");

        var isMember = await projectRepository.IsUserMemberAsync(board.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember) throw new ForbiddenException();

        var column = board.Columns.FirstOrDefault(c => c.Id == request.ColumnId)
            ?? throw new NotFoundException("Coluna não encontrada neste board.");

        var card = column.AddCard(request.Title, currentUser.UserId, request.Description);

        if (request.DueDate.HasValue || request.Priority != Domain.Enums.Priority.Medium)
            card.Update(card.Title, card.Description, request.Priority, request.DueDate);

        boardRepository.Update(board);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CardSummaryDto(
            card.Id, card.Title, card.ColumnId, card.Position,
            card.Priority.ToString(), card.DueDate, false, 0, 0, [], []);
    }
}
