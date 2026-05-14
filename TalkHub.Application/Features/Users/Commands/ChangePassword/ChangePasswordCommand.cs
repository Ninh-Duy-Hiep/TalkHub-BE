using MediatR;
using TalkHub.Application.Interfaces.IRepository;
using FluentValidation;
using FluentValidation.Results;
using TalkHub.Application.Interfaces.Services;
using BC = BCrypt.Net.BCrypt;

namespace TalkHub.Application.Features.Users.Commands.ChangePassword;

public record ChangePasswordCommand(string OldPassword, string NewPassword) : IRequest<Unit>;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public ChangePasswordCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

        if (!BC.Verify(request.OldPassword, user.PasswordHash))
        {
            throw new ValidationException(new[] { new ValidationFailure("oldPassword", "Mật khẩu cũ không chính xác.") });
        }

        user.PasswordHash = BC.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        return Unit.Value;
    }
}
