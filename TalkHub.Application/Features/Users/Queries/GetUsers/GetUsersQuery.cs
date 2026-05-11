using MediatR;
using TalkHub.Application.Common.Models;
using TalkHub.Application.DTOs;
using TalkHub.Application.Interfaces.IRepository;

namespace TalkHub.Application.Features.Users.Queries.GetUsers;

public record GetUsersQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedResponse<UserDto>>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedResponse<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PaginatedResponse<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var (users, totalCount) = await _userRepository.GetPagedAsync(request.PageNumber, request.PageSize);
        
        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            FullName = u.FullName,
            Role = u.Role,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt
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
