using AppTemp.Infrastructure.Identity.Users.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using IdentityConstants = AppTemp.Core.Auth.Shared.IdentityConstants;
using AppTemp.Core.Persistence;
using AppTemp.Core.Identity.Roles;
using AppTemp.Core.Identity.Tokens;
using AppTemp.Core.Audit;
using AppTemp.Core.Identity.Users.Abstractions;
using AppTemp.Infrastructure.Auth;
using AppTemp.Infrastructure.Identity.Tokens.Endpoints;
using AppTemp.Infrastructure.Identity.Users.Endpoints;
using AppTemp.Infrastructure.Identity.Audit;
using AppTemp.Infrastructure.Persistence;
using AppTemp.Infrastructure.Identity.Users.Services;
using AppTemp.Infrastructure.Identity.Users;
using AppTemp.Infrastructure.Identity.Roles;
using AppTemp.Infrastructure.Identity.Persistence;
using AppTemp.Infrastructure.Identity.Roles.Endpoints;
using AppTemp.Infrastructure.Identity.Tokens;

namespace AppTemp.Infrastructure.Identity;
internal static class Extensions
{
    internal static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddScoped<CurrentUserMiddleware>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IAuditService, AuditService>();
        services.BindDbContext<IdentityDbContext>();
        services.AddScoped<IDbInitializer, IdentityDbInitializer>();
        services.AddIdentity<AppUser, AppRole>(options =>
           {
               options.Password.RequiredLength = IdentityConstants.PasswordLength;
               options.Password.RequireDigit = false;
               options.Password.RequireLowercase = false;
               options.Password.RequireNonAlphanumeric = false;
               options.Password.RequireUppercase = false;
               options.User.RequireUniqueEmail = true;
           })
           .AddEntityFrameworkStores<IdentityDbContext>()
           .AddDefaultTokenProviders();
        return services;
    }

    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("api/users").WithTags("users");
        users.MapUserEndpoints();

        var tokens = app.MapGroup("api/token").WithTags("token");
        tokens.MapTokenEndpoints();

        var roles = app.MapGroup("api/roles").WithTags("roles");
        roles.MapRoleEndpoints();

        return app;
    }
}
