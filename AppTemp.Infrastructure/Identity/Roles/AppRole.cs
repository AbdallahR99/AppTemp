using Microsoft.AspNetCore.Identity;

namespace AppTemp.Infrastructure.Identity.Roles;
public class AppRole : IdentityRole
{
    public string? Description { get; set; }

    public AppRole(string name, string? description = null)
        : base(name)
    {
        Description = description;
        NormalizedName = name.ToUpperInvariant();
    }
}
