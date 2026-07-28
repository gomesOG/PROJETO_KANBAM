using FlowBoard.Application.Common.Exceptions;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Interfaces.Repositories;
using MediatR;

namespace FlowBoard.Application.Features.Auth.Queries.LoginUser;

public class LoginUserQueryHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService
) : IRequestHandler<LoginUserQuery, LoginUserResponse>
{
    public async Task<LoginUserResponse> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new NotFoundException("Credenciais inválidas.");

        if (!user.IsActive)
            throw new ForbiddenException("Conta desativada. Entre em contato com o suporte.");

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new NotFoundException("Credenciais inválidas.");

        var accessToken = tokenService.GenerateToken(user.Id, user.Email.Value, user.Name);
        var refreshToken = tokenService.GenerateRefreshToken();

        return new LoginUserResponse(accessToken, refreshToken, user.Name, user.Email.Value, user.Id);
    }
}
