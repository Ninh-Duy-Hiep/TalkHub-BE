using MediatR;
using Microsoft.AspNetCore.Mvc;
using TalkHub.Application.Common.Models;
using TalkHub.Application.Features.Auth.Commands.Login;
using TalkHub.Application.Features.Auth.Commands.Logout;
using TalkHub.Application.Features.Auth.Commands.RefreshToken;
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
        SetTokenCookies(result.AccessToken, result.RefreshToken);

        var userInfo = new
        {
            Username = result.Username,
            FullName = result.FullName,
        };

        return Ok(ApiResponse<object>.SuccessResponse(userInfo, "Đăng nhập thành công."));
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<object>>> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var accessToken = command.AccessToken ?? Request.Cookies["accessToken"];
        var refreshToken = command.RefreshToken ?? Request.Cookies["refreshToken"];

        var result = await _mediator.Send(new RefreshTokenCommand(accessToken, refreshToken));
        
        SetTokenCookies(result.AccessToken, result.RefreshToken);

        return Ok(ApiResponse<object>.SuccessResponse(new { 
            Username = result.Username, 
            FullName = result.FullName 
        }, "Làm mới Token thành công."));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<string>>> Logout()
    {
        await _mediator.Send(new LogoutCommand());
        RemoveTokenCookies();
        return Ok(ApiResponse<string>.SuccessResponse("Đăng xuất thành công."));
    }

    private void SetTokenCookies(string accessToken, string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:TokenDurationInMinutes"] ?? "15")),
            Path = "/"
        };

        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:RefreshTokenDurationInDays"] ?? "7")),
            Path = "/"
        };

        Response.Cookies.Append("accessToken", accessToken, cookieOptions);
        Response.Cookies.Append("refreshToken", refreshToken, refreshCookieOptions);
    }

    private void RemoveTokenCookies()
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(-1),
            Path = "/"
        };

        Response.Cookies.Delete("accessToken", cookieOptions);
        Response.Cookies.Delete("refreshToken", cookieOptions);
    }
}
