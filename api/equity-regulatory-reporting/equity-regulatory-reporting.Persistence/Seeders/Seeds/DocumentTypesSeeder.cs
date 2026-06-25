using equity_regulatory_reporting.Domain.Entities;
using equity_regulatory_reporting.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Persistence.Seeders;

public partial class DatabaseSeeder
{
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
}
