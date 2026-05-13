using MediatR;
using TalkHub.Application.Common.Models;
using TalkHub.Application.DTOs;
using TalkHub.Application.Interfaces.IRepository;

namespace TalkHub.Application.Features.Users.Queries.GetUsers;

public record GetUsersQuery(string? searchTerm, bool? isActive, int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedResponse<UserDto>>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedResponse<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PaginatedResponse<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var (users, totalCount) = await _userRepository.GetPagedAsync(request.searchTerm, request.isActive, request.PageNumber, request.PageSize);

        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            FullName = u.FullName,
            Role = u.Role,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            IsDeleted = u.IsDeleted,
            AvatarUrl = u.AvatarUrl,
            PhoneNumber = u.PhoneNumber,
            LastLoginAt = u.LastLoginAt
        });

        return new PaginatedResponse<UserDto>
        {
            Data = new PagedData<UserDto>
            {
                Items = userDtos,
                MetaData = new MetaData
                {
                    CurrentPage = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalItems = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
                }
            }
        };
    }
}
