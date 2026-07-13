using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.ToggleChecklistItem;

public record ToggleChecklistItemCommand(Guid CardId, Guid ItemId) : IRequest;
