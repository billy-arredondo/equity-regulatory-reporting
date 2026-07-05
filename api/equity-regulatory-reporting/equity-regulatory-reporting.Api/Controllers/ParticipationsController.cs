using equity_regulatory_reporting.Api.Common;
using equity_regulatory_reporting.Api.Import;
using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.Participations.Commands.CreateParticipation;
using equity_regulatory_reporting.Application.Features.Participations.Commands.DeleteParticipation;
using equity_regulatory_reporting.Application.Features.Participations.Commands.ImportParticipations;
using equity_regulatory_reporting.Application.Features.Participations.Commands.UpdateParticipation;
using equity_regulatory_reporting.Application.Features.Participations.Queries.GetParticipationById;
using equity_regulatory_reporting.Application.Features.Participations.Queries.ListParticipations;
using equity_regulatory_reporting.Domain.Enums;
using equity_regulatory_reporting.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace equity_regulatory_reporting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParticipationsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [HasPermission(Permission.ParticipationRead)]
    public async Task<IActionResult> List([FromQuery] PageRequest request, [FromQuery] Guid? companyId, CancellationToken ct)
    {
        var result = await sender.Send(new ListParticipationsQuery(request, companyId), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permission.ParticipationRead)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetParticipationByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permission.ParticipationWrite)]
    public async Task<IActionResult> Create([FromBody] CreateParticipationCommand command, CancellationToken ct)
    {
        var id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permission.ParticipationWrite)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateParticipationCommand command, CancellationToken ct)
    {
        await sender.Send(command with { Id = id }, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permission.ParticipationDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteParticipationCommand(id), ct);
        return NoContent();
    }

    [HttpPost("import")]
    [HasPermission(Permission.ParticipationWrite)]
    public async Task<IActionResult> Import(IFormFile file, [FromForm] ImportMode mode, CancellationToken ct)
    {
        var rows = ParticipationsImportParser.Parse(file);
        var result = await sender.Send(new ImportParticipationsCommand(rows, mode), ct);
        return this.ToActionResult(result, mode);
    }
}
