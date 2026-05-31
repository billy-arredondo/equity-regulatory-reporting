using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.Users.Commands.AssignRole;
using equity_regulatory_reporting.Application.Features.Users.Commands.DeleteUser;
using equity_regulatory_reporting.Application.Features.Users.Commands.UpdateUser;
using equity_regulatory_reporting.Application.Features.Users.Queries.GetUserById;
using equity_regulatory_reporting.Application.Features.Users.Queries.ListRoles;
using equity_regulatory_reporting.Application.Features.Users.Queries.ListUsers;
using equity_regulatory_reporting.Domain.Enums;
using equity_regulatory_reporting.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace equity_regulatory_reporting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(ISender sender) : ControllerBase
{
    [HttpGet]
    [HasPermission(Permission.UserRead)]
    public async Task<IActionResult> List([FromQuery] PageRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new ListUsersQuery(request), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permission.UserRead)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetUserByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permission.UserWrite)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command, CancellationToken ct)
    {
        await sender.Send(command with { Id = id }, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permission.UserDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteUserCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/role")]
    [HasPermission(Permission.UserWrite)]
    public async Task<IActionResult> AssignRole(Guid id, [FromBody] AssignRoleCommand command, CancellationToken ct)
    {
        await sender.Send(command with { UserId = id }, ct);
        return NoContent();
    }

    [HttpGet("roles")]
    [HasPermission(Permission.UserRead)]
    public async Task<IActionResult> ListRoles(CancellationToken ct)
    {
        var result = await sender.Send(new ListRolesQuery(), ct);
        return Ok(result);
    }
}
