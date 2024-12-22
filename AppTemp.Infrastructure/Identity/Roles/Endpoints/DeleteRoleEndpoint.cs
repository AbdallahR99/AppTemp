using AppTemp.Core.Identity.Roles;
using AppTemp.Infrastructure.Auth.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AppTemp.Infrastructure.Identity.Roles.Endpoints;

public static class DeleteRoleEndpoint
{
    public static RouteHandlerBuilder MapDeleteRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{id:guid}", async (string id, IRoleService roleService) =>
        {
            await roleService.DeleteRoleAsync(id);
        })
        .WithName(nameof(DeleteRoleEndpoint))
        .WithSummary("Delete a role by ID")
        .RequirePermission("Permissions.Roles.Delete")
        .WithDescription("Remove a role from the system by its ID.");
    }
}

