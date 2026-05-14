using MediatR;
using TalkHub.Application.DTOs;
using TalkHub.Application.Interfaces.IRepository;
using TalkHub.Application.Interfaces.Services;

namespace TalkHub.Application.Features.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery() : IRequest<UserDto>;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetCurrentUserQueryHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new UnauthorizedAccessException("Không tìm thấy người dùng.");

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            IsDeleted = user.IsDeleted,
            AvatarUrl = user.AvatarUrl,
            PhoneNumber = user.PhoneNumber,
            LastLoginAt = user.LastLoginAt
        };
    }
}
