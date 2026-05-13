using MediatR;
using TalkHub.Application.Interfaces.IRepository;
using TalkHub.Application.Interfaces.Services;

namespace TalkHub.Application.Features.Users.Commands.UpdateProfile;

public record UpdateProfileCommand(string FullName, string? AvatarUrl, string? PhoneNumber) : IRequest<Unit>;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateProfileCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new Exception("Không tìm thấy người dùng.");

        user.FullName = request.FullName;
        user.AvatarUrl = request.AvatarUrl;
        user.PhoneNumber = request.PhoneNumber;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        return Unit.Value;
    }
}
