using equity_regulatory_reporting.Application.Features.PersonTypes.Queries.ListPersonTypes;
using equity_regulatory_reporting.Domain.Enums;
using equity_regulatory_reporting.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace equity_regulatory_reporting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonTypesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [HasPermission(Permission.PersonRead)]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var result = await sender.Send(new ListPersonTypesQuery(), ct);
        return Ok(result);
    }
}
