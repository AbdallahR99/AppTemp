using AppTemp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using AppTemp.Core.Persistence;
using AppTemp.Core.Audit;
using AppTemp.Infrastructure.Identity.Users;
using AppTemp.Infrastructure.Identity.RoleClaims;
using AppTemp.Infrastructure.Identity.Roles;

namespace AppTemp.Infrastructure.Identity.Persistence;
public class IdentityDbContext : IdentityDbContext<AppUser,
    AppRole,
    string,
    IdentityUserClaim<string>,
    IdentityUserRole<string>,
    IdentityUserLogin<string>,
    AppRoleClaim,
    IdentityUserToken<string>>
{
    private readonly DatabaseOptions _settings;
    //private new FshTenantInfo TenantInfo { get; set; }
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, IOptions<DatabaseOptions> settings) : base(options)
    {
        _settings = settings.Value;
        //TenantInfo = multiTenantContextAccessor.MultiTenantContext.TenantInfo!;
    }

    public DbSet<AuditTrail> AuditTrails { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    if (!string.IsNullOrWhiteSpace(TenantInfo?.ConnectionString))
    //    {
    //        optionsBuilder.ConfigureDatabase(_settings.Provider, TenantInfo.ConnectionString);
    //    }
    //}
}
