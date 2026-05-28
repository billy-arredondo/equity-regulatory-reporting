using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.Positions.Commands.CreatePosition;
using equity_regulatory_reporting.Application.Features.Positions.Commands.DeletePosition;
using equity_regulatory_reporting.Application.Features.Positions.Commands.UpdatePosition;
using equity_regulatory_reporting.Application.Features.Positions.Queries.GetPositionById;
using equity_regulatory_reporting.Application.Features.Positions.Queries.ListPositions;
using equity_regulatory_reporting.Domain.Enums;
using equity_regulatory_reporting.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace equity_regulatory_reporting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PositionsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [HasPermission(Permission.PositionRead)]
    public async Task<IActionResult> List([FromQuery] PageRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new ListPositionsQuery(request), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permission.PositionRead)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetPositionByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permission.PositionWrite)]
    public async Task<IActionResult> Create([FromBody] CreatePositionCommand command, CancellationToken ct)
    {
        var id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permission.PositionWrite)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePositionCommand command, CancellationToken ct)
    {
        await sender.Send(command with { Id = id }, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permission.PositionDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeletePositionCommand(id), ct);
        return NoContent();
    }
}
