namespace FlowBoard.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(Guid userId, string email, string name);
    string GenerateRefreshToken();
    Guid? ValidateToken(string token);
}
