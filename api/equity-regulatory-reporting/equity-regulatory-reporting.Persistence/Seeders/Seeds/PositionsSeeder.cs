using equity_regulatory_reporting.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Persistence.Seeders;

public partial class DatabaseSeeder
{
    private async Task SeedPositionsAsync()
    {
        (string Name, string ReportCode)[] data =
        [
            ("Sin cargo",                 "00"),
            ("Presidente del directorio", "01"),
            ("Director",                  "02"),
            ("Gerente",                   "03"),
            ("Principal funcionario",     "04"),
            ("Asesor",                    "05"),
            ("Gestor",                    "06"),
        ];

        var existing = await context.Positions.ToListAsync();
        var existingByName = existing.ToDictionary(p => p.Name);

        foreach (var (name, code) in data)
        {
            if (existingByName.TryGetValue(name, out var position))
            {
                if (position.ReportCode != code)
                    position.ReportCode = code;
            }
            else
            {
                context.Positions.Add(new Position { Name = name, ReportCode = code });
            }
        }

        await context.SaveChangesAsync();
    }
}
