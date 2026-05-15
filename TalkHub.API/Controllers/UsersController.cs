using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkHub.Application.Common.Models;
using TalkHub.Application.DTOs;
using TalkHub.Application.Features.Users.Commands.ChangePassword;
using TalkHub.Application.Features.Users.Commands.CreateUser;
using TalkHub.Application.Features.Users.Commands.DeleteUser;
using TalkHub.Application.Features.Users.Commands.UpdateProfile;
using TalkHub.Application.Features.Users.Commands.UpdateUser;
using TalkHub.Application.Features.Users.Queries.GetCurrentUser;
using TalkHub.Application.Features.Users.Queries.GetUserById;
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
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaginatedResponse<UserDto>>> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null, [FromQuery] bool? isActive = null)
    {
        var response = await _mediator.Send(new GetUsersQuery(searchTerm, isActive, pageNumber, pageSize));
        return Ok(response);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(Guid id)
    {
        var response = await _mediator.Send(new GetUserByIdQuery(id));
        return Ok(ApiResponse<UserDto>.SuccessResponse(response));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateUser([FromBody] CreateUserCommand command)
    {
        var userId = await _mediator.Send(command);
        return StatusCode(201, ApiResponse<Guid>.SuccessResponse(userId, "Tạo người dùng thành công.", 201));
    }
    
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetMe()
    {
        var response = await _mediator.Send(new GetCurrentUserQuery());
        return Ok(ApiResponse<UserDto>.SuccessResponse(response));
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<string>>> UpdateProfile([FromBody] UpdateProfileCommand command)
    {
        await _mediator.Send(command);
        return Ok(ApiResponse<string>.SuccessResponse("Cập nhật thông tin thành công."));
    }

    [HttpPut("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<string>>> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok(ApiResponse<string>.SuccessResponse("Đổi mật khẩu thành công."));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<string>>> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<string>.FailureResponse(400, "ID không khớp."));
        }
        await _mediator.Send(command);
        return Ok(ApiResponse<string>.SuccessResponse("Cập nhật người dùng thành công."));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteUser(Guid id)
    {
        await _mediator.Send(new DeleteUserCommand(id));
        return Ok(ApiResponse<string>.SuccessResponse("Xóa người dùng thành công."));
    }

    [HttpPut("{id}/reset-password")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<string>>> ResetPassword(Guid id, [FromBody] string newPassword)
    {
        await _mediator.Send(new TalkHub.Application.Features.Users.Commands.AdminResetPassword.AdminResetPasswordCommand(id, newPassword));
        return Ok(ApiResponse<string>.SuccessResponse("Đặt lại mật khẩu thành công."));
    }
}
