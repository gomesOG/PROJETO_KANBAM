using FlowBoard.Application.Common.Exceptions;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Interfaces;
using FlowBoard.Domain.Interfaces.Repositories;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.ToggleChecklistItem;

public class ToggleChecklistItemCommandHandler(
    ICardRepository cardRepository,
    IBoardRepository boardRepository,
    IProjectRepository projectRepository,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork
) : IRequestHandler<ToggleChecklistItemCommand>
{
    public async Task Handle(ToggleChecklistItemCommand request, CancellationToken cancellationToken)
    {
        var card = await cardRepository.GetWithDetailsAsync(request.CardId, cancellationToken)
            ?? throw new NotFoundException(nameof(Card), request.CardId);

        var board = await boardRepository.GetByIdAsync(card.BoardId, cancellationToken)
            ?? throw new NotFoundException(nameof(Board), card.BoardId);

        var isMember = await projectRepository.IsUserMemberAsync(board.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember) throw new ForbiddenException();

        card.ToggleChecklistItem(request.ItemId);
        cardRepository.Update(card);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
