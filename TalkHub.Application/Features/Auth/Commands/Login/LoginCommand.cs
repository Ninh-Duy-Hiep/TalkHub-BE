using MediatR;
using TalkHub.Application.Interfaces.IRepository;
using TalkHub.Application.Interfaces.Services;
using BC = BCrypt.Net.BCrypt;

namespace TalkHub.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Username, string Password) : IRequest<LoginResponse>;

public record LoginResponse(string AccessToken, string RefreshToken, string Username, string FullName);

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

    public LoginCommandHandler(IUserRepository userRepository, ITokenService tokenService, Microsoft.Extensions.Configuration.IConfiguration config)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _config = config;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        if (user == null || !BC.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không chính xác.");
        }

        if (!user.IsActive)
        {
            throw new Exception("Tài khoản của bạn đã bị khóa.");
        }

        var accessToken = _tokenService.CreateToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        var days = int.Parse(_config["Jwt:RefreshTokenDurationInDays"] ?? "7");
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(days);
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        return new LoginResponse(accessToken, refreshToken, user.Username, user.FullName);
    }
}
