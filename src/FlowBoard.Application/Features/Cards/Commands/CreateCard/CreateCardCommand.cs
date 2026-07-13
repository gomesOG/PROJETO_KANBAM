using FlowBoard.Application.DTOs;
using FlowBoard.Domain.Enums;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.CreateCard;

public record CreateCardCommand(
    string Title,
    Guid ColumnId,
    string? Description,
    Priority Priority = Priority.Medium,
    DateTime? DueDate = null
) : IRequest<CardSummaryDto>;
