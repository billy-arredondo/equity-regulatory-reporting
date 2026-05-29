using equity_regulatory_reporting.Domain.Entities;
using equity_regulatory_reporting.Domain.Enums;
using equity_regulatory_reporting.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace equity_regulatory_reporting.Persistence.Seeders;

public class DatabaseSeeder(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IConfiguration configuration,
    ILogger<DatabaseSeeder> logger)
{
    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedPositionsAsync();
        await SeedCountriesAsync();
        await SeedDocumentTypesAsync();
        await SeedDefaultAdminAsync();
    }

    private async Task SeedRolesAsync()
    {
        ApplicationRole[] roles =
        [
            new ()
            {
                Name = "Admin",
                NormalizedName = "ADMIN",
                Permissions = Permission.Admin
            },
            new ()
            {
                Name = "Guest",
                NormalizedName = "GUEST",
                Permissions =
                    Permission.PersonRead |
                    Permission.ParticipationRead |
                    Permission.BoardRead |
                    Permission.ReportRead |
                    Permission.CountryRead |
                    Permission.DocumentTypeRead |
                    Permission.PositionRead
            }
        ];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                var result = await roleManager.CreateAsync(role);
                if (!result.Succeeded)
                    logger.LogWarning("Failed to create role {Role}: {Errors}", role.Name, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    private async Task SeedPositionsAsync()
    {
        string[] names =
        [
            "Sin cargo",
            "Presidente del directorio",
            "Director",
            "Gerente",
            "Principal funcionario",
            "Asesor",
            "Gestor",
        ];

        var existing = await context.Positions.Select(p => p.Name).ToHashSetAsync();
        var toAdd = names.Where(n => !existing.Contains(n)).Select(n => new Position { Name = n });
        context.Positions.AddRange(toAdd);
        await context.SaveChangesAsync();
    }

    private async Task SeedCountriesAsync()
    {
        (string Name, string Abbreviation)[] data =
        [
            ("Colombia",                    "CO"),
            ("Estados Unidos",              "US"),
            ("Panamá",                      "PA"),
            ("Ecuador",                     "EC"),
            ("Venezuela",                   "VE"),
            ("Perú",                        "PE"),
            ("Brasil",                      "BR"),
            ("México",                      "MX"),
            ("España",                      "ES"),
            ("Alemania",                    "DE"),
            ("Reino Unido",                 "GB"),
            ("Francia",                     "FR"),
            ("China",                       "CN"),
            ("Japón",                       "JP"),
            ("Países Bajos",                "NL"),
            ("Suiza",                       "CH"),
            ("Luxemburgo",                  "LU"),
            ("Islas Caimán",                "KY"),
            ("Islas Vírgenes Británicas",   "VG"),
        ];

        var existing = await context.Countries.Select(c => c.Abbreviation).ToHashSetAsync();
        var toAdd = data
            .Where(d => !existing.Contains(d.Abbreviation))
            .Select(d => new Country { Name = d.Name, Abbreviation = d.Abbreviation });
        context.Countries.AddRange(toAdd);
        await context.SaveChangesAsync();
    }

    private async Task SeedDocumentTypesAsync()
    {
        var existing = await context.DocumentTypes.Select(d => d.Abbreviation).ToHashSetAsync();

        List<DocumentType> docTypes =
        [
            new()
            {
                Name = "Otros",
                Abbreviation = "OTR",
                ValidationRegex = @"^[A-Za-z0-9\-\/\.]{1,20}$",
                AllowedPersonTypes =
                [
                    new() { PersonType = PersonType.Natural },
                    new() { PersonType = PersonType.Legal }
                ]
            },
            new()
            {
                Name = "Documento Nacional de Identidad",
                Abbreviation = "DNI",
                ValidationRegex = @"^\d{8}$",
                AllowedPersonTypes =
                [
                    new() { PersonType = PersonType.Natural }
                ]
            },
            new()
            {
                Name = "Carnet de Identidad",
                Abbreviation = "CI",
                ValidationRegex = @"^\d{1,9}$",
                AllowedPersonTypes =
                [
                    new() { PersonType = PersonType.Natural }
                ]
            },
            new()
            {
                Name = "Carnet de Extranjería",
                Abbreviation = "CE",
                ValidationRegex = @"^\d{1,11}$",
                AllowedPersonTypes =
                [
                    new() { PersonType = PersonType.Natural }
                ]
            },
            new()
            {
                Name = "Pasaporte",
                Abbreviation = "PA",
                ValidationRegex = @"^[A-Za-z0-9]{1,15}$",
                AllowedPersonTypes =
                [
                    new() { PersonType = PersonType.Natural }
                ]
            },
            new()
            {
                Name = "Carnet de Permiso Temporal de Permanencia",
                Abbreviation = "CPP",
                ValidationRegex = @"^\d{9}$",
                AllowedPersonTypes =
                [
                    new() { PersonType = PersonType.Natural }
                ]
            },
            new()
            {
                Name = "Permiso Temporal de Permanencia",
                Abbreviation = "PTP",
                ValidationRegex = @"^\d{9}$",
                AllowedPersonTypes =
                [
                    new() { PersonType = PersonType.Natural }
                ]
            },
            new()
            {
                Name = "Registro Único del Contribuyente",
                Abbreviation = "RUC",
                ValidationRegex = @"^\d{11}$",
                AllowedPersonTypes =
                [
                    new() { PersonType = PersonType.Natural },
                    new() { PersonType = PersonType.Legal }
                ]
            },
            new()
            {
                Name = "Personas Jurídicas Locales sin RUC",
                Abbreviation = "PJ-SRUC",
                ValidationRegex = @"^[A-Za-z0-9\-\/\.]{1,20}$",
                AllowedPersonTypes =
                [
                    new() { PersonType = PersonType.Legal }
                ]
            },
            new()
            {
                Name = "Empresas Extranjeras sin RUC",
                Abbreviation = "EE-SRUC",
                ValidationRegex = @"^[A-Za-z0-9\-\/\.]{1,20}$",
                AllowedPersonTypes =
                [
                    new() { PersonType = PersonType.Legal }
                ]
            },
            new()
            {
                Name = "Society for Worldwide Interbank Financial Telecommunication",
                Abbreviation = "SWIFT",
                ValidationRegex = @"^[A-Za-z]{6}[A-Za-z0-9]{2}([A-Za-z0-9]{3})?$",
                AllowedPersonTypes =
                [
                    new() { PersonType = PersonType.Natural },
                    new() { PersonType = PersonType.Legal }
                ]
            },
            new()
            {
                Name = "Código Fiscal de País de Origen",
                Abbreviation = "CFPO",
                ValidationRegex = @"^[A-Za-z0-9\-\/\.]{1,20}$",
                AllowedPersonTypes =
                [
                    new() { PersonType = PersonType.Natural },
                    new() { PersonType = PersonType.Legal }
                ]
            },
            new()
            {
                Name = "Código IBAN - InternationalBank Account Number",
                Abbreviation = "IBAN",
                ValidationRegex = @"^[A-Z]{2}\d{2}[A-Z0-9]{1,30}$",
                AllowedPersonTypes =
                [
                    new() { PersonType = PersonType.Natural },
                    new() { PersonType = PersonType.Legal }
                ]
            },
            new()
            {
                Name = "Código ABA - American Bankers Association",
                Abbreviation = "ABA",
                ValidationRegex = @"^\d{9}$",
                AllowedPersonTypes =
                [
                    new() { PersonType = PersonType.Natural },
                    new() { PersonType = PersonType.Legal }
                ]
            },
            new()
            {
                Name = "No Identificado",
                Abbreviation = "NI",
                ValidationRegex = @"^$|^[A-Za-z0-9\-\/\.]{1,20}$",
                AllowedPersonTypes =
                [
                    new() { PersonType = PersonType.Natural },
                    new() { PersonType = PersonType.Legal }
                ]
            }
        ];

        var toAdd = docTypes.Where(d => !existing.Contains(d.Abbreviation));
        context.DocumentTypes.AddRange(toAdd);
        await context.SaveChangesAsync();
    }

    private async Task SeedDefaultAdminAsync()
    {
        var adminEmail = configuration["Seed:AdminEmail"];
        var adminPassword = configuration["Seed:AdminPassword"];

        if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
            return;

        if (await userManager.FindByEmailAsync(adminEmail) is not null)
            return;

        var admin = new ApplicationUser
        {
            Id = Guid.CreateVersion7(),
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "System"
        };

        var result = await userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, "Admin");
        else
            logger.LogWarning("Failed to create default admin: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}
