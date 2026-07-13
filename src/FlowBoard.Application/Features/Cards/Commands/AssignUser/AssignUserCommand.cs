using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.AssignUser;

public record AssignUserCommand(Guid CardId, Guid UserId, bool Assign) : IRequest;
