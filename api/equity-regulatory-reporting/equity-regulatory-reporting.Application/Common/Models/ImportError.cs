namespace equity_regulatory_reporting.Application.Common.Models;

public record ImportError(int Row, string? Field, string Message);
