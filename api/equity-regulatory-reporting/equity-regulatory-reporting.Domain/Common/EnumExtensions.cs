using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace equity_regulatory_reporting.Domain.Common;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType().GetMember(value.ToString())[0];
        var display = member.GetCustomAttribute<DisplayAttribute>();
        return display?.Name ?? value.ToString();
    }
}
