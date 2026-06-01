using equity_regulatory_reporting.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace equity_regulatory_reporting.Api.Common;

public static class ImportResultMapper
{
    public static IActionResult ToActionResult(this ControllerBase controller, ImportResult result, ImportMode mode)
    {
        if (result.FailedCount == 0)
            return controller.Ok(result);

        return mode == ImportMode.Atomic
            ? controller.UnprocessableEntity(result)
            : controller.StatusCode(StatusCodes.Status207MultiStatus, result);
    }
}
