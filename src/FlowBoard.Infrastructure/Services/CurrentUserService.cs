using FlowBoard.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FlowBoard.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid UserId
    {
        get
        {
            var value = User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(value, out var id)
                ? id
                : throw new UnauthorizedAccessException("Usuário não autenticado.");
        }
    }

    public string Email =>
        User?.FindFirst(JwtRegisteredClaimNames.Email)?.Value
        ?? User?.FindFirst(ClaimTypes.Email)?.Value
        ?? throw new UnauthorizedAccessException("Usuário não autenticado.");

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;
}
