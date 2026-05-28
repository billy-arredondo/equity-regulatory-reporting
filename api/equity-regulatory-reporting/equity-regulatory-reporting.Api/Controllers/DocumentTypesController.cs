using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.DocumentTypes.Commands.CreateDocumentType;
using equity_regulatory_reporting.Application.Features.DocumentTypes.Commands.DeleteDocumentType;
using equity_regulatory_reporting.Application.Features.DocumentTypes.Commands.UpdateDocumentType;
using equity_regulatory_reporting.Application.Features.DocumentTypes.Queries.GetDocumentTypeById;
using equity_regulatory_reporting.Application.Features.DocumentTypes.Queries.ListDocumentTypes;
using equity_regulatory_reporting.Domain.Enums;
using equity_regulatory_reporting.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace equity_regulatory_reporting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentTypesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [HasPermission(Permission.DocumentTypeRead)]
    public async Task<IActionResult> List([FromQuery] PageRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new ListDocumentTypesQuery(request), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permission.DocumentTypeRead)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetDocumentTypeByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permission.DocumentTypeWrite)]
    public async Task<IActionResult> Create([FromBody] CreateDocumentTypeCommand command, CancellationToken ct)
    {
        var id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permission.DocumentTypeWrite)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDocumentTypeCommand command, CancellationToken ct)
    {
        await sender.Send(command with { Id = id }, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permission.DocumentTypeDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteDocumentTypeCommand(id), ct);
        return NoContent();
    }
}
