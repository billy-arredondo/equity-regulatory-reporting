using equity_regulatory_reporting.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace equity_regulatory_reporting.Infrastructure.Auth;

public class HasPermissionAttribute(Permission permission)
    : AuthorizeAttribute($"Permission:{(int)permission}");
