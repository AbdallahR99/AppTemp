using AppTemp.Infrastructure.Identity.RoleClaims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AppTemp.Core.Persistence;
using AppTemp.Core.Origin;
using AppTemp.Core.Auth.Shared;
using AppTemp.Infrastructure.Identity.Users;
using AppTemp.Infrastructure.Identity.Roles;

namespace AppTemp.Infrastructure.Identity.Persistence;
internal sealed class IdentityDbInitializer(
    ILogger<IdentityDbInitializer> logger,
    IdentityDbContext context,
    RoleManager<AppRole> roleManager,
    UserManager<AppUser> userManager,
    TimeProvider timeProvider,
    IOptions<OriginOptions> originSettings) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await context.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false)).Any())
        {
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("[{ContextId}] applied database migrations for identity module", context.ContextId);
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await SeedRolesAsync();
        await SeedAdminUserAsync();
    }

    private async Task SeedRolesAsync()
    {
        foreach (string roleName in AppRoles.DefaultRoles)
        {
            if (await roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName)
                is not AppRole role)
            {
                // create role
                role = new AppRole(roleName, $"{roleName} Role");
                await roleManager.CreateAsync(role);
            }

            // Assign permissions
            if (roleName == AppRoles.Basic)
            {
                await AssignPermissionsToRoleAsync(context, AppPermissions.Basic, role);
            }
            else if (roleName == AppRoles.Admin)
            {
                await AssignPermissionsToRoleAsync(context, AppPermissions.Admin, role);

                //if (multiTenantContextAccessor.MultiTenantContext.TenantInfo?.Id == TenantConstants.Root.Id)
                //{
                //    await AssignPermissionsToRoleAsync(context, AppPermissions.Root, role);
                //}
            }
        }
    }

    private async Task AssignPermissionsToRoleAsync(IdentityDbContext dbContext, IReadOnlyList<AppPermission> permissions, AppRole role)
    {
        var currentClaims = await roleManager.GetClaimsAsync(role);
        var newClaims = permissions
            .Where(permission => !currentClaims.Any(c => c.Type == AppClaims.Permission && c.Value == permission.Name))
            .Select(permission => new AppRoleClaim
            {
                RoleId = role.Id,
                ClaimType = AppClaims.Permission,
                ClaimValue = permission.Name,
                CreatedBy = "application",
                CreatedOn = timeProvider.GetUtcNow()
            })
            .ToList();

        foreach (var claim in newClaims)
        {
            logger.LogInformation("Seeding {Role} Permission '{Permission}'", role.Name, claim.ClaimValue);
            await dbContext.RoleClaims.AddAsync(claim);
        }

        // Save changes to the database context
        if (newClaims.Count != 0)
        {
            await dbContext.SaveChangesAsync();
        }

    }

    private async Task SeedAdminUserAsync()
    {

        if (await userManager.Users.FirstOrDefaultAsync(u => u.Email == "admin@admin.com")
            is not AppUser adminUser)
        {
            string adminUserName = $"admin";
            adminUser = new AppUser
            {
                FirstName = "Admin",
                LastName = AppRoles.Admin,
                Email = "admin@admin.com",
                UserName = adminUserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                NormalizedEmail = "admin@admin.com".ToUpperInvariant(),
                NormalizedUserName = adminUserName.ToUpperInvariant(),
                ImageUrl = new Uri(originSettings.Value.OriginUrl! + "../admin-profile.png"),
                IsActive = true
            };

            logger.LogInformation("Seeding Default Admin User");
            var password = new PasswordHasher<AppUser>();
            adminUser.PasswordHash = password.HashPassword(adminUser, "DD@19375");
            await userManager.CreateAsync(adminUser);
        }

        // Assign role to user
        if (!await userManager.IsInRoleAsync(adminUser, AppRoles.Admin))
        {
            logger.LogInformation("Assigning Admin Role to Admin User");
            await userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
        }
    }
}
