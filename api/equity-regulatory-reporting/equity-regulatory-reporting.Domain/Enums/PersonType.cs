using System.ComponentModel.DataAnnotations;

namespace equity_regulatory_reporting.Domain.Enums;

public enum PersonType
{
    [Display(Name = "Persona Natural")]
    Natural = 1,

    [Display(Name = "Persona Jurídica")]
    Legal = 2,

    [Display(Name = "Entidad Jurídica")]
    LegalEntity = 3
}
