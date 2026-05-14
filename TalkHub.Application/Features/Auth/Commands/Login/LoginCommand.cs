using MediatR;
using TalkHub.Application.Interfaces.IRepository;
using TalkHub.Application.Interfaces.Services;

namespace TalkHub.Application.Features.Auth.Commands.Login;

public record LoginCommand : IRequest<LoginResponse>
{
    public string Username { get; init; }
    public string Password { get; init; }

    public LoginCommand(string username, string password)
    {
        Username = username?.Trim() ?? string.Empty;
        Password = password?.Trim() ?? string.Empty;
    }
}

public record LoginResponse(string AccessToken, string RefreshToken, string Username, string FullName);