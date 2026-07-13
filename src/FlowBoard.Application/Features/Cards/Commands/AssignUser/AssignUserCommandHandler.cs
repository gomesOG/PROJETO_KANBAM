using FlowBoard.Application.Common.Exceptions;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Interfaces;
using FlowBoard.Domain.Interfaces.Repositories;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.AssignUser;

public class AssignUserCommandHandler(
    ICardRepository cardRepository,
    IBoardRepository boardRepository,
    IProjectRepository projectRepository,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork
) : IRequestHandler<AssignUserCommand>
{
    public async Task Handle(AssignUserCommand request, CancellationToken cancellationToken)
    {
        var card = await cardRepository.GetWithDetailsAsync(request.CardId, cancellationToken)
            ?? throw new NotFoundException(nameof(Card), request.CardId);

        var board = await boardRepository.GetByIdAsync(card.BoardId, cancellationToken)
            ?? throw new NotFoundException(nameof(Board), card.BoardId);

        var isMember = await projectRepository.IsUserMemberAsync(board.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember) throw new ForbiddenException();

        if (request.Assign)
            card.AssignUser(request.UserId);
        else
            card.UnassignUser(request.UserId);

        cardRepository.Update(card);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
