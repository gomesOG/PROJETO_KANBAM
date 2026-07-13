using FlowBoard.Application.Common.Exceptions;
using FlowBoard.Application.DTOs;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Interfaces.Repositories;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Queries.GetCardById;

public class GetCardByIdQueryHandler(
    ICardRepository cardRepository,
    IBoardRepository boardRepository,
    IProjectRepository projectRepository,
    ICurrentUserService currentUser
) : IRequestHandler<GetCardByIdQuery, CardDetailDto>
{
    public async Task<CardDetailDto> Handle(GetCardByIdQuery request, CancellationToken cancellationToken)
    {
        var card = await cardRepository.GetWithDetailsAsync(request.CardId, cancellationToken)
            ?? throw new NotFoundException(nameof(Card), request.CardId);

        var board = await boardRepository.GetByIdAsync(card.BoardId, cancellationToken)
            ?? throw new NotFoundException(nameof(Board), card.BoardId);

        var isMember = await projectRepository.IsUserMemberAsync(board.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember) throw new ForbiddenException();

        return new CardDetailDto(
            card.Id, card.Title, card.Description,
            card.ColumnId, card.BoardId, card.Position,
            card.Priority.ToString(), card.Status.ToString(),
            card.DueDate,
            card.DueDate.HasValue && card.DueDate.Value < DateTime.UtcNow,
            card.IsArchived,
            card.CreatedByUserId,
            card.Tags.Select(t => new CardTagDto(t.Id, t.Name, t.Color)).ToList().AsReadOnly(),
            card.Assignees.Select(a => new CardAssigneeDto(a.UserId, string.Empty, null)).ToList().AsReadOnly(),
            card.ChecklistItems.OrderBy(i => i.Position).Select(i => new ChecklistItemDto(i.Id, i.Text, i.IsCompleted, i.Position)).ToList().AsReadOnly(),
            card.CreatedAt, card.UpdatedAt);
    }
}
