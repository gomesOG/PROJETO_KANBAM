using FlowBoard.Application.Common.Exceptions;
using FlowBoard.Application.DTOs;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Interfaces.Repositories;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Queries.GetBoardById;

public class GetBoardByIdQueryHandler(
    IBoardRepository boardRepository,
    IProjectRepository projectRepository,
    ICurrentUserService currentUser
) : IRequestHandler<GetBoardByIdQuery, BoardDetailDto>
{
    public async Task<BoardDetailDto> Handle(GetBoardByIdQuery request, CancellationToken cancellationToken)
    {
        var board = await boardRepository.GetWithColumnsAndCardsAsync(request.BoardId, cancellationToken)
            ?? throw new NotFoundException(nameof(Board), request.BoardId);

        var isMember = await projectRepository.IsUserMemberAsync(board.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember) throw new ForbiddenException();

        var columns = board.Columns
            .OrderBy(c => c.Position)
            .Select(c => new ColumnDto(
                c.Id, c.Name, c.Color, c.BoardId, c.Position, c.CardLimit,
                c.Cards
                    .Where(card => !card.IsArchived)
                    .OrderBy(card => card.Position)
                    .Select(card => MapCardSummary(card))
                    .ToList()
                    .AsReadOnly()))
            .ToList()
            .AsReadOnly();

        return new BoardDetailDto(board.Id, board.Name, board.Description, board.ProjectId, board.IsArchived, columns, board.CreatedAt);
    }

    private static CardSummaryDto MapCardSummary(Card card) =>
        new(
            card.Id, card.Title, card.ColumnId, card.Position,
            card.Priority.ToString(),
            card.DueDate,
            card.DueDate.HasValue && card.DueDate.Value < DateTime.UtcNow,
            card.ChecklistItems.Count,
            card.ChecklistItems.Count(i => i.IsCompleted),
            card.Tags.Select(t => new CardTagDto(t.Id, t.Name, t.Color)).ToList().AsReadOnly(),
            card.Assignees.Select(a => new CardAssigneeDto(a.UserId, string.Empty, null)).ToList().AsReadOnly()
        );
}
