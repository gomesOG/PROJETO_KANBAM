using MediatR;

namespace FlowBoard.Application.Features.Auth.Queries.LoginUser;

public record LoginUserQuery(string Email, string Password) : IRequest<LoginUserResponse>;

public record LoginUserResponse(string AccessToken, string RefreshToken, string UserName, string Email, Guid UserId);
