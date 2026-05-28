namespace equity_regulatory_reporting.Application.Common.Models;

public record PageRequest(int Page = 1, int PageSize = 25, string? Search = null);
