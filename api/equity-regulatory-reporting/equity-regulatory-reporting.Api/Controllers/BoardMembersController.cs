using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.BoardMembers.Commands.CreateBoardMember;
using equity_regulatory_reporting.Application.Features.BoardMembers.Commands.DeleteBoardMember;
using equity_regulatory_reporting.Application.Features.BoardMembers.Commands.UpdateBoardMember;
using equity_regulatory_reporting.Application.Features.BoardMembers.Queries.GetBoardMemberById;
using equity_regulatory_reporting.Application.Features.BoardMembers.Queries.ListBoardMembers;
using equity_regulatory_reporting.Domain.Enums;
using equity_regulatory_reporting.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace equity_regulatory_reporting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardMembersController(ISender sender) : ControllerBase
{
    [HttpGet]
    [HasPermission(Permission.BoardRead)]
    public async Task<IActionResult> List([FromQuery] PageRequest request, [FromQuery] Guid? companyId, CancellationToken ct)
    {
        var result = await sender.Send(new ListBoardMembersQuery(request, companyId), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permission.BoardRead)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetBoardMemberByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permission.BoardWrite)]
    public async Task<IActionResult> Create([FromBody] CreateBoardMemberCommand command, CancellationToken ct)
    {
        var id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permission.BoardWrite)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBoardMemberCommand command, CancellationToken ct)
    {
        await sender.Send(command with { Id = id }, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permission.BoardDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteBoardMemberCommand(id), ct);
        return NoContent();
    }
}
