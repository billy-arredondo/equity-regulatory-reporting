using FluentValidation;

namespace equity_regulatory_reporting.Application.Common.Csv;

public static class PsvParser
{
    public static PsvDocument Parse(Stream stream, IReadOnlyCollection<string> expectedColumns)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);

        string? headerLine = null;
        int lineNumber = 0;

        while (!reader.EndOfStream)
        {
            lineNumber++;
            var line = reader.ReadLine();
            if (!string.IsNullOrWhiteSpace(line))
            {
                headerLine = line;
                break;
            }
        }

        if (headerLine is null)
            throw new ValidationException("The file is empty or contains no header row.");

        var headerCells = headerLine.Split('|').Select(h => h.Trim()).ToList();
        var headers = headerCells
            .Select((h, i) => (Key: h.ToLowerInvariant(), Index: i))
            .ToDictionary(x => x.Key, x => x.Index);

        var missing = expectedColumns
            .Where(c => !headers.ContainsKey(c.ToLowerInvariant()))
            .ToList();

        if (missing.Count > 0)
            throw new ValidationException($"Missing required columns: {string.Join(", ", missing)}.");

        var rows = new List<PsvRow>();

        while (!reader.EndOfStream)
        {
            lineNumber++;
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var cells = line.Split('|').Select(c => c.Trim()).ToList();
            rows.Add(new PsvRow(lineNumber, cells));
        }

        return new PsvDocument(headers, rows);
    }
}
