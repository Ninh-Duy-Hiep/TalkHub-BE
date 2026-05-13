using MediatR;
using TalkHub.Application.Interfaces.IRepository;
using TalkHub.Application.Interfaces.Services;

namespace TalkHub.Application.Features.Auth.Commands.Logout;

public record LogoutCommand() : IRequest<Unit>;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public LogoutCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new Exception("Không tìm thấy người dùng.");

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await _userRepository.UpdateAsync(user);

        return Unit.Value;
    }
}
