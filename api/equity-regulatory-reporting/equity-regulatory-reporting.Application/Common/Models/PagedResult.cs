namespace equity_regulatory_reporting.Application.Common.Models;

public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);
