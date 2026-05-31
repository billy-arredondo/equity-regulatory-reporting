using equity_regulatory_reporting.Application.Features.Auth.Commands.Login;
using equity_regulatory_reporting.Application.Features.Auth.Commands.RefreshToken;
using equity_regulatory_reporting.Application.Features.Auth.Commands.Register;
using equity_regulatory_reporting.Application.Features.Auth.Commands.RevokeToken;
using equity_regulatory_reporting.Infrastructure.Options;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace equity_regulatory_reporting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    ISender sender,
    IWebHostEnvironment env,
    IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    private const string RefreshCookieName = "refresh_token";

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Success ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        if (!result.Success) return Unauthorized(result.Error);
        SetRefreshCookie(result.RefreshToken!);
        return Ok(new { result.AccessToken });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        var token = Request.Cookies[RefreshCookieName];
        if (string.IsNullOrEmpty(token)) return Unauthorized("No refresh token.");
        var result = await sender.Send(new RefreshTokenCommand(token), ct);
        if (!result.Success) return Unauthorized(result.Error);
        SetRefreshCookie(result.RefreshToken!);
        return Ok(new { result.AccessToken });
    }

    [HttpPost("revoke")]
    [AllowAnonymous]
    public async Task<IActionResult> Revoke(CancellationToken ct)
    {
        var token = Request.Cookies[RefreshCookieName];
        if (!string.IsNullOrEmpty(token))
            await sender.Send(new RevokeTokenCommand(token), ct);
        ClearRefreshCookie();
        return NoContent();
    }

    private void SetRefreshCookie(string token) =>
        Response.Cookies.Append(RefreshCookieName, token, BuildCookieOptions());

    private void ClearRefreshCookie() =>
        Response.Cookies.Delete(RefreshCookieName, new CookieOptions
        {
            Path = "/api/Auth",
            HttpOnly = true,
            Secure = !env.IsDevelopment(),
            SameSite = env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.Strict,
        });

    private CookieOptions BuildCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = !env.IsDevelopment(),
        SameSite = env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.Strict,
        Path = "/api/Auth",
        MaxAge = TimeSpan.FromDays(jwtOptions.Value.RefreshTokenDays),
    };
}
