using equity_regulatory_reporting.Api.Common;
using equity_regulatory_reporting.Api.Import;
using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.Countries.Commands.CreateCountry;
using equity_regulatory_reporting.Application.Features.Countries.Commands.DeleteCountry;
using equity_regulatory_reporting.Application.Features.Countries.Commands.ImportCountries;
using equity_regulatory_reporting.Application.Features.Countries.Commands.UpdateCountry;
using equity_regulatory_reporting.Application.Features.Countries.Queries.GetCountryById;
using equity_regulatory_reporting.Application.Features.Countries.Queries.ListCountries;
using equity_regulatory_reporting.Domain.Enums;
using equity_regulatory_reporting.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace equity_regulatory_reporting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountriesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [HasPermission(Permission.CountryRead)]
    public async Task<IActionResult> List([FromQuery] PageRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new ListCountriesQuery(request), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permission.CountryRead)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetCountryByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPost]
    [HasPermission(Permission.CountryWrite)]
    public async Task<IActionResult> Create([FromBody] CreateCountryCommand command, CancellationToken ct)
    {
        var id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permission.CountryWrite)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCountryCommand command, CancellationToken ct)
    {
        await sender.Send(command with { Id = id }, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permission.CountryDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteCountryCommand(id), ct);
        return NoContent();
    }

    [HttpPost("import")]
    [HasPermission(Permission.CountryWrite)]
    public async Task<IActionResult> Import(IFormFile file, [FromForm] ImportMode mode, CancellationToken ct)
    {
        var rows = CountriesImportParser.Parse(file);
        var result = await sender.Send(new ImportCountriesCommand(rows, mode), ct);
        return this.ToActionResult(result, mode);
    }
}
