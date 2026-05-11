using TalkHub.Domain.Entities;

namespace TalkHub.Application.Interfaces.Services;

public interface ITokenService
{
    string CreateToken(User user);
    string GenerateRefreshToken();
    System.Security.Claims.ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
