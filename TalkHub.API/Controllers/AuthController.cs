using MediatR;
using Microsoft.AspNetCore.Mvc;
using TalkHub.Application.Common.Models;
using TalkHub.Application.Features.Auth.Commands.Login;

namespace TalkHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<LoginResponse>.SuccessResponse(result, "Đăng nhập thành công."));
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshToken([FromBody] TalkHub.Application.Features.Auth.Commands.RefreshToken.RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<LoginResponse>.SuccessResponse(result, "Làm mới Token thành công."));
    }
}
