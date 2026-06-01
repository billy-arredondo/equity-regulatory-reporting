namespace equity_regulatory_reporting.Application.Common.Csv;

public sealed class PsvDocument
{
    private readonly Dictionary<string, int> _headers;

    internal PsvDocument(Dictionary<string, int> headers, IReadOnlyList<PsvRow> rows)
    {
        _headers = headers;
        Rows = rows;
    }

    public IReadOnlyList<PsvRow> Rows { get; }

    public string? Get(PsvRow row, string column)
    {
        if (!_headers.TryGetValue(column.ToLowerInvariant(), out var index))
            return null;
        if (index >= row.Cells.Count)
            return null;
        var value = row.Cells[index];
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}

public sealed record PsvRow(int LineNumber, IReadOnlyList<string> Cells);
