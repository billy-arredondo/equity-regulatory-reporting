using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.Persons.Commands.CreatePerson;
using equity_regulatory_reporting.Application.Features.Persons.Commands.DeletePerson;
using equity_regulatory_reporting.Application.Features.Persons.Commands.UpdatePerson;
using equity_regulatory_reporting.Application.Features.Persons.Queries.GetPersonById;
using equity_regulatory_reporting.Application.Features.Persons.Queries.ListPersons;
using equity_regulatory_reporting.Domain.Enums;
using equity_regulatory_reporting.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace equity_regulatory_reporting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [HasPermission(Permission.PersonRead)]
    public async Task<IActionResult> List([FromQuery] PageRequest request, [FromQuery] PersonType? personType, CancellationToken ct)
    {
        var result = await sender.Send(new ListPersonsQuery(request, personType), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permission.PersonRead)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetPersonByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permission.PersonWrite)]
    public async Task<IActionResult> Create([FromBody] CreatePersonCommand command, CancellationToken ct)
    {
        var id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permission.PersonWrite)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePersonCommand command, CancellationToken ct)
    {
        await sender.Send(command with { Id = id }, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permission.PersonDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeletePersonCommand(id), ct);
        return NoContent();
    }
}
