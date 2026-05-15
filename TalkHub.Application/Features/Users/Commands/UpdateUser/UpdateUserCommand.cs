using MediatR;
using TalkHub.Domain.Enums;

namespace TalkHub.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? PhoneNumber { get; init; }
    public string? AvatarUrl { get; init; }
    public UserRole Role { get; init; }
    public bool IsActive { get; init; }

    public UpdateUserCommand(Guid id, string fullName, string email, string? phoneNumber, UserRole role, bool isActive, string? avatarUrl = null)
    {
        Id = id;
        FullName = fullName?.Trim() ?? string.Empty;
        Email = email?.Trim() ?? string.Empty;
        PhoneNumber = phoneNumber?.Trim();
        Role = role;
        IsActive = isActive;
        AvatarUrl = avatarUrl?.Trim();
    }
}
