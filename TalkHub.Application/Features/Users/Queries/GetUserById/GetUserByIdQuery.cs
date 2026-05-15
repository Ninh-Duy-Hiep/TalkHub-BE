using MediatR;
using TalkHub.Application.DTOs;
using TalkHub.Application.Interfaces.IRepository;

namespace TalkHub.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);

        if (user == null || user.IsDeleted)
        {
            throw new KeyNotFoundException("Người dùng không tồn tại.");
        }

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
