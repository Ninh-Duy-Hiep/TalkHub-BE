using MediatR;
using TalkHub.Application.Interfaces.IRepository;
using BC = BCrypt.Net.BCrypt;

namespace TalkHub.Application.Features.Users.Commands.AdminResetPassword;

public record AdminResetPasswordCommand(Guid UserId, string NewPassword) : IRequest<Unit>;

public class AdminResetPasswordCommandHandler : IRequestHandler<AdminResetPasswordCommand, Unit>
{
    private readonly IUserRepository _userRepository;

    public AdminResetPasswordCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(AdminResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId) 
            ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

        user.PasswordHash = BC.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        return Unit.Value;
    }
}
