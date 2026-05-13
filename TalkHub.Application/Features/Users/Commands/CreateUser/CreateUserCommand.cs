using MediatR;
using TalkHub.Application.Interfaces.IRepository;
using TalkHub.Domain.Entities;
using TalkHub.Domain.Enums;
using BC = BCrypt.Net.BCrypt;

namespace TalkHub.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(string Username, string Password, string FullName, string Email, string? PhoneNumber, UserRole Role) : IRequest<Guid>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsAsync(request.Username))
        {
            throw new Exception("Tên đăng nhập đã tồn tại.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BC.HashPassword(request.Password),
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        return user.Id;
    }
}
