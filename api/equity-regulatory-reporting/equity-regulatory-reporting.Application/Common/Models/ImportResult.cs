namespace equity_regulatory_reporting.Application.Common.Models;

public record ImportResult(int ImportedCount, int FailedCount, IReadOnlyList<ImportError> Errors);
