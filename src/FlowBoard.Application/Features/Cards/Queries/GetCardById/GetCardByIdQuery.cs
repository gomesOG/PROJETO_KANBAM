using FlowBoard.Application.DTOs;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Queries.GetCardById;

public record GetCardByIdQuery(Guid CardId) : IRequest<CardDetailDto>;
