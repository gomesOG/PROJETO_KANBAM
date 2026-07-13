using FlowBoard.Application.Common.Exceptions;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Interfaces;
using FlowBoard.Domain.Interfaces.Repositories;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.MoveCard;

public class MoveCardCommandHandler(
    ICardRepository cardRepository,
    IBoardRepository boardRepository,
    IProjectRepository projectRepository,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork
) : IRequestHandler<MoveCardCommand>
{
    public async Task Handle(MoveCardCommand request, CancellationToken cancellationToken)
    {
        var card = await cardRepository.GetByIdAsync(request.CardId, cancellationToken)
            ?? throw new NotFoundException(nameof(Card), request.CardId);

        var board = await boardRepository.GetWithColumnsAndCardsAsync(card.BoardId, cancellationToken)
            ?? throw new NotFoundException(nameof(Board), card.BoardId);

        var isMember = await projectRepository.IsUserMemberAsync(board.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember) throw new ForbiddenException();

        var targetColumn = board.Columns.FirstOrDefault(c => c.Id == request.TargetColumnId)
            ?? throw new NotFoundException("Coluna de destino não encontrada neste board.");

        // Reordena os cards na coluna de destino para abrir espaço
        var cardsInTarget = targetColumn.Cards
            .Where(c => c.Id != card.Id && !c.IsArchived)
            .OrderBy(c => c.Position)
            .ToList();

        card.MoveToColumn(request.TargetColumnId, request.NewPosition);

        for (var i = 0; i < cardsInTarget.Count; i++)
        {
            var adjustedPosition = i >= request.NewPosition ? i + 1 : i;
            cardsInTarget[i].SetPosition(adjustedPosition);
        }

        cardRepository.Update(card);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
