using Microsoft.AspNetCore.Identity;

namespace AppTemp.Infrastructure.Identity.RoleClaims;
public class AppRoleClaim : IdentityRoleClaim<string>
{
    public string? CreatedBy { get; init; }
    public DateTimeOffset CreatedOn { get; init; }
}
