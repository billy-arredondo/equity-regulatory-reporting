using equity_regulatory_reporting.Application.Auth.Commands.Login;
using equity_regulatory_reporting.Application.Auth.Commands.RefreshToken;
using equity_regulatory_reporting.Application.Auth.Commands.Register;
using equity_regulatory_reporting.Application.Auth.Commands.RevokeToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace equity_regulatory_reporting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ISender sender) : ControllerBase
{
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
        return Ok(new { result.AccessToken, result.RefreshToken });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        if (!result.Success) return Unauthorized(result.Error);
        return Ok(new { result.AccessToken, result.RefreshToken });
    }

    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> Revoke([FromBody] RevokeTokenCommand command, CancellationToken ct)
    {
        await sender.Send(command, ct);
        return NoContent();
    }
}
