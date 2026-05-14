using MediatR;
using TalkHub.Domain.Enums;

namespace TalkHub.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand : IRequest<Guid>
{ 
    public string Username { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string FullName { get; init; } = null!;
    public string? PhoneNumber { get; init; }
    public UserRole Role { get; init; }

    public CreateUserCommand(string username, string password, string fullName, string email, string? phoneNumber, UserRole role)
    {
        Username = username?.Trim() ?? string.Empty;
        Password = password?.Trim() ?? string.Empty;
        FullName = fullName?.Trim() ?? string.Empty;
        Email = email?.Trim() ?? string.Empty;
        PhoneNumber = phoneNumber?.Trim();
        Role = role;
    }
}
