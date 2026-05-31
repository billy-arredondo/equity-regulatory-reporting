namespace equity_regulatory_reporting.Application.Features.Users.Dtos;

public record UserDto(Guid Id, string Email, string FirstName, string LastName, IReadOnlyList<string> Roles);
