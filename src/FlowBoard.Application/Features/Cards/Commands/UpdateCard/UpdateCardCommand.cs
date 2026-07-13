using FlowBoard.Domain.Enums;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.UpdateCard;

public record UpdateCardCommand(
    Guid CardId,
    string Title,
    string? Description,
    Priority Priority,
    DateTime? DueDate
) : IRequest;
