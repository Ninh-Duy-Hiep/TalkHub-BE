using System.Security.Claims;
using MediatR;
using TalkHub.Application.Features.Auth.Commands.Login;
using TalkHub.Application.Interfaces.IRepository;
using TalkHub.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace TalkHub.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string? AccessToken, string? RefreshToken) : IRequest<LoginResponse>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _config;

    public RefreshTokenCommandHandler(IUserRepository userRepository, ITokenService tokenService, IConfiguration config)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _config = config;
    }

    public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.AccessToken))
        {
            throw new UnauthorizedAccessException("Access Token không được để trống.");
        }

        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            throw new UnauthorizedAccessException("Refresh Token không được để trống.");
        }

        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        var username = principal.Identity?.Name;

        if (string.IsNullOrEmpty(username))
        {
            throw new UnauthorizedAccessException("Token không hợp lệ.");
        }

        var user = await _userRepository.GetByUsernameAsync(username);

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Refresh Token không hợp lệ hoặc đã hết hạn.");
        }

        var newAccessToken = _tokenService.CreateToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        var days = int.Parse(_config["Jwt:RefreshTokenDurationInDays"] ?? "7");
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(days);
        await _userRepository.UpdateAsync(user);

        return new LoginResponse(newAccessToken, newRefreshToken, user.Username, user.FullName);
    }
}
