using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.MoveCard;

public record MoveCardCommand(
    Guid CardId,
    Guid TargetColumnId,
    int NewPosition
) : IRequest;
