using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkHub.Application.Common.Models;
using TalkHub.Application.DTOs;
using TalkHub.Application.Features.Users.Commands.CreateUser;
using TalkHub.Application.Features.Users.Queries.GetUsers;

namespace TalkHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<UserDto>>> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _mediator.Send(new GetUsersQuery(pageNumber, pageSize));
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateUser([FromBody] CreateUserCommand command)
    {
        var userId = await _mediator.Send(command);
        return StatusCode(201, ApiResponse<Guid>.SuccessResponse(userId, "Tạo người dùng thành công.", 201));
    }
}
