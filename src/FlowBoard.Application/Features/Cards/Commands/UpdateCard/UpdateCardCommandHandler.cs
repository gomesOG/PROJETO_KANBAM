using FlowBoard.Application.Common.Exceptions;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Interfaces;
using FlowBoard.Domain.Interfaces.Repositories;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.UpdateCard;

public class UpdateCardCommandHandler(
    ICardRepository cardRepository,
    IProjectRepository projectRepository,
    IBoardRepository boardRepository,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork
) : IRequestHandler<UpdateCardCommand>
{
    public async Task Handle(UpdateCardCommand request, CancellationToken cancellationToken)
    {
        var card = await cardRepository.GetByIdAsync(request.CardId, cancellationToken)
            ?? throw new NotFoundException(nameof(Card), request.CardId);

        var board = await boardRepository.GetByIdAsync(card.BoardId, cancellationToken)
            ?? throw new NotFoundException(nameof(Board), card.BoardId);

        var isMember = await projectRepository.IsUserMemberAsync(board.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember) throw new ForbiddenException();

        card.Update(request.Title, request.Description, request.Priority, request.DueDate);
        cardRepository.Update(card);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
