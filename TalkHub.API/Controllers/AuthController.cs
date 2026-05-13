using MediatR;
using Microsoft.AspNetCore.Mvc;
using TalkHub.Application.Common.Models;
using TalkHub.Application.Features.Auth.Commands.Login;
using TalkHub.Application.Features.Auth.Commands.Logout;
using Microsoft.AspNetCore.Authorization;

namespace TalkHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    public AuthController(IMediator mediator, IWebHostEnvironment env, IConfiguration config)
    {
        _mediator = mediator;
        _env = env;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<object>>> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !_env.IsDevelopment(),
            SameSite = _env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(15)
        };

        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !_env.IsDevelopment(),
            SameSite = _env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:RefreshTokenDurationInDays"] ?? "7"))
        };

        Response.Cookies.Append("accessToken", result.AccessToken, cookieOptions);
        Response.Cookies.Append("refreshToken", result.RefreshToken, refreshCookieOptions);

        var userInfo = new
        {
            Username = result.Username,
            FullName = result.FullName,
        };

        return Ok(ApiResponse<object>.SuccessResponse(userInfo, "Đăng nhập thành công."));
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshToken([FromBody] TalkHub.Application.Features.Auth.Commands.RefreshToken.RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<LoginResponse>.SuccessResponse(result, "Làm mới Token thành công."));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<string>>> Logout()
    {
        await _mediator.Send(new LogoutCommand());

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !_env.IsDevelopment(),
            SameSite = _env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(-1)
        };

        Response.Cookies.Append("accessToken", string.Empty, cookieOptions);
        Response.Cookies.Append("refreshToken", string.Empty, cookieOptions);

        return Ok(ApiResponse<string>.SuccessResponse("Đăng xuất thành công."));
    }
}
